using TIZSoft.Database.Attributes;  //original: MicroOrm.Pocos.SqlGenerator.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TIZSoft.Database.SqlGenerator //original: MicroOrm.Pocos.SqlGenerator
{

    public enum SupportedSqlStatement
    {
        MsSql,
        MySql
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SqlGenerator<TEntity> : ISqlGenerator<TEntity> where TEntity : new()
    {
        static readonly Utils.Log.Logger logger = TIZSoft.Utils.Log.LogManager.Default.FindOrCreateLogger<DeployDatabase>();

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public SqlGenerator(SupportedSqlStatement statementType = SupportedSqlStatement.MySql)
        {
            StatementType = statementType;
            this.LoadEntityMetadata();
        }

        private string TryGetTableName(Type type)
        {
            // both support attr "Table" and "StoreAs"
            var aliasAttribute = type.GetCustomAttribute<StoredAs>();
            if (aliasAttribute != null)
                return aliasAttribute.Value;

            var tableAttribute = type.GetCustomAttribute<Dapper.Contrib.Extensions.TableAttribute>();
            if (tableAttribute != null)
                return tableAttribute.Name;

            return string.Empty;
        }

        private string TryGetDatabaseName(Type type, SupportedSqlStatement statementType= SupportedSqlStatement.MySql)
        {
            var schemeAttribute = type.GetCustomAttribute<Scheme>();
            if (schemeAttribute != null)
                return schemeAttribute.Value;

            return statementType == SupportedSqlStatement.MsSql ? "dbo" :　string.Empty;
        }

        private void LoadEntityMetadata()
        {
            var entityType = typeof(TEntity);

            //var aliasAttribute = entityType.GetCustomAttribute<StoredAs>();
            //var schemeAttribute = entityType.GetCustomAttribute<Scheme>();
            string tableName = TryGetTableName(entityType);
            string dbName = TryGetDatabaseName(entityType);

            //this.TableName = aliasAttribute != null ? aliasAttribute.Value : entityType.Name;
            this.TableName = tableName != string.Empty ? tableName : entityType.Name;
            //this.Scheme = schemeAttribute != null ? schemeAttribute.Value : "dbo";
            this.Scheme = dbName ;

            //Load all the "primitive" entity properties
            IEnumerable<PropertyInfo> props = entityType.GetProperties().Where(p => p.PropertyType.IsValueType || 
                                                                                    p.PropertyType.Name.Equals("String", StringComparison.InvariantCultureIgnoreCase) || 
                                                                                    p.PropertyType.Name.Equals("Byte[]", StringComparison.InvariantCultureIgnoreCase));

            //Filter the non stored properties
            this.BaseProperties = props.Where(p => !p.GetCustomAttributes<NonStored>().Any()).Select(p => new PropertyMetadata(p));

            //Filter key properties, both support [Key][PrimaryKey]
            this.KeyProperties = props.Where(p => p.GetCustomAttributes<KeyProperty>().Any()).Select(p => new PropertyMetadata(p));
            if (this.KeyProperties.Count() == 0)
            {
                this.KeyProperties = props.Where(p => p.GetCustomAttributes<PrimaryKeyAttribute>().Any()).Select(p => new PropertyMetadata(p));
            }

            //Use identity as key pattern
            var identityProperty = props.SingleOrDefault(p => p.GetCustomAttributes<KeyProperty>().Any(k => k.Identity));
            this.IdentityProperty = identityProperty != null ? new PropertyMetadata(identityProperty) : null ;

            //Status property (if exists, and if it does, it must be an enumeration)
            var statusProperty = props.FirstOrDefault(p => p.PropertyType.IsEnum && p.GetCustomAttributes<StatusProperty>().Any());

            if (statusProperty != null)
            {
                this.StatusProperty = new PropertyMetadata(statusProperty);
                var statusPropertyType = statusProperty.PropertyType;
                var deleteOption = statusPropertyType.GetFields().FirstOrDefault(f => f.GetCustomAttribute<Deleted>() != null);

                if (deleteOption != null)
                {
                    var enumValue = Enum.Parse(statusPropertyType, deleteOption.Name);

                    if (enumValue != null)
                        this.LogicalDeleteValue = Convert.ChangeType(enumValue, Enum.GetUnderlyingType(statusPropertyType));
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool IsIdentity
        {
            get
            {
                return this.IdentityProperty != null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool LogicalDelete
        {
            get
            {
                return this.StatusProperty != null;
            }
        }

        public SupportedSqlStatement StatementType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Scheme { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public PropertyMetadata IdentityProperty { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<PropertyMetadata> KeyProperties { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<PropertyMetadata> BaseProperties { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public PropertyMetadata StatusProperty { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public object LogicalDeleteValue { get; private set; }

        #endregion

        #region Query generators

        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        public virtual string GetInsert()
        {
            //Enumerate the entity properties
            //Identity property (if exists) has to be ignored
            IEnumerable<PropertyMetadata> properties = (this.IsIdentity ?
                                                        this.BaseProperties.Where(p => !p.Name.Equals(this.IdentityProperty.Name, StringComparison.InvariantCultureIgnoreCase)) :
                                                        this.BaseProperties).ToList();

            string columNames = string.Join(", ", properties.Select(p => string.Format(StatementType == SupportedSqlStatement.MsSql ? 
                                                    "[{0}].[{1}]" : "{0}.{1}", this.TableName, p.ColumnName)));
            string values = string.Join(", ", properties.Select(p => string.Format("@{0}", p.Name)));

            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat(StatementType == SupportedSqlStatement.MsSql ? 
                                        "INSERT INTO [{0}].[{1}] {2} {3} " : "INSERT INTO {0}.{1} {2} {3} ",
                                    this.Scheme,
                                    this.TableName,
                                    string.IsNullOrEmpty(columNames) ? string.Empty : string.Format("({0})", columNames),
                                    string.IsNullOrEmpty(values) ? string.Empty : string.Format(" VALUES ({0})", values));

            //If the entity has an identity key, we create a new variable into the query in order to retrieve the generated id
            if (this.IsIdentity)
            {
                sqlBuilder.AppendLine("DECLARE @NEWID NUMERIC(38, 0)");
                sqlBuilder.AppendLine("SET	@NEWID = SCOPE_IDENTITY()");
                sqlBuilder.AppendLine("SELECT @NEWID");
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// todo :　where clause only support Key columns
        /// </summary>
        /// <returns></returns>
        public virtual string GetUpdate()
        {
            var properties = this.BaseProperties.Where(p => !this.KeyProperties.Any(k => k.Name.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase)));

            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat(StatementType == SupportedSqlStatement.MsSql ? 
                                        "UPDATE [{0}] SET {1} WHERE {2}" : "UPDATE {0} SET {1} WHERE {2}",
                                    //this.Scheme,
                                    this.TableName,
                                    string.Join(", ", properties.Select(p => string.Format(
                                        StatementType == SupportedSqlStatement.MsSql ? "[{0}].[{1}] = @{2}" : "{0}.{1} = @{2}", 
                                            this.TableName, p.ColumnName, p.Name))),
                                    string.Join(" AND ", this.KeyProperties.Select(p => string.Format(
                                        StatementType == SupportedSqlStatement.MsSql ? "[{0}].[{1}] = @{2}" : "{0}.{1} = @{2}", 
                                            this.TableName, p.ColumnName, p.Name))));
            
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string GetSelectAll()
        {
            return this.GetSelect(new { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="rowCount">Maximum number of rows to return</param>
        /// <returns></returns>
        public virtual string GetSelect(object filters, int? rowCount = null)
        {
            //Projection function
            Func<PropertyMetadata, string> projectionFunction = (p) =>
            {
                if (!string.IsNullOrEmpty(p.Alias))
                {
                    return string.Format(StatementType == SupportedSqlStatement.MsSql ? 
                                        "[{0}].[{1}] AS [{2}]" : "{0}.{1} AS {2}", this.TableName, p.ColumnName, p.Name);
                }
                return string.Format(StatementType == SupportedSqlStatement.MsSql ? "[{0}].[{1}]" : "{0}.{1}", this.TableName, p.ColumnName);
            };

            var sqlBuilder = new StringBuilder();

            var rowLimitSql = string.Empty;

            if (rowCount.HasValue)
            {
                rowLimitSql = string.Format("TOP {0} ", rowCount);
            }

            var template = StatementType == SupportedSqlStatement.MsSql ?
                            "SELECT {0}{1} FROM [{2}] WITH (NOLOCK)" : "SELECT {0}{1} FROM {2} ";
            sqlBuilder.AppendFormat(template,
                                    rowLimitSql,
                                    string.Join(", ", this.BaseProperties.Select(projectionFunction)),
                                    //this.Scheme,
                                    this.TableName
                                    );

            //Properties of the dynamic filters object
            var filterProperties = filters.GetType().GetProperties().Select(p => p.Name);
            bool containsFilter = (filterProperties != null && filterProperties.Any());

            if (containsFilter)
                sqlBuilder.AppendFormat(" WHERE {0} ", this.ToWhere(filterProperties, filters));

            //Evaluates if this repository implements logical delete
            if (this.LogicalDelete)
            {
                if (containsFilter)
                    sqlBuilder.AppendFormat(StatementType == SupportedSqlStatement.MsSql ? 
                                            " AND [{0}].[{1}] != {2}" : " AND {0}.{1} != {2}",
                                            this.TableName,
                                            this.StatusProperty.Name,
                                            this.LogicalDeleteValue);
                else
                    sqlBuilder.AppendFormat(StatementType == SupportedSqlStatement.MsSql ? 
                                            " WHERE [{0}].[{1}] != {2}" : " WHERE {0}.{1} != {2}",
                                            this.TableName,
                                            this.StatusProperty.Name,
                                            this.LogicalDeleteValue);
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// todo :　where clause only support Key columns
        /// </summary>
        /// <returns></returns>
        public virtual string GetDelete()
        {
            var sqlBuilder = new StringBuilder();
            
            if (!this.LogicalDelete)
            {
                string template = StatementType == SupportedSqlStatement.MsSql ? "[{0}].[{1}] = @{2}" : "{0}.{1} = @{2}";
                string where_clause = string.Join(" AND ", this.KeyProperties.Select(p => string.Format(template, this.TableName, p.ColumnName, p.Name)));
                logger.Debug("where_clause:" + this.KeyProperties);
                sqlBuilder.AppendFormat(StatementType == SupportedSqlStatement.MsSql ?
                                        "DELETE FROM [{0}] WHERE {1}" : "DELETE FROM {0} WHERE {1}",
                                        //this.Scheme,
                                        this.TableName,
                                        where_clause
                                        );
            }
            else
                sqlBuilder.AppendFormat(StatementType == SupportedSqlStatement.MsSql ? 
                                    "UPDATE [{0}].[{1}] SET {2} WHERE {3}" : "UPDATE {0}.{1} SET {2} WHERE {3}",
                                    this.Scheme,
                                    this.TableName,
                                    string.Format(StatementType == SupportedSqlStatement.MsSql ?
                                        "[{0}].[{1}] = {2}" : "{0}.{1} = {2}", 
                                        this.TableName, this.StatusProperty.ColumnName, this.LogicalDeleteValue),
                                    string.Join(" AND ", this.KeyProperties.Select(
                                        p => string.Format(StatementType == SupportedSqlStatement.MsSql ? 
                                            "[{0}].[{1}] = @{2}" : "{0}.{1} = @{2}", this.TableName, p.ColumnName, p.Name))));

            logger.Debug(sqlBuilder.ToString());
            return sqlBuilder.ToString();
        }

        #endregion

        #region Private utility

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public virtual string ToWhere(IEnumerable<string> properties, object filters)
        {
            return string.Join(" AND ", properties.Select(p => {

                var propertyMetadata = this.BaseProperties.FirstOrDefault(pm => pm.Name.Equals(p, StringComparison.InvariantCultureIgnoreCase));

                var columnName = p;
                var propertyName = p;
                
                if (propertyMetadata != null)
                {
                    columnName = propertyMetadata.ColumnName;
                    propertyName = propertyMetadata.Name;
                }

                var prop = filters.GetType().GetProperty(propertyMetadata.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                var values = prop.GetValue(filters, null);

                if (values == null)
                {
                    return string.Format(StatementType == SupportedSqlStatement.MsSql ? 
                        "[{0}].[{1}] IS NULL" : "{0}.{1} IS NULL", this.TableName, columnName);
                }
                else if((values as IEnumerable) != null && !(values is string))
                {
                    return string.Format(StatementType == SupportedSqlStatement.MsSql ? 
                        "[{0}].[{1}] IN @{2}" : "{0}.{1} IN @{2}", this.TableName, columnName, propertyName);
                }
                else {
                    return string.Format(StatementType == SupportedSqlStatement.MsSql ? 
                        "[{0}].[{1}] = @{2}" : "{0}.{1} = @{2}", this.TableName, columnName, propertyName);
                }

            }));
        }

        #endregion
    }
}

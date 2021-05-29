using TIZSoft.Database.Attributes;  //original: MicroOrm.Pocos.SqlGenerator.Attributes;
using System.Reflection;

namespace TIZSoft.Database.SqlGenerator //original: MicroOrm.Pocos.SqlGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyMetadata
    {
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName
        {
            get
            {
                return string.IsNullOrEmpty(this.Alias) ? this.PropertyInfo.Name : this.Alias;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get 
            {
                return this.PropertyInfo.Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        public PropertyMetadata(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;

            //fetch alias
            var aliasAttribute = this.PropertyInfo.GetCustomAttribute<StoredAs>();
            if (aliasAttribute != null)
            {
                this.Alias = aliasAttribute.Value;
                return;
            }

            var tableAttribute = this.PropertyInfo.GetCustomAttribute<Dapper.Contrib.Extensions.TableAttribute>();
            if (tableAttribute != null)
            {
                this.Alias = tableAttribute.Name;
                return;
            }

            var columnAttribute = this.PropertyInfo.GetCustomAttribute<ColumnAttribute>();
            if (columnAttribute != null)
            {
                this.Alias = columnAttribute.ColName;
                return;
            }
            this.Alias = string.Empty;
        }
    }
}

using System;

namespace TIZSoft.Database.Attributes
{
    /// <summary>
    /// [TableAttribute] : defined in "SQLite", "Dapper.Contrib.Extention", use Dapper first
    /// [PrimaryKeyAttribute] / [KeyAttribute] : defined in "TIZSoft", "Dapper.Contrib.Extention", only TIZSoft works
    /// [ColumnAttribute] : also defined in "SQLite"  
    /// </summary>

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string columnName) { ColName = columnName; }
        public string ColName { get; set; }
    }
}
using System;

namespace TIZSoft.Database
{
    /// <summary>
    /// [TableAttribute] : defined in "SQLite", "Dapper.Contrib.Extention", use Dapper first
    /// [PrimaryKeyAttribute] / [KeyAttribute] : defined in "TIZSoft", "Dapper.Contrib.Extention", only TIZSoft works
    /// </summary>

    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class AutoIncrementAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColomnAttribute : Attribute 
    {
        public ColomnAttribute(string columnName) { ColName = columnName; }
        public string ColName { get; set; }
    }
}

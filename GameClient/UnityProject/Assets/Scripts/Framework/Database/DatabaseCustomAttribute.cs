using System;

namespace TIZSoft.Database
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class AutoIncrementAttribute : Attribute { }

    /// <summary>
    /// ColomnAttribute. for DBSchema Mapping, code tracking and "readability" !
    /// Table Attribute already defined in Dapper.Contrib.Extention 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColomnAttribute : Attribute 
    {
        public ColomnAttribute(string columnName) { }
        public string Name { get; set; }
    }
}

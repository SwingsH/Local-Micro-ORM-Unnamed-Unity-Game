using System;

namespace TIZSoft.Database.Attributes
{
    /// <summary>
    /// [TableAttribute] : defined in "SQLite", "Dapper.Contrib.Extention", use Dapper first
    /// [PrimaryKeyAttribute] / [KeyAttribute] : defined in "TIZSoft", "Dapper.Contrib.Extention", only TIZSoft works
    /// </summary>

    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute { }
}
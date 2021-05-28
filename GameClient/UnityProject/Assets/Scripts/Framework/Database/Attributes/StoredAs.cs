using System;

namespace TIZSoft.Database.Attributes  //original: MicroOrm.Pocos.SqlGenerator.Attributes
{
    /// <summary>
    /// [StoredAs("table or column name")]
    /// For classes or properties that don't match name with its corresponding table or column. 
    /// Use this attribute to specify the table or column name that the SQL Generator has to use.
    /// </summary>
    public class StoredAs : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public StoredAs(string value)
        {
            this.Value = value;
        }
    }
}

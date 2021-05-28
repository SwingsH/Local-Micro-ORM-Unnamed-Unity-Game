using System;

namespace TIZSoft.Database.Attributes  //original: MicroOrm.Pocos.SqlGenerator.Attributes
{
    /// <summary>
    /// Use this attribute to decorate those tables that does not belong to the default database scheme. 
    /// Database name;
    /// </summary>
    public class Scheme : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Scheme(string value)
        {
            this.Value = value;
        }
    }
}

using System;

namespace TIZSoft.Database.Attributes  //original: MicroOrm.Pocos.SqlGenerator.Attributes
{
    /// <summary>
    /// For property or properties that compose the primary key of the table. 
    /// If Identity optional parameter is not specified, 
    /// its default value will be false. Tables with identity primary keys need to set the extra parameter 
    /// "Identity" to true and only one single property might be decorated with this attribute, like this [KeyProperty(Identity = true)].
    /// </summary>
    public class KeyProperty : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Identity { get; set; }
    }
}

using System;

namespace TIZSoft.Database.Attributes  //original: MicroOrm.Pocos.SqlGenerator.Attributes
{
    /// <summary>
    /// For "logical" properties that does not have a corresponding column and have to be ignored by the SQL Generator.
    /// </summary>
    public class NonStored : Attribute
    {
    }
}

using System;

namespace TIZSoft.Database.Attributes  //original: MicroOrm.Pocos.SqlGenerator.Attributes
{
    /// <summary>
    /// For tables that implements "logical deletes" instead of physical deletes. 
    /// This attribute can decorate only enum properties and one of the possible values for that enumeration has to be 
    /// decorated with the "Deleted" attribute
    /// </summary>
    public class StatusProperty : Attribute
    {
    }
}

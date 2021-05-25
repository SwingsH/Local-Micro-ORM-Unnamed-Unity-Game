using System;
using Dapper.Contrib.Extensions;

[Table("user")]
public class UserSave
{
    [Key]
    public int UserID { get; set; }
    string Name { get; set; }
    string NickName { get; set; }
    DateTime CreateDatetime { get; set; }
}

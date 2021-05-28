using System;
using TIZSoft.Database.Attributes;
using Dapper.Contrib.Extensions;

[Table("user")]
public class UserSave
{
    [PrimaryKey]
    [Column("serial_id")]
    public int Id { get; set; }
    [Column("guid")]
    public string Guid { get; set; }
    [Column("user_name")]
    public string Name { get; set; }
    [Column("screen_name")]
    public string ScreenName { get; set; }
    [Column("create_time")]
    public DateTime CreateDatetime { get; set; }
    [Column("lastlogin_time")]
    public DateTime LastLoginDatetime { get; set; }
    [Column("lastlogin_device_name")]
    public string LastLoginDeviceName { get; set; }
    [Column("lastlogin_device_lang")]
    public string LastLoginDeviceLang { get; set; }
}

using System;
using TIZSoft.Database;
using Dapper.Contrib.Extensions;

[Table("user")]
public class UserSave
{
    [Key]
    [Colomn("serial_id")]
    public int Id { get; set; }
    [Colomn("guid")]
    string Guid { get; set; }
    [Colomn("user_name")]
    string Name { get; set; }
    [Colomn("screen_name")]
    string ScreenName { get; set; }
    [Colomn("create_time")]
    DateTime CreateDatetime { get; set; }
    [Colomn("lastlogin_time")]
    DateTime LastLoginDatetime { get; set; }
    [Colomn("lastlogin_device_name")]
    string LastLoginDeviceName { get; set; }
    [Colomn("lastlogin_device_lang")]
    string LastLoginDeviceLang { get; set; }
}

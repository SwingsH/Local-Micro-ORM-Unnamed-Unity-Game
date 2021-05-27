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
    public string Guid { get; set; }
    [Colomn("user_name")]
    public string Name { get; set; }
    [Colomn("screen_name")]
    public string ScreenName { get; set; }
    [Colomn("create_time")]
    public DateTime CreateDatetime { get; set; }
    [Colomn("lastlogin_time")]
    public DateTime LastLoginDatetime { get; set; }
    [Colomn("lastlogin_device_name")]
    public string LastLoginDeviceName { get; set; }
    [Colomn("lastlogin_device_lang")]
    public string LastLoginDeviceLang { get; set; }
}

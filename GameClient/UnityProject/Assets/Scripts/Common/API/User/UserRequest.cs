using TIZSoft.UnityHTTP;

[EntryPoint("/user/my")]
public class UserRequest : APIRequest
{
	public byte m { get; set; } //( 1: get my userinfo)
}

[EntryPoint("/user/change-name")]
public class UserNameChangeRequest : APIRequest
{
	public string name { get; set; }
}

[EntryPoint("/user/change-team")]
public class UserTeamChangeRequest : APIRequest
{
	public byte teamId { get; set; }
}
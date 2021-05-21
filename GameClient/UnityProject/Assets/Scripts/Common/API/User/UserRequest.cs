
//[EntryPoint("/ajax/user/my")]
public class UserRequest //: Api
{
	/// <summary>
	/// （1:取得）
	/// </summary>
	public byte m { get; set; }
}


//[EntryPoint("/ajax/user/change-name")]
public class UserNameChangeRequest //: Api
{
	public string name { get; set; }
}

//[EntryPoint("/ajax/user/change-team")]
public class UserTeamChangeRequest //: Api
{
	public byte teamId { get; set; }
}
//[EntryPoint("/ajax/register/create")]
public class RegisterRequest //: Api
{
	public string Name { get; set; }
	public string TeamName { get; set; }
	//public Team TeamId { get; set; }
	public string D { get; set; }
	public string M { get; set; }
	public string C { get; set; }
	public string A { get; set; }
	public string P { get; set; }
	public bool TutorialSkip { get; set; }
}

//[EntryPoint("/ajax/register/check-account")]
public class RegisterCheckAccountRequest //: Api
{
}

//[EntryPoint("ajax/push-notification/device/post")]
public class RegisterPushNotificationTokenRequest //: Api
{
	public string mainToken { get; set; }
	public int clientVer { get; set; }
	public string deviceToken { get; set; }
}

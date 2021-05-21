public class RegisterResponse //: Response
{
	//public UserModel User { get; set; }

	/// <summary>
	/// ptCookie
	/// </summary>
	public string P { get; set; }

	public string H { get; set; }
}

[System.Serializable]
public class RegisterPushNotificationResponse //: Response
{
	//public RegisterPushNotificationModel Result { get; set; }
}

public class RegisterCheckAccountResponse //: Response
{
	//public AdvancedUserModel Au { get; set; }
}


[System.Serializable]
public class registerAccountResponse //: Response
{
	//public RegisterAcountModel Result { get; set; }
}

/// <summary>
/// 新アドバンスログイン、アカウント取得レスポンス
/// </summary>
[System.Serializable]
public class getAccountResponse //: Response
{
	//public UserAcountResponsModel Result { get; set; }
}
using System.Collections.Generic;

public class ClientCustomHeader
{
    readonly CustomHeaderSave violinSave;
    readonly CustomHeaderUserInfo localUser;

    public ClientCustomHeader(CustomHeaderSave violinSave, CustomHeaderUserInfo localUser)
    {
        this.localUser = localUser;
        this.violinSave = violinSave;
    }

    public IEnumerable<KeyValuePair<string, string>> CreateHeaders()
    {
        var headers = new Dictionary<string, string>();
        headers["User-Id"] = "CustomHeaderSave.id";
        var token = "CustomHeaderUserInfo";
        headers["Token"] = token;
        return headers;
    }
}

public class CustomHeaderSave { };
public class CustomHeaderUserInfo { };
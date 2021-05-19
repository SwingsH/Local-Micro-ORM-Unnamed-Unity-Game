using System.Collections.Generic;
using System;
using UnityHTTPServer;

public enum APIToken : int
{
    [EnumClassValue(typeof(ServiceBase), "login")]            Login = 0,
    [EnumClassValue(typeof(ServiceBase), "user")]              User,
    [EnumClassValue(typeof(ServiceBase), "mail")]             Mail,
    [EnumClassValue(typeof(ServiceBase), "shop")]             Shop,
    [EnumClassValue(typeof(ServiceBase), "user/friends")]     Friends,
    [EnumClassValue(typeof(ServiceBase), "user/character")]   Character
}

public class ServiceController
{
    private Dictionary<string, ServiceBase> services;

    private Dictionary<ServiceBase, string> allAPIPath;

    public ServiceController() { }
    public void Initialize()
    {
        services = new Dictionary<string, ServiceBase>();
        allAPIPath = new Dictionary<ServiceBase, string>();

        IAccountService accountService = new AccountService(null);

        //accountService.GetAccountInfo = () => Registe();
        Registe(ServerType.GameHost, "account/", null, null);
        Registe(ServerType.GameHost, "account-bag/", null, null);
        Registe(ServerType.GameHost, "account-detail/", null, null);
        Registe(ServerType.GameHost, "account-equip/", null, null);
    }

    public void Registe(ServerType serverType, string api, object data, Action<ClientHttpRequest> onResponse)
    {

    }

    public void Registe() { }
}

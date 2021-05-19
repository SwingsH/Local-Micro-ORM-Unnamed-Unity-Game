using UnityHTTPServer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountService : ServiceBase, IAccountService
{
    public void GetAccountInfo(Action<ClientHttpRequest> onResponse) { }

    public void GetCharacterInfo(Action<ClientHttpRequest> onResponse) { }

    public void RefreshStatus(Action<ClientHttpRequest> onResponse) { }

    // Start is called before the first frame update
    public AccountService(IGameNetwork network): base(network)
    {

    }
}

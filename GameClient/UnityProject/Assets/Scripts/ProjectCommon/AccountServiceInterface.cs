using System;
using UnityHTTPServer;

public interface IAccountService
{
    void GetAccountInfo(Action<ClientHttpRequest> onResponse);
    void GetCharacterInfo(Action<ClientHttpRequest> onResponse);
    void RefreshStatus(Action<ClientHttpRequest> onResponse);
}

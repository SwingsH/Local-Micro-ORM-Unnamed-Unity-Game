using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TIZSoft.Services;
using TIZSoft.UnityHTTP.Client;

public class TestService : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ClientHttpManager httpManager = new ClientHttpManager();
        ClientHostManager.Settings settings = new ClientHostManager.Settings {
            Hosts = new List<ClientHostManager.Settings.Entry> {
                new ClientHostManager.Settings.Entry { HostId = "suck_hostid", Host = "http://app.domain.com" },
                new ClientHostManager.Settings.Entry { HostId = "suck_hostid_2", Host = "http://app.domain.com" }
            },
            DefaultHostId = "suck_hostid"
        };
        ClientHostManager hostManager = new ClientHostManager(settings);
        ClientWebServices webService = new ClientWebServices(httpManager, hostManager);

        ClientHTTPNetwork clientHTTPNetwork = new ClientHTTPNetwork();
        GameServices tempService = new GameServices(webService, "volleyballjr", clientHTTPNetwork);

        clientHTTPNetwork.AddServer("suck_groupname", ServerType.GameHost, "http://127.0.0.1/");

        tempService.CallAPI<UserRequest>( API_METHOD.HTTP_GET, new UserRequest { m=1 }, OnResponse);
    }

    public void OnResponse(ClientHttpRequest request)
    {
        Debug.Log("request");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
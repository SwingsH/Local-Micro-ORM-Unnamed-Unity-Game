using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tizsoft.Net;

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

        //hostManager.AddHost("suck_hostid", "http://127.0.0.1");
        ClientHTTPNetwork clientHTTPNetwork = new ClientHTTPNetwork();
        TempService tempService = new TempService(webService, "usertest", clientHTTPNetwork);

        clientHTTPNetwork.AddServer("suck_groupname", ServerType.GameHost, "http://localhost");

        tempService.GetUser("wahaha");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class TempService : GameServices
{
    public void GetUser(string name)
    {
        Get(ServerType.GameHost, "user", this.WebService, OnResponse);
    }
    public TempService(ClientWebServices webService, string prefix, ClientHTTPNetwork network): base(webService, prefix, network)
    {
        
    }

    public void OnResponse(ClientHttpRequest request)
    {
        Debug.Log("request");
    }
}

/*
 * {
  "HttpManager": {
    "RequestQueueCount": 0,
    "SendingRequestCount": 0,
    "TotalRequestCount": 0,
    "RequestCustomHeaders": null
  },
  "HostManager": {
    "CurrentHostId": "1",
    "CurrentHost": "http://app.domain.com",
    "Count": 1
  }
}
*/


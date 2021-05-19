using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceInfo
{
    public string Name;
    public ServiceInfo() { }
}

public class ServiceBase: ServiceInterface
{
    private ServiceInfo serviceInfo;
    public ServiceBase(IGameNetwork network) {
        serviceInfo = new ServiceInfo { Name = this.ToString() };
    }

    public void Get() { }
    public void Post() { }
    public void Head() { }
    public void Delete() { }
    public void Create() { }
    public void Read() { }
    public void Update() { }

    public ServiceInfo Info => serviceInfo;
}

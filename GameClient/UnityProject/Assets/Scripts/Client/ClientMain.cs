#if CLIENT_APP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientMain : UnitySingletonBase<ClientMain>
{
    // Dictionary< IServices, key > _services; // maybe ser service or client service 
    // Dictionary< IRepository, key > _repositories; // maybe ser repo or client repo 

    // remote procedure call ?
    // get http data like from local ? super big repository (database CRUD)
    void Start()
    {
        // Service , Repository , Control, View
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
#endif

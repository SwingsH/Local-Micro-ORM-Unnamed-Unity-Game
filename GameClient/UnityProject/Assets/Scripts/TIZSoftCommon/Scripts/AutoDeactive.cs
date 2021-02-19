using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDeactive : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false);
    }
}

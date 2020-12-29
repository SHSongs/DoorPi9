using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorGameManager : MonoBehaviour
{

    private GameObject _networkGameObject;

    private NetworkDivider _networkDivider;
    
    // Start is called before the first frame update
    void Start()
    {
        _networkGameObject = GameObject.Find("Network");

        _networkDivider = _networkGameObject.GetComponent<NetworkDivider>();
        
        
        
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    
    public InputField IPAddress;

    public GameObject NetworkGameObject;

    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(NetworkGameObject);

    }

    public void HostButton()
    {
        PlayerInfo.GetInstance().SetPlayerId(0);
        SceneManager.LoadScene("main");
    }

    public void ClientButton()
    {
        string address = IPAddress.text;
        
        PlayerInfo.GetInstance().SetPlayerId(1);
        PlayerInfo.GetInstance().hostAdress = address;
        
        SceneManager.LoadScene("main");
        
                
    }
}

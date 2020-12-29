using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Threading;

public class NetworkDivider : MonoBehaviour
{
    public GameObject remote;
    public bool IsServer;
    
    List<PlayerMovementManager> playerMovementManagers = new List<PlayerMovementManager>();

    private ChatState		m_state = ChatState.HOST_TYPE_SELECT;


    public string m_hostAdress = "127.0.0.1";
    private List<string>[]	m_message;

    private static int		CHAT_MEMBER_NUM = 2;
    private	bool			m_isServer = false;

    public Network _network;
    
    protected Thread		m_thread = null;
    private bool CreateThread = false;


    public int playerId = 0;
    
    enum ChatState {
        HOST_TYPE_SELECT = 0,	// 방 선택.
        CHATTING,				// 채팅 중.
        LEAVE,					// 나가기.
        ERROR,					// 오류.
    };

    private float x;
    private float y;
    private float z;


    public void OnReceiveStatPacket(PacketId id, byte[] data)
    {
        PlayerStatDataPacket packet = new PlayerStatDataPacket(data);
        PlayerStat ps = packet.GetPacket();
        FindRemotePlayer().HP = ps.HP;

        FindRemotePlayer().EnemyHp.text = FindRemotePlayer().HP.ToString();
        
        Debug.Log("적체력" + FindRemotePlayer().HP);
    }
    
    
    public void OnReceiveCoordPacket(PacketId id, byte[] data)
    {
		
        PlayerCoordDataPacket packet = new PlayerCoordDataPacket(data);
        PlayerCoordinate pc = packet.GetPacket();

        Vector3 dir = new Vector3(pc.Coordinate.x, pc.Coordinate.y, pc.Coordinate.z);
        
        FindRemotePlayer().SetDestination(dir);
           
    }

    public void OnReceiveAttackPacket(PacketId id, byte[] data)
    {
        PlayerAttackDataPacket packet = new PlayerAttackDataPacket(data);
        PlayerAttack pa = packet.GetPacket();
        
        
        Vector3 dir = new Vector3(pa.Coordinate.x, pa.Coordinate.y, pa.Coordinate.z);
        
        FindRemotePlayer().ThrowKnife(dir);
    }

    
    private PlayerMovementManager FindRemotePlayer()
    {
        foreach (var pmm in playerMovementManagers)
        {
            if (pmm.playerId != PlayerInfo.GetInstance().GetPlayerId())
            {
                return pmm;
            }
        }

        return null;
    }
    
    private PlayerMovementManager FindPlayer(int id)
    {
        foreach (var pmm in playerMovementManagers)
        {
            if (pmm.playerId == id)
            {
                return pmm;
            }
        }
        Debug.Log("player not found" + id);
        return null;
    }


   
    // Start is called before the first frame update
    void Start()
    {
        
        GameObject[] playerObjectss = GameObject.FindGameObjectsWithTag("Player");
                
        foreach (var pgb in playerObjectss)
        {
            playerMovementManagers.Add(pgb.GetComponent < PlayerMovementManager > ());
        }

        
        m_hostAdress = PlayerInfo.GetInstance().hostAdress;
        if (PlayerInfo.GetInstance().GetPlayerId() == 0)
        {
            IsServer = true;
            FindPlayer(0).isRemote = false;
            FindPlayer(1).isRemote = true;
        }
        else
        {
            IsServer = false;
            FindPlayer(0).isRemote = true;
            FindPlayer(1).isRemote = false;
        }

        
        
        SetUpInGameNetwork();
    }  

    public void SetUpInGameNetwork()
    {
        Debug.Log("SetUpInGame");

        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        System.Net.IPAddress hostAddress = hostEntry.AddressList[0];
        Debug.Log(hostEntry.HostName);
        //m_hostAddress = hostAddress.ToString ();
        
        GameObject go = GameObject.FindGameObjectWithTag("network");
       
        _network = go.GetComponent<Network>();
        _network.RegisterReceiveNotification(PacketId.Coord, OnReceiveCoordPacket);
        _network.RegisterReceiveNotification(PacketId.Attack, OnReceiveAttackPacket);
        _network.RegisterReceiveNotification(PacketId.Stat, OnReceiveStatPacket);

       
        
        if (IsServer)
        {
            _network.StartServer();
        }
        else
        {
            bool ret = _network.Connect(m_hostAdress);
            if (ret) 
            {
                m_state = ChatState.CHATTING;
            }
            else
            {
                m_state = ChatState.ERROR;
                Debug.Log("ERROR.");
            }
        }
    }


    
    // Update is called once per frame
    void Update()
    {
        x = this.transform.position.x;
        y = this.transform.position.y;
        z = this.transform.position.z;
        
        switch (m_state) {
            case ChatState.HOST_TYPE_SELECT:
            
                break;

            case ChatState.CHATTING:

                if (!CreateThread)
                {
                    try {
                        // Dispatch용 스레드 시작.
                        m_thread = new Thread(new ThreadStart(Dispatch));
                        m_thread.Start();
                        CreateThread = true;
                    }
                    catch {
                        Debug.Log("Cannot launch thread.");
                     
                    }
                    
                }
                    
                
                // UpdateChatting();
                break;
			
            case ChatState.LEAVE:
                UpdateLeave();
                break;
        }


    }
    
    public void Dispatch()
    {
        Debug.Log("Dispatch thread started.");

        while (true)
        {
     
            byte[] reciveData = new byte[1000];
            int len = 0;
            
            Thread.Sleep(50);
        }

        Debug.Log("Dispatch thread ended.");
    }
    
    

    string GetPosition()
    {
        string x = this.x.ToString(); 
        string y = this.y.ToString(); 
        string z = this.z.ToString(); 

        string position = x + "," + y + "," + z;

        return position;
    }
    

    public void UpdateLeave()
    {
        if (m_isServer == true) {
            _network.StopServer();
        }
        else {
            _network.Disconnect();
        }

        // 메시지 삭제.
        for (int i = 0; i < 2; ++i) {
            m_message[i].Clear();
        }
		
        m_state = ChatState.HOST_TYPE_SELECT;
    }
    

    
}

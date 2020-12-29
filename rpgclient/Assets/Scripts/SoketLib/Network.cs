using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;


public class Network : MonoBehaviour
{
    
    
    // 이벤트 알림 델리게이트.
    public delegate void 	EventHandler(NetEventState state);
    // 이벤트 핸들러.
    private EventHandler	m_handler;
    
    private	string 			m_hostAddress = "127.0.0.1";
	
    private const int 		m_port = 50765;
    
    private TransportTCP	m_transport;

    // 송수신용 패킷 최대 크기.
    private const int		m_packetSize = 1400;

    // 수신 패킷 처리함수 델리게이트. 
    public delegate	void RecvNotifier(PacketId id, byte[] data);
    
    // 수신 패킷 분배 해시 테이블.
    private Dictionary<int, RecvNotifier> m_notifier = new Dictionary<int, RecvNotifier>();

    public void RegisterReceiveNotification(PacketId id, RecvNotifier notifier)
    {
        int index = (int)id;

        if (m_notifier.ContainsKey(index)) {
            m_notifier.Remove(index);
        }

        m_notifier.Add(index, notifier);
    }
    // Start is called before the first frame update
    void Start()
    {
        m_transport = GetComponent<TransportTCP>();
        m_transport.RegisterEventHandler(OnEventHandling);
        
    }

    public void Send(byte[] data, int size)
    {
        m_transport.Send(data, size);
    }

    public int Receive(ref byte[] buffer, int size)
    {
        return m_transport.Receive(ref buffer, size);
    }
    // Update is called once per frame
    void Update()
    {
        
        byte[] packet = new byte[m_packetSize];
	
        // 도달 보증 패킷을 수신합니다.
        while (m_transport && Receive(ref packet, packet.Length) > 0) {
            // 수신 패킷을 분배합니다.
            ReceivePacket(packet);
        }
    }
    
    public void ReceivePacket(byte[] data)
    {
        PacketHeader header = new PacketHeader();
        HeaderSerializer serializer = new HeaderSerializer();
		
        bool ret = serializer.Deserialize(data, ref header);
        if (ret == false) {
            // 패킷으로서 인식할 수 없으므로 폐기합니다.
            return;	
        }
        
        int packetId = (int)header.packetId;
        if (m_notifier.ContainsKey(packetId) &&
            m_notifier[packetId] != null) {
            int headerSize = Marshal.SizeOf(typeof(PacketHeader));//sizeof(PacketId) + sizeof(int);
            byte[] packetData = new byte[data.Length - headerSize];
            Buffer.BlockCopy(data, headerSize, packetData, 0, packetData.Length);
	    
            m_notifier[packetId]((PacketId)packetId, packetData);
        }
    }
    
    
    public void OnEventHandling(NetEventState state)
    {
        switch (state.type) {
            case NetEventType.Connect:
                if (m_transport.IsServer()) {
                }
                else {
                }
                break;

            case NetEventType.Disconnect:
                if (m_transport.IsServer()) {
                }
                else {
                }
                break;
        }
    }
    
    
    public bool Connect(string hostAddress)
    {
        bool success =  m_transport.Connect(hostAddress, m_port);
        
        // 접속을 알립니다.
        if (m_handler != null) {
            NetEventState state = new NetEventState();
            state.type = NetEventType.Connect;
            state.result = NetEventResult.Success;
            m_handler(state);
        }

        return success;
    }
    
    public void StartServer()
    {
        m_transport.StartServer(m_port, 1);

    }
    public void StopServer()
    {
        m_transport.StopServer();
    }
    
    public void Disconnect()
    {
        m_transport.Disconnect();
    }

    // 이벤트 통지 함수 등록.
    public void RegisterEventHandler(EventHandler handler)
    {
        m_handler += handler;
    }
	
    // 이벤트 통지 함수 삭제.
    public void UnregisterEventHandler(EventHandler handler)
    {
        m_handler -= handler;
    }

    public int SendReliable<T>(IPacket<T> packet)
    {
        int sendSize = 0;
		
        if (m_transport != null) {
            // 모듈에서 사용할 헤더 정보를 생성합니다.
            PacketHeader header = new PacketHeader();
            HeaderSerializer serializer = new HeaderSerializer();
			
            header.packetId = packet.GetPacketId();
			
            byte[] headerData = null;
            if (serializer.Serialize(header) == true) {
                headerData = serializer.GetSerializedData();
            }
			
            byte[] packetData = packet.GetData();
            byte[] data = new byte[headerData.Length + packetData.Length];
			
            int headerSize = Marshal.SizeOf(typeof(PacketHeader));
            Buffer.BlockCopy(headerData, 0, data, 0, headerSize);
            Buffer.BlockCopy(packetData, 0, data, headerSize, packetData.Length);
			
            string str = "Send reliable packet[" +  header.packetId  + "]";
			
            sendSize = m_transport.Send(data, data.Length);
        }
		
        return sendSize;
    }

}

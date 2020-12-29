//
// 캐릭터의 이동 정보.
//
public struct CharacterData
{
    public string			characterId;	// 캐릭터 ID.
    public int 				index;			// 위치 좌표 인덱스.
    public int				dataNum;		// 좌표 데이터 수.
    public CharacterCoord[]	coordinates;	// 좌표 데이터.
	
    public const int 	characterNameLength = 64;	// 캐릭터 ID 길이.
}

//
// 캐릭터 좌표 패킷 정의.
//
public class CharacterDataPacket : IPacket<CharacterData>
{
    class CharacterDataSerializer : Serializer
    {
        //
        public bool Serialize(CharacterData packet)
        {
			
            Serialize(packet.characterId, CharacterData.characterNameLength);
			
            Serialize(packet.index);
            Serialize(packet.dataNum);
			
            for (int i = 0; i < packet.dataNum; ++i) {
                // CharacterCoord
                Serialize(packet.coordinates[i].x);
                Serialize(packet.coordinates[i].z);
            }	
			
            return true;
        }
		
        //
        public bool Deserialize(ref CharacterData element)
        {
            if (GetDataSize() == 0) {
                // 데이터가 설정되어 있지 않습니다.
                return false;
            }
			
            Deserialize(ref element.characterId, CharacterData.characterNameLength);
			
            Deserialize(ref element.index);
            Deserialize(ref element.dataNum);
			
            element.coordinates = new CharacterCoord[element.dataNum];
            for (int i = 0; i < element.dataNum; ++i) {
                // CharacterCoord
                Deserialize(ref element.coordinates[i].x);
                Deserialize(ref element.coordinates[i].z);
            }
			
            return true;
        }
    }
	
    // 패킷 데이터의 실체.
    CharacterData		m_packet;
	
    public CharacterDataPacket(CharacterData data)
    {
        m_packet = data;
    }
	
    public CharacterDataPacket(byte[] data)
    {
        CharacterDataSerializer serializer = new CharacterDataSerializer();
		
        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref m_packet);
    }
	
    public PacketId	GetPacketId()
    {
        return PacketId.CharacterData;
    }
	
    public CharacterData	GetPacket()
    {
        return m_packet;
    }
	
	
    public byte[] GetData()
    {
        CharacterDataSerializer serializer = new CharacterDataSerializer();
		
        serializer.Serialize(m_packet);
		
        return serializer.GetSerializedData();
    }
}

public struct PlayerCoordinate
{
    public int PlayerId;
    public Coordinate Coordinate;
}
public struct Coordinate
{
    public float x;
    public float y;
    public float z;
}

public class PlayerCoordDataPacket : IPacket<PlayerCoordinate>
{

    public class CoordSerializer : Serializer
    {

        public bool Serialize(PlayerCoordinate packet)
        {
            // 각 요소를 차례로 시리얼라이즈합니다.
            bool ret = true;
            ret &= Serialize(packet.PlayerId);
            ret &= Serialize(packet.Coordinate.x);
            ret &= Serialize(packet.Coordinate.y);
            ret &= Serialize(packet.Coordinate.z);
            return ret;
        }

        public bool Deserialize(ref PlayerCoordinate serialized)
        {
            if (GetDataSize() == 0)
            {
                return false;
            }

            bool ret = true;
            // 데이터의 요소별로 디시리얼라이즈합니다.
            ret &= Deserialize(ref serialized.PlayerId);
            ret &= Deserialize(ref serialized.Coordinate.x); 
            ret &= Deserialize(ref serialized.Coordinate.y); 
            ret &= Deserialize(ref serialized.Coordinate.z);


            return ret;
        }
    }

    
    // 패킷 데이터의 실체.
    PlayerCoordinate		m_packet;
    
    public PlayerCoordDataPacket(PlayerCoordinate data)
    {
        m_packet = data;
    }

    
    
    public PlayerCoordDataPacket(byte[] data)
    {
        CoordSerializer serializer = new CoordSerializer();
		    
        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref m_packet);
    }
    public PacketId GetPacketId()
    {
        return PacketId.Coord;
    }

    public PlayerCoordinate GetPacket()
    {
        return m_packet;
    }

    public byte[] GetData()
    {
        CoordSerializer serializer = new CoordSerializer();
		
        serializer.Serialize(m_packet);
		
        return serializer.GetSerializedData();
        
    }
}


public struct PlayerAttack
{
    public int PlayerId;
    public Coordinate Coordinate;
}

public class PlayerAttackDataPacket : IPacket<PlayerAttack>
{
    public class AttackSerializer : Serializer
    {
        public bool Serialize(PlayerAttack packet)
        {
            bool ret = true;
            ret &= Serialize(packet.PlayerId);
            ret &= Serialize(packet.Coordinate.x);
            ret &= Serialize(packet.Coordinate.y);
            ret &= Serialize(packet.Coordinate.z);
            return ret;
        }
        
        
        public bool Deserialize(ref PlayerAttack serialized)
        {
            if (GetDataSize() == 0)
            {
                return false;
            }

            bool ret = true;
            // 데이터의 요소별로 디시리얼라이즈합니다.
            ret &= Deserialize(ref serialized.PlayerId);
            ret &= Deserialize(ref serialized.Coordinate.x); 
            ret &= Deserialize(ref serialized.Coordinate.y); 
            ret &= Deserialize(ref serialized.Coordinate.z);


            return ret;
        }
    }
    
    private PlayerAttack m_packet;

    public PlayerAttackDataPacket(PlayerAttack mPacket)
    {
        m_packet = mPacket;
    }

    public PlayerAttackDataPacket(byte[] data)
    {
        AttackSerializer serializer = new AttackSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref m_packet);
    }
    public PacketId GetPacketId()
    {
        return PacketId.Attack;
    }

    public PlayerAttack GetPacket()
    {
        return m_packet;
    }

    public byte[] GetData()
    {
        AttackSerializer serializer = new AttackSerializer();

        serializer.Serialize(m_packet);

        return serializer.GetSerializedData();
    }
}

public struct PlayerStat
{
    public int PlayerId;
    public int HP;
}

public class PlayerStatDataPacket : IPacket<PlayerStat>
{
    public class StatSerializer : Serializer
    {
        public bool Serialize(PlayerStat packet)
        {
            // 각 요소를 차례로 시리얼라이즈합니다.
            bool ret = true;
            ret &= Serialize(packet.PlayerId);
            ret &= Serialize(packet.HP);
            return ret;
        }

        public bool Deserialize(ref PlayerStat serialized)
        {
            if (GetDataSize() == 0)
            {
                return false;
            }

            bool ret = true;
            // 데이터의 요소별로 디시리얼라이즈합니다.
            ret &= Deserialize(ref serialized.PlayerId);
            ret &= Deserialize(ref serialized.HP); 


            return ret;
        }
    }

    private PlayerStat m_packet;

    public PlayerStatDataPacket(PlayerStat mPacket)
    {
        m_packet = mPacket;
    }
    
    public PlayerStatDataPacket(byte[] data)
    {
        StatSerializer serializer = new StatSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref m_packet);
    }

    public PacketId GetPacketId()
    {
        return PacketId.Stat;
    }

    public PlayerStat GetPacket()
    {
        return m_packet;
    }

    public byte[] GetData()
    {
        StatSerializer serializer = new StatSerializer();

        serializer.Serialize(m_packet);

        return serializer.GetSerializedData();
    }
}

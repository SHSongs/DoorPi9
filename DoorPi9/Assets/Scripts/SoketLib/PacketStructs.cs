using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum PacketId
{
    // 게임용 패킷.
    Coord,
    Attack,
    Stat,
    GameSyncInfo,
    GameSyncInfoHouse,
    CharacterData,
    ItemData,
    Moving,
    GoingOut,
    ChatMessage,
	
    Max,
}

public struct PacketHeader
{
    // 패킷 종류.
    public PacketId 	packetId;
}

//
// 캐릭터 좌표 정보.
//
public struct CharacterCoord
{
    public float	x;		// 캐릭터의 x좌표.
    public float	y;		// 캐릭터의 z좌표.
    public float	z;		// 캐릭터의 z좌표.
	
    public CharacterCoord(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Vector3	ToVector3()
    {
        return(new Vector3(this.x, y, this.z));
    }
    public static CharacterCoord	FromVector3(Vector3 v)
    {
        return(new CharacterCoord(v.x,v.y, v.z));
    }
	
    public static CharacterCoord	Lerp(CharacterCoord c0, CharacterCoord c1, float rate)
    {
        CharacterCoord	c;
		
        c.x = Mathf.Lerp(c0.x, c1.x, rate);
        c.y = Mathf.Lerp(c0.y, c1.y, rate);
        c.z = Mathf.Lerp(c0.z, c1.z, rate);
		
        return(c);
    }
}
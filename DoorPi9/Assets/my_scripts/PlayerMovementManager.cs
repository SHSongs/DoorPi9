using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementManager : MonoBehaviour
{
    public bool isRemote;
    public int playerId;
    
    private GameObject _networkObject;

    private NetworkDivider _networkDivider;
    private Network _network;
    
    // Start is called before the first frame update
    private Camera camera;
    private bool isMove;
    private Vector3 destination;
    public float move_speed = 3.0f;

    public GameObject knife;
    public GameObject direction;

    private int _hp;

    public Text MyHp;
    public Text EnemyHp;
    
    private Animator m_animator = null;

    public int HP
    {
        get => _hp; 
        set => _hp = value;
    }
    
    
    private void Start()
    {
        _networkObject = GameObject.FindGameObjectWithTag("network");
        _networkDivider = _networkObject.GetComponent<NetworkDivider>();
        _network = _networkObject.GetComponent<Network>();

        HP = 100;
    }

    private void Awake()
    {
        m_animator = gameObject.GetComponent<Animator>();
        camera = Camera.main;
    }

    void Update()
    {

        if (!isRemote)
        {
            RaycastHit hit;
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                SetKnifeDirection(hit.point);
            }
        
            if (Input.GetMouseButton(1))
            {
                if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.transform.tag == "plane")
                    {
                        SetDestination(hit.point);


                        PlayerInfo.GetInstance().GetPlayerId();
                        PlayerCoordinate playerCoordinate = new PlayerCoordinate();

                        playerCoordinate.PlayerId = PlayerInfo.GetInstance().GetPlayerId();
                        playerCoordinate.Coordinate.x = hit.point.x;
                        playerCoordinate.Coordinate.y = hit.point.y;
                        playerCoordinate.Coordinate.z = hit.point.z;


                        PlayerCoordDataPacket packet = new PlayerCoordDataPacket(playerCoordinate);
                        _networkDivider._network.SendReliable(packet);
                    }

                }

            }else if (Input.GetKeyDown(KeyCode.Q))
            {
                if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    ThrowKnife(hit.point);
                    
                    PlayerAttack playerAttack = new PlayerAttack();

                    playerAttack.PlayerId = PlayerInfo.GetInstance().GetPlayerId();
                    playerAttack.Coordinate.x = hit.point.x;
                    playerAttack.Coordinate.y = hit.point.y;
                    playerAttack.Coordinate.z = hit.point.z;

                    
                
                    PlayerAttackDataPacket packet = new PlayerAttackDataPacket(playerAttack);
                    _networkDivider._network.SendReliable(packet);

                }
            }
        }
        else
        {
            
        }
        Move();
        
    }

    private void SetKnifeDirection(Vector3 destination)
    {
        var dir = destination - transform.position;
        direction.transform.forward = dir;
    }
    public void ThrowKnife(Vector3 dest)
    {
        SetKnifeDirection(dest);
        m_animator.SetTrigger("Attack");
        Instantiate(knife, transform.position, direction.transform.rotation);
    }

    public void SetDestination(Vector3 dest)
    {
        destination = dest;
        isMove = true;
    }

    private void Move()
    {
       
        if (isMove)
        {
            if (Vector3.Distance(destination, transform.position) <= 0.1f)
            {
                isMove = false;
                m_animator.SetFloat("MoveSpeed", 0);
                return;
            }
            m_animator.SetFloat("MoveSpeed", move_speed);

            var dir = destination - transform.position;
            transform.forward = dir;
            transform.position += dir.normalized * Time.deltaTime * move_speed;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Equals("Weapon"))
        {
            Destroy(other.gameObject);

            if (!isRemote)
            {
                HP -= 10;
                
                PlayerStat playerStat = new PlayerStat();
                
                playerStat.PlayerId = PlayerInfo.GetInstance().GetPlayerId();
                playerStat.HP = HP;
                PlayerStatDataPacket packet = new PlayerStatDataPacket(playerStat);
                _networkDivider._network.SendReliable(packet);


                MyHp.text = HP.ToString();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class movemanager : MonoBehaviour
{
    public float speed = 5;
    public Rigidbody rb;
    bool jump = false;
    public GameObject pbullet;
    Stopwatch reattack = new Stopwatch();
    Stopwatch prevent2 = new Stopwatch();
    public static bool isleft;
    


    // Start is called before the first frame update
    void Start()
    {
        isleft = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            isleft = true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            isleft = false;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left*speed*Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!jump)
            {
                jump = true;
                rb.AddForce(Vector3.up * 7, ForceMode.Impulse);
            }
            else
                return;
        }

       

        if (Input.GetKey(KeyCode.Space))
        {
            if (!reattack.IsRunning)
            {
                reattack.Start();
                doattack();
            }


                if (reattack.ElapsedMilliseconds >= 500)
                {
                    reattack.Restart();
                    doattack();
                }

        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("block"))
        {
            jump = false;
            transform.eulerAngles = new Vector3(transform.rotation.x,transform.rotation.y,0);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }


    private void OnBecameInvisible()
    {
        transform.position = new Vector3(0, 1, 0);
        transform.eulerAngles = new Vector3(0, 0, 0);
    }
    void doattack()
    {
        if (isleft)
        {
            Instantiate(pbullet, new Vector3(transform.position.x - 1, transform.position.y, -0.5f), transform.rotation);

        }
        else
        {
            Instantiate(pbullet, new Vector3(transform.position.x + 1, transform.position.y, -0.5f), transform.rotation);
        }
    }

}

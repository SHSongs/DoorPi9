using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeManager : MonoBehaviour
{
    private float knife_x = 0.0f;
    public float knife_speed = 5f;

    private SphereCollider _collider;
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<SphereCollider>();
        StartCoroutine (RunCoroutine ());
    }
    IEnumerator RunCoroutine() {
        yield return new WaitForSeconds(0.2f);

        _collider.enabled = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        knife_x += 15;
        transform.localRotation = Quaternion.Euler(knife_x, 0, 0);
        var parent = transform.parent;
       
        
        parent.position += parent.forward.normalized * (Time.deltaTime * knife_speed);
        
        
    }
}

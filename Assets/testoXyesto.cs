using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testoXyesto : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var hits = Physics.SphereCastAll(transform.position, 10, Vector3.up);
        Debug.Log(hits.Length);
    }
}

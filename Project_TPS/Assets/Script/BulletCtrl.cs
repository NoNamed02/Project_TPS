using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    private Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.AddForce(new Vector3(1f, 0f, 0f), ForceMode.Impulse);
        _rigidbody.useGravity = false;
    }
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody;
using UnityEngine;

public class ForceTest : MonoBehaviour
{
    public Rigidbody Test;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            AddForce();
    }
    public void AddForce()
    { 
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        //Rigidbody playerRd = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        //playerRd.AddForce(transform.forward * 10f, ForceMode.Impulse);

        Test.AddForce(transform.forward * 10f, ForceMode.Impulse);

        // 힘을 추가
        rigidbody.AddForce(transform.forward * 10f, ForceMode.Impulse);
    }
}

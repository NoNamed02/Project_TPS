using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : MonoBehaviour
{
    public float HP = 10;
    public GameObject VFX;

    // Update is called once per frame
    void Update()
    {
        DIE();
    }

    public void DIE()
    {
        if (HP <= 0)
        {
            Instantiate(VFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

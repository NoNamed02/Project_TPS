using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : NormalEnemy
{
    void Start()
    {
        HP = 20;
    }

    // 부모의 Update 메서드를 재정의
    void Update()
    {
        DIEBoss();
    }

    public void DIEBoss()
    {
        if (HP <= 0)
        {
            GAMEMANAGER.Instance.getBlueCard = true;
            Debug.Log("보스 죽음");
            Instantiate(VFX, transform.position, Quaternion.identity);
            //Destroy(gameObject);
        }
    }
}

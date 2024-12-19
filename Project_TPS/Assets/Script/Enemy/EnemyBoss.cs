using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : NormalEnemy
{
    public PlayerMovement player;
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
            Instantiate(VFX, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
            Instantiate(VFX, transform.position + new Vector3(Random.Range(-5, 5), 3, Random.Range(-5, 5)), Quaternion.identity);
            //Instantiate(VFX, transform.position + new Vector3(Random.Range(-5, 5), 2, Random.Range(-5, 5)), Quaternion.identity);
            //Instantiate(VFX, transform.position + new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5)), Quaternion.identity);
            //Instantiate(VFX, transform.position + new Vector3(Random.Range(-5, 5), 3, Random.Range(-5, 5)), Quaternion.identity);
            player.PlayNormalBGM();
            Destroy(gameObject);
        }
    }
}

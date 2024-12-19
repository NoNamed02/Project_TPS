using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

public class DummyBoss : MonoBehaviour
{
    public GameObject Boss;
    public float spwanTime = 3f;
    void Start()
    {
        StartCoroutine(BossSpwan(spwanTime));
    }
    IEnumerator BossSpwan(float spwanTime)
    {
        yield return new WaitForSeconds(spwanTime);
        Boss.SetActive(true);
        StartDialogue Player = GameObject.FindWithTag("Player").GetComponent<StartDialogue>();
        Player.TalkEnd();
        Destroy(gameObject);
    }
}

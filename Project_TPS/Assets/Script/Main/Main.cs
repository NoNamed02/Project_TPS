using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

public class Main : MonoBehaviour
{
    public GameObject Loading;

    private void Awake() {
        MasterAudio.PlaySound("Menu_BGM");
    }
    public void StartButton()
    {
        Loading.SetActive(true);
    }
}

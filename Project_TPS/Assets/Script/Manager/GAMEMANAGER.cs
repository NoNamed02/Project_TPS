using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DarkTonic.MasterAudio;

public class GAMEMANAGER : MonoBehaviour
{
    public static GAMEMANAGER Instance = null;
    public bool getBlueCard = false;
    void Awake()
    {
        
        MasterAudio.PlaySound3DAtTransform("BGMAlone_320", transform);
        Init();
    }
    private void Init()
    {
        StartCoroutine(CursorOff());
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator CursorOff()
    {
        yield return new WaitForSeconds(0.2f);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


        if (Cursor.lockState != CursorLockMode.Locked || Cursor.visible != false)
        {
            StartCoroutine(CursorOff());
        }
    }

    public void PlaySound(string BGMName, Transform transform)
    {
        MasterAudio.PlaySound3DAtTransform(BGMName, transform);
    }
    public void StopBGM()
    {
        //MasterAudio.StopAllOfSound()
    }
}

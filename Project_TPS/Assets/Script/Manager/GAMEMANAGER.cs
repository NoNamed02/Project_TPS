using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAMEMANAGER : MonoBehaviour
{
    void Awake()
    {
        Init();
    }
    private void Init()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}

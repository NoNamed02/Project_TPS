using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCameraPos : MonoBehaviour
{
    public Transform endPos;
    public Transform Player;
    void Update()
    {
        Camera.main.transform.position = endPos.position;
        Camera.main.transform.LookAt(Player.position);
    }
}

using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tactical;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private RectTransform rectTransform;
    public TextMeshProUGUI bulletLeft;

    void Start()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        AimRotation();
        BulletCount();
    }

    private void BulletCount()
    {
        bulletLeft.text = playerMovement.leftBulletsForCount.ToString() + " / 30" ;
    }

    private void AimRotation()
    {
        Vector3 targetAngle = playerMovement.isAiming ? new Vector3(0f, 20f, 0f) : new Vector3(0f, 0f, 0f);

        // 로컬 좌표계를 기준으로 회전
        Vector3 currentLocalAngle = rectTransform.localEulerAngles;
        Vector3 newLocalAngle = Vector3.Lerp(currentLocalAngle, targetAngle, Time.deltaTime * 10f);

        rectTransform.localRotation = Quaternion.Euler(newLocalAngle);
    }

}

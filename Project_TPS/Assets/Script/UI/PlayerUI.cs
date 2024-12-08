using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tactical;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Image를 사용하기 위해 필요

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement playerMovement; // 플레이어 스크립트 참조
    private RectTransform rectTransform; // 에임 회전용 RectTransform
    public TextMeshProUGUI bulletLeft; // 탄약 UI
    public Image hpBar; // HP를 표현할 Image UI

    void Start()
    {
        // PlayerMovement 초기화
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject with tag 'Player' not found!");
            return;
        }

        playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on Player GameObject!");
            return;
        }

        // RectTransform 초기화
        rectTransform = gameObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform component not found on this GameObject!");
        }

        // HP Bar 초기화 확인
        if (hpBar == null)
        {
            Debug.LogError("HP Bar Image is not assigned!");
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.H)) // 테스트로 HP 감소
        {
            playerMovement.HP -= 10;
            Debug.Log("HP: " + playerMovement.HP); // 디버그 출력
        }
        if (playerMovement == null || rectTransform == null) return;

        AimRotation();
        BulletCount();
        UpdateHPBar(); // HP Bar 업데이트
    }

    private void BulletCount()
    {
        if (bulletLeft != null && playerMovement != null)
        {
            bulletLeft.text = playerMovement.leftBulletsForCount.ToString() + " / 30";
        }
    }

    private void AimRotation()
    {
        if (playerMovement == null || rectTransform == null) return;

        Vector3 targetAngle = playerMovement.isAiming ? new Vector3(0f, 20f, 0f) : new Vector3(0f, 0f, 0f);

        Vector3 currentLocalAngle = rectTransform.localEulerAngles;
        Vector3 newLocalAngle = Vector3.Lerp(currentLocalAngle, targetAngle, Time.deltaTime * 10f);

        rectTransform.localRotation = Quaternion.Euler(newLocalAngle);
    }

    private void UpdateHPBar()
    {
        if (hpBar != null && playerMovement != null)
        {
            // PlayerMovement의 HP를 기반으로 HP 바 업데이트
            float normalizedHP = Mathf.Clamp01(playerMovement.HP / 100f); // HP를 0~1로 정규화
            hpBar.fillAmount = normalizedHP; // Image의 fillAmount를 설정
        }
    }
}

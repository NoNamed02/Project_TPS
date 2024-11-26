using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 _cameraVelocity = Vector3.zero;

    public float verticalRotation = 0f;
    private float _verticalRotationReset;
    public float verticalRotationLimit = 30f;
    public float verticalSensitivity = 1f;

    public void MoveCamera(PlayerMovement PlayerScript, Transform AimTarget)
    {

        Vector3 targetPosition = PlayerScript.isAiming ? AimTarget.position : PlayerScript.gameObject.transform.position;

        Quaternion cameraRotation = Quaternion.Euler(0f, PlayerScript.gameObject.transform.eulerAngles.y, 0f);
        Vector3 desiredPosition = targetPosition + cameraRotation * PlayerScript.cameraOffset;

        // 카메라의 목표 위치 계산
        Vector3 finalPosition;

        if (PlayerScript.isAiming)
        {
            float mouseY = PlayerScript.mouseY * verticalSensitivity;

            // 상하 회전 값 갱신 (마우스 입력에 따라 증가/감소)
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);
            //finalPosition = desiredPosition;
            //finalPosition = Vector3.SmoothDamp(Camera.main.transform.position, desiredPosition, ref _cameraVelocity, 0.01f);
            finalPosition = Vector3.Lerp(Camera.main.transform.position, desiredPosition, 1f);
        }
        else
        {
            verticalRotation = 0f;
            finalPosition = Vector3.SmoothDamp(Camera.main.transform.position, desiredPosition, ref _cameraVelocity, 0.1f);
        }

        Vector3 localPosition = PlayerScript.gameObject.transform.InverseTransformPoint(finalPosition);

        localPosition.z = -3f;

        localPosition.x = Mathf.Clamp(localPosition.x, -1.5f, 1.5f);

        finalPosition = PlayerScript.gameObject.transform.TransformPoint(localPosition);

        // 카메라 위치 설정
        Camera.main.transform.position = finalPosition;

        // 카메라 상하 회전 적용
        Vector3 lookAtTarget = targetPosition + new Vector3(0f, 1.5f, 0f);
        Camera.main.transform.LookAt(lookAtTarget);
        Camera.main.transform.RotateAround(lookAtTarget, Camera.main.transform.right, verticalRotation);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 _cameraVelocity = Vector3.zero;
    public void MoveCamera(PlayerMovement PlayerScript, Transform AimTarget)
    {
        Vector3 targetPosition = PlayerScript.isAiming ? AimTarget.position : PlayerScript.gameObject.transform.position;

        Quaternion cameraRotation = Quaternion.Euler(0f, PlayerScript.gameObject.transform.eulerAngles.y, 0f);
        Vector3 desiredPosition = targetPosition + cameraRotation * PlayerScript.cameraOffset;

        // 카메라의 목표 위치 계산
        Vector3 finalPosition;
        if (!PlayerScript.isAiming)
        {
            finalPosition = Vector3.SmoothDamp(Camera.main.transform.position, desiredPosition, ref _cameraVelocity, 0.1f);
        }
        else
        {
            finalPosition = desiredPosition;
        }

        Vector3 localPosition = PlayerScript.gameObject.transform.InverseTransformPoint(finalPosition);

        localPosition.z = -3f;

        localPosition.x = Mathf.Clamp(localPosition.x, -1.5f, 1.5f);

        finalPosition = PlayerScript.gameObject.transform.TransformPoint(localPosition);

        Camera.main.transform.position = finalPosition;
        //Debug.Log("pos = " + PlayerScript.gameObject.transform.InverseTransformPoint(Camera.main.transform.position));

        Camera.main.transform.LookAt(targetPosition + new Vector3(0f, 1.5f, 0f));
    }
}

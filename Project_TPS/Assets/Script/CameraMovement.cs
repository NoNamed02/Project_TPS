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

        if (!PlayerScript.isAiming)
        {
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, desiredPosition, ref _cameraVelocity, 0.3f);
        }
        else
        {
            Camera.main.transform.position = desiredPosition;
        }

        Camera.main.transform.LookAt(targetPosition + new Vector3(0f, 1.5f, 0f));
    }
}

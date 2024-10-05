using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private float _horizontal; 
    private float _vertical; 
    private float _mouseX; 

    private Animator _animator;

    private Vector3 _cameraVelocity = Vector3.zero;
    public Transform cameraTransform;

    public float moveSpeed = 10f;
    public float viewSpeed = 100f;
    private Vector3 _cameraOffset;
    private bool _isAiming;

    public GameObject aimIndicator;
    public Transform aimTarget;

    void Start()
    {
        _mouseX = 0f;
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        MovePlayer();
        UpdateAnimation();
        MoveCamera();
    }

    private void HandleInput()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        if (_vertical < 0)
        {
            _vertical *= 0.3f;
            _horizontal *= 0.3f;
        }

        if (_isAiming)
        {
            _vertical *= 0.5f;
            _horizontal *= 0.5f;
        }

        _mouseX = Input.GetAxis("Mouse X") * viewSpeed * Time.deltaTime;

        if (Input.GetMouseButton(1))
        {
            _cameraOffset = new Vector3(0f, 1.5f, -2f);
            _isAiming = true;
            aimIndicator.SetActive(true);
        }
        else
        {
            _cameraOffset = new Vector3(0f, 2f, -3f);
            _isAiming = false;
            aimIndicator.SetActive(false);
        }
    }

    private void MovePlayer()
    {
        float mappedX = _horizontal * Mathf.Sqrt(1 - (_vertical * _vertical) / 2);
        float mappedY = _vertical * Mathf.Sqrt(1 - (_horizontal * _horizontal) / 2);

        Vector3 movement = new Vector3(mappedX, 0, mappedY) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        RotatePlayer();
    }

    private void RotatePlayer()
    {
        transform.Rotate(0, _mouseX * viewSpeed, 0);
    }

    private void UpdateAnimation()
    {
        float movementMagnitude = new Vector3(_horizontal, 0, _vertical).magnitude;
        _animator.SetFloat("Speed", movementMagnitude > 0.5f ? 1f : 0f);
        _animator.SetFloat("X", _horizontal);
        _animator.SetFloat("Y", _vertical);
    }

    private void MoveCamera()
    {
        Vector3 targetPosition = _isAiming ? aimTarget.position : transform.position;

        Quaternion cameraRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        Vector3 desiredPosition = targetPosition + cameraRotation * _cameraOffset;

        if (!_isAiming)
        {
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, desiredPosition, ref _cameraVelocity, 0.3f);
        }
        else
        {
            Camera.main.transform.position = desiredPosition;
        }

        cameraTransform.LookAt(targetPosition + new Vector3(0f, 1.5f, 0f));
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float _horizontal; 
    private float _vertical; 
    private float _mouseX; 

    private Animator _animator;

    public float moveSpeed = 10f;
    public float viewSpeed = 100f;
    public Vector3 cameraOffset;
    public bool isAiming;

    private GameObject _aimIndicator;
    public Transform aimTarget;

    private CameraMovement _cameraMovement;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _cameraMovement = Camera.main.GetComponent<CameraMovement>();
        _aimIndicator = GameObject.Find("CrossHair");
    }

    void Update()
    {
        HandleInput();
        MovePlayer();
        UpdateAnimation();
        Aiming();
        _cameraMovement.MoveCamera(this, aimTarget);
    }

    private void HandleInput()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
        _mouseX = Input.GetAxis("Mouse X") * viewSpeed * Time.deltaTime;
        ControllSpeed();
    }
    private void Aiming()
    {
        isAiming = Input.GetMouseButton(1);
        cameraOffset = isAiming ? new Vector3(0f, 1.5f, -2f) : new Vector3(0f, 2f, -3f);
        _aimIndicator.SetActive(isAiming);
    }
    private void ControllSpeed()
    {
        if (_vertical < 0)
        {
            _vertical *= 0.3f;
            _horizontal *= 0.3f;
        }
        if (isAiming)
        {
            _vertical *= 0.5f;
            _horizontal *= 0.5f;
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
}

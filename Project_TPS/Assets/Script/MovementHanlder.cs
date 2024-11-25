/*
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class movementHandler
{
    public float _horizontal; 
    public float _vertical; 
    public float _mouseX;
    public bool _mouseLeft;
    public bool _canMove = true;
    public float viewSpeed = 50f;
    private void HandleInput()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
        _mouseX = Input.GetAxis("Mouse X") * viewSpeed * Time.deltaTime;
        _mouseLeft = Input.GetMouseButton(0);
        ControllSpeed();
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
}
*/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _horizontal; 
    [SerializeField]
    private float _vertical; 
    private float _mouseX;
    [SerializeField]
    private bool _canMove = true;

    private Animator _animator;

    public float moveSpeed = 10f;
    public float viewSpeed = 50f;
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
        Cursor.visible = false;
    }

    void Update()
    {
        HandleInput();
        MovePlayer();
        UpdateAnimation();
        Aiming();
        _cameraMovement.MoveCamera(this, aimTarget);

        
    }

    private float _lastSpaceTime;
    private float _doubleClickThreshold = 0.3f;
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
        Vector3 direction = new Vector3(_horizontal, 0, _vertical);
        Vector3 forward = Vector3.Slerp(transform.forward, direction, viewSpeed * Time.deltaTime / Vector3.Angle(transform.forward, direction));
        if (_canMove)
        {
            float mappedX = _horizontal * Mathf.Sqrt(1 - (_vertical * _vertical) / 2);
            float mappedY = _vertical * Mathf.Sqrt(1 - (_horizontal * _horizontal) / 2);

            Vector3 movement = new Vector3(mappedX, 0, mappedY) * moveSpeed * Time.deltaTime;

            /*
            if (_vertical > 0.01f && _horizontal > 0.01f) // WD
            {
                Debug.Log("check");
                movement = new Vector3(mappedX * _horizontal * 2, 0, mappedY) * moveSpeed * Time.deltaTime;
            }
            else if (_vertical > 0.01f && _horizontal < -0.01f)
            {
                Debug.Log("check2");
                movement = new Vector3(mappedX, 0, mappedY) * moveSpeed * Time.deltaTime;
            }
            else
            {
                movement = new Vector3(mappedX, 0, mappedY) * moveSpeed * Time.deltaTime;
            }
            */
            movement = new Vector3(mappedX, 0, mappedY) * moveSpeed * Time.deltaTime;
            transform.Translate(movement);

            RotatePlayer();
        }
    }

    private void RotatePlayer()
    {
        transform.Rotate(0, _mouseX * viewSpeed * Time.deltaTime, 0);
        if (_vertical > 0.01f && _horizontal != 0) // WD
        {
            Debug.Log("check");
            transform.rotation = transform.rotation * Quaternion.Euler(0, _horizontal * 0.5f, 0);
        }
    }

    private void UpdateAnimation()
    {
        float movementMagnitude = new Vector3(_horizontal, 0, _vertical).magnitude;
        _animator.SetFloat("Speed", movementMagnitude > 0.5f ? 1f : 0f);
        _animator.SetFloat("X", _horizontal);
        _animator.SetFloat("Y", _vertical);
    }

    private void OnCollisionStay(Collision other) {
        if (other.gameObject.tag == "wall")
        {
            Debug.Log("Hit wall");
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //transform.rotation = other.transform.rotation;
                StartCoroutine(WallJump());
            }
        }
    }

    IEnumerator WallJump()
    {
        _canMove = false;
        _animator.Play("WallJump");

        float jumpDuration = 0.5f;
        float jumpDistance = 1.5f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + transform.forward * jumpDistance;

        float elapsedTime = 0f;
        while (elapsedTime < jumpDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / jumpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;

        yield return new WaitForSeconds(0.3f);
        _canMove = true;
    }

    IEnumerator Slide()
    {
        _canMove = false;
        _animator.Play("SlIDE00");

        float slideDuration = 1f;  // 슬라이딩 시간
        float slideDistance = 3f;  // 슬라이딩 거리
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + transform.forward * slideDistance;

        float elapsedTime = 0f;
        while (elapsedTime < slideDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        _canMove = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    private float _horizontal; 
    private float _vertical; 
    private float _mouseX;
    private bool _canMove = true;

    private Animator _animator;

    public float moveSpeed = 10f;
    public float viewSpeed = 50f;
    public Vector3 cameraOffset;
    public bool isAiming = false;
    public bool isReload = false;
    private int _leftBullet = 30;

    private GameObject _aimIndicator;
    public Transform aimTarget;
    public GameObject gunMuzzle;
    private CameraMovement _cameraMovement;

    private float fireRate = 1f / 3f;
    private float lastFireTime = 0f;

    public int HP = 100;
    void Start()
    {
        _animator = GetComponent<Animator>();
        _cameraMovement = Camera.main.GetComponent<CameraMovement>();
        _aimIndicator = GameObject.Find("CrossHair");
        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {
        HandleInput();
        MovePlayer();
        UpdateAnimation();
        Aiming();
        ReloadSystem();
        _cameraMovement.MoveCamera(this, aimTarget);
    }
    private void ReloadSystem()
    {
        _animator.SetBool("IsReload", isReload);
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReLoadStart());
            Debug.Log("left bullet : " + _leftBullet);
        }
    }
    private IEnumerator ReLoadStart()
    {
        isReload = true;
        yield return new WaitForSeconds (2f);
        Debug.Log("Reloaded");
        _leftBullet = 30;
        isReload = false;
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
        _animator.SetBool("IsAiming", isAiming);
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = !isAiming;
        }
        //isAiming = Input.GetMouseButton(1);
        _aimIndicator.SetActive(isAiming);
        if (isAiming)
        {
            cameraOffset = new Vector3(0f, 1.5f, -2f);
            Shoot();
        }
        else
        {
            cameraOffset = new Vector3(0f, 2f, -3f);
        }
    }
    private void Shoot()
    {
        LayerMask layerMask = ~LayerMask.GetMask("Player");
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray cameraRay = Camera.main.ScreenPointToRay(screenCenter);
        Vector3 targetPoint;
        float maxDistance = 100f;

        if (Physics.Raycast(cameraRay, out RaycastHit cameraHitInfo, maxDistance, layerMask))
        {
            targetPoint = cameraHitInfo.point;
        }
        else
        {
            targetPoint = cameraRay.GetPoint(maxDistance);
        }

        // 총구에서 조준점으로 방향 계산
        Vector3 muzzlePosition = gunMuzzle.transform.position;
        Vector3 directionToTarget = (targetPoint - muzzlePosition).normalized;
        // 총구 근처 Ray 충돌 검사
        if (Physics.Raycast(muzzlePosition, directionToTarget, out RaycastHit muzzleHitInfo, maxDistance))
        {
            targetPoint = muzzleHitInfo.point;
            directionToTarget = (targetPoint - muzzlePosition).normalized;
        }
        
        if (Input.GetMouseButton(0) && Time.time >= lastFireTime + fireRate && _leftBullet > 0 && !isReload)
        {
            lastFireTime = Time.time;
            --_leftBullet;
            if (Physics.Raycast(muzzlePosition, directionToTarget, out RaycastHit enemy, maxDistance))
            {
                if (enemy.collider.CompareTag("Enemy"))
                {
                    Debug.Log("적 맞춤");
                    enemy.collider.GetComponent<Enemy_3D>().HP -= 1;
                    Debug.Log("enemy HP = " + enemy.collider.GetComponent<Enemy_3D>().HP);
                }
            }
        }
        else if (Input.GetMouseButton(0) && _leftBullet <= 0)
        {
            Debug.Log("No left bullet");
        }


        // 확인용 Ray
        if (Physics.Raycast(muzzlePosition, directionToTarget, out RaycastHit Hit, maxDistance))
        {
            // 충돌이 발생했을 경우: 충돌 지점까지만 그리기
            Debug.DrawRay(muzzlePosition, (Hit.point - muzzlePosition), Color.red, 1.5f);
        }
        else
        {
            // 충돌이 발생하지 않았을 경우: 최대 거리까지 그리기
            Debug.DrawRay(muzzlePosition, directionToTarget * maxDistance, Color.red, 1.5f);
        }
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

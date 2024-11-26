using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Keyboard input
    private float _horizontal; 
    private float _vertical; 
    private float _mouseX;
    public float mouseY;
    private bool _mouseLeft;
    private bool _mouseRight;
    private bool _KeySpace;
    private bool _keyR;
    
    
    // animator
    private Animator _animator;

    // movement system
    public float moveSpeed = 10f;
    public float viewSpeed = 50f;
    public Vector3 cameraOffset;
    public bool isAiming = false;
    public bool isReload = false;
    private int _leftBullets = 30;
    private bool _canMove = true;
    [SerializeField]
    private bool _isCover = false;

    // combet system
    private GameObject _aimIndicator;
    public Transform aimTarget;
    public GameObject gunMuzzle;
    private CameraMovement _cameraMovement;
    private float _fireRate = 1f / 10f;
    private float _lastFireTime = 0f;
    public int HP = 100;
            // combet VFX
            public GameObject shootVFX;
            private ObjectPool _vfxPool;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _cameraMovement = Camera.main.GetComponent<CameraMovement>();
        _aimIndicator = GameObject.Find("CrossHair");
        _vfxPool = FindObjectOfType<ObjectPool>();
        Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {
        HandleInput();
        MovePlayer();
        UpdateAnimation();
        Aiming();
        ReloadSystem();
        TakeCover();
        _cameraMovement.MoveCamera(this, aimTarget);
    }
    private void TakeCover()
    {
        if (_KeySpace && _canMove)
        {
            Vector3 PlayerPos = transform.position + new Vector3 (0f, 1f, 0f);
            Ray coverRay = new Ray(PlayerPos, transform.forward);
            LayerMask layerMaskPlayer = ~LayerMask.GetMask("Player");
            float _coverRange = 1f;
            if (Physics.Raycast(coverRay, out RaycastHit HitTarget, _coverRange, layerMaskPlayer))
            {
                if (HitTarget.collider.CompareTag("Cover"))
                {
                    if (isAiming) isAiming = false;
                    Debug.Log("엄폐 가능");
                    StartCoroutine(TakeCoverStart(HitTarget.collider.gameObject.transform));
                }
            }
        }
        if (_isCover && (isAiming || (Math.Abs(_horizontal) >= 0.1f) || Math.Abs(_vertical) >= 0.1f))
            _isCover = false;
        Debug.DrawRay(transform.position + new Vector3 (0f, 1f, 0f), transform.forward * 1f, Color.blue, 0.5f);
    }
    private IEnumerator TakeCoverStart(Transform cover)
    {
        _isCover = true;
        _canMove = false;
        Vector3 startPos = transform.position;
        Vector3 coverPos = cover.TransformPoint(new Vector3(0f, 0f, -0.9f));
        coverPos.y = transform.position.y;
        // 이동 시간 설정
        float duration = 0.3f; // 이동에 걸리는 시간 (초)
        float elapsedTime = 0f;
        transform.rotation = cover.rotation;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, coverPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = coverPos;

        yield return new WaitForSeconds(0.5f);
        Debug.Log("Cover End");
        _canMove = true;
    }
    private void ReloadSystem()
    {
        if (_keyR && !isReload)
        {
            StartCoroutine(ReLoadStart());
            Debug.Log("left bullet : " + _leftBullets);
        }
    }
    private IEnumerator ReLoadStart()
    {
        isReload = true;
        yield return new WaitForSeconds (2f);
        Debug.Log("Reloaded");
        _leftBullets = 30;
        isReload = false;
    }
    private void HandleInput()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
        _mouseX = Input.GetAxis("Mouse X") * viewSpeed * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y");
        _mouseLeft = Input.GetMouseButton(0);
        _mouseRight = Input.GetMouseButtonDown(1);
        _KeySpace = Input.GetKeyDown(KeyCode.Space);
        _keyR = Input.GetKeyDown(KeyCode.R);
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
    private void Aiming()
    {
        if (_mouseRight)
        {
            isAiming = !isAiming;
        }
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
        
        if (_mouseLeft && Time.time >= _lastFireTime + _fireRate && _leftBullets > 0 && !isReload)
        {
            _lastFireTime = Time.time;
            --_leftBullets;
            MakeGunRecoil();
            if (Physics.Raycast(muzzlePosition, directionToTarget, out RaycastHit enemy, maxDistance))
            {
                if (enemy.collider.CompareTag("Enemy"))
                {
                    Debug.Log("적 맞춤");
                    enemy.collider.GetComponent<Enemy_3D>().HP -= 1;
                    Debug.Log("enemy HP = " + enemy.collider.GetComponent<Enemy_3D>().HP);
                }
            }
            RayTest(muzzlePosition, directionToTarget, maxDistance);
            UsingVFX(shootVFX, targetPoint, 0.7f);
        }
        else if (_mouseLeft && _leftBullets <= 0)
        {
            Debug.Log("No left bullet");
            RayTest(muzzlePosition, directionToTarget, maxDistance);
        }
    }

    private void UsingVFX(GameObject VFX, Vector3 instancePoint, float desTime)
    {
        //GameObject vfx = Instantiate(VFX, instnacePoint, Quaternion.identity);
        //Destroy(vfx, desTime);

        _vfxPool.GetObject(instancePoint, Quaternion.identity, desTime);
    }

    private void RayTest(Vector3 muzzlePosition, Vector3 directionToTarget, float maxDistance) // ray 확인용임 굳이굳이 ???
    {
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

    private void MakeGunRecoil()
    {
        _cameraMovement.verticalRotation -= 1.5f;
        float RandomHorizontalValue = UnityEngine.Random.Range(-1f, 1f);
        transform.rotation = transform.rotation * Quaternion.Euler(0f, RandomHorizontalValue, 0f);
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
        if (_canMove && !_isCover)
        {
            transform.Rotate(0, _mouseX * viewSpeed * Time.deltaTime, 0);
            if (_vertical > 0.01f && _horizontal != 0) // WD
            {
                //Debug.Log("check");
                transform.rotation = transform.rotation * Quaternion.Euler(0, _horizontal * 0.5f, 0);
            }
        }
    }

    private void UpdateAnimation()
    {
        float movementMagnitude = new Vector3(_horizontal, 0, _vertical).magnitude; // 이거 뭐임??
        if (_canMove)
        {
            //_isCover = false;
            _animator.SetFloat("Speed", movementMagnitude > 0.5f ? 1f : 0f);
            _animator.SetFloat("X", _horizontal);
            _animator.SetFloat("Y", _vertical);
            _animator.SetBool("IsAiming", isAiming);
            _animator.SetBool("IsReload", isReload);
        }
        _animator.SetBool("IsCover", _isCover);
    }

    private void OnCollisionStay(Collision other) {
        if (other.gameObject.tag == "wall")
        {
            Debug.Log("Hit wall");
            if (_KeySpace)
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
}

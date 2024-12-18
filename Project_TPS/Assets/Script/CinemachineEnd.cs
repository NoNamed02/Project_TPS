using UnityEngine;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;

public class CinemachineEnd : MonoBehaviour
{
    [SerializeField]
    private CinemachineDollyCart cart; // Cinemachine Dolly Cart
    [SerializeField]
    private CinemachineBrain brain; // Main Camera의 Cinemachine Brain
    [SerializeField]
    private GameObject fadePrefab; // 페이드 프리팹

    public GameObject [] Player = new GameObject[3]; // 0 더미 // 1 플레이어 // 2 조준선
    public GameObject CinemachineSelf;
    public List<GameObject> Enemys;

    private bool _isend = false;

    public int wantPos = 3;
    void Awake()
    {
        if (Player[0] != null)
            Player[0].SetActive(true);
        if (Player[1] != null)
            Player[1].SetActive(false);
        if (Player[2] != null)
            Player[2].SetActive(false);
    }

    void Start()
    {
        if (brain != null)
        {
            brain.enabled = true;
        }
        // Dolly Cart 가져오기
        cart = GameObject.Find("Dolly Cart").GetComponent<CinemachineDollyCart>();

        // Cinemachine Brain 가져오기
        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    void Update()
    {
        // Dolly Cart가 특정 위치에 도달하면 페이드 효과 시작
        if (cart.m_Position >= wantPos && !_isend) // 원하는 조건 설정
        {
            StartCoroutine(CameraOff());
            _isend = true;
        }
    }

    private IEnumerator CameraOff()
    {
        // 페이드 프리팹 Instantiate
        GameObject fadeinout = Instantiate(fadePrefab, GameObject.Find("Canvas").transform);

        // 페이드 인 시작
        BlackFadeInOut fadeScript = fadeinout.GetComponent<BlackFadeInOut>();
        if (fadeScript != null)
        {
            fadeScript.StartFadeOut(); // 페이드 인 실행
            Debug.Log("A");
        }

        yield return new WaitForSeconds(3f);
        fadeScript.StartFadeIn();
        Debug.Log("B");
        
        //Player[0].SetActive(false);
        Player[1].SetActive(true);
        //Player[2].SetActive(true);
        
        if (Player[0] != null)
            Player[0].SetActive(false);
        if (Player[1] != null)
            Player[1].SetActive(true);
        if (Player[2] != null)
            Player[2].SetActive(true);

        foreach (var enemy in Enemys)
        {
            enemy.SetActive(true);
        }

        brain.enabled = false;
        CinemachineSelf.SetActive(false);
    }
}

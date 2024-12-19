using UnityEngine;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;

public class CinemachineEnd_2rd : MonoBehaviour
{
    [SerializeField]
    private CinemachineDollyCart cart; // Cinemachine Dolly Cart
    [SerializeField]
    private CinemachineBrain brain; // Main Camera의 Cinemachine Brain
    [SerializeField]
    private GameObject fadePrefab; // 페이드 프리팹
    public GameObject CinemachineSelf;
    private bool _isend = false;

    public GameObject Canvas;

    public int wantPos = 1;
    void Awake()
    {
        MasterAudio.PlaySound("end_BGM");
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
        if (cart.m_Position >= wantPos && !_isend) // 원하는 조건 설정
        {
            StartCoroutine(CameraOff());
            _isend = true;
        }
    }

    private IEnumerator CameraOff()
    {
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
        
        brain.enabled = false;
        Canvas.SetActive(true);
        CinemachineSelf.SetActive(false);
    }
}

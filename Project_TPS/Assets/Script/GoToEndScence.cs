using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToEndScence : MonoBehaviour
{
    public GameObject _loading;
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player") && GAMEMANAGER.Instance.getBlueCard)
        {
            if (_loading != null)
            {
                _loading.SetActive(true);
            }
            else
            {
                Debug.LogWarning("로딩 오브젝트 없다요");
            }
        }
    }
}

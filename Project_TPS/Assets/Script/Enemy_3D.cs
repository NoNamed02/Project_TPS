using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Enemy_3D : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Transform _target = null;
    private float _distanceToPlayer;
    private bool _attack = false;
    private Animator _animator;
    public int HP = 30;
    void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();
        if (_target != null)
        {
            if (_target.CompareTag("Player") && _distanceToPlayer < 5f)
            {
                _attack = true;
                _animator.SetBool("attack", _attack);
                StartCoroutine(Attack());
            }
            else if (!_attack) _agent.SetDestination(_target.position);
        }

        if (HP <= 0) Destroy(gameObject);
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(3f);
        _attack = false;
        _animator.SetBool("attack", _attack);
    }

    private void FindTarget()
    {
        GameObject[] taggedMonsters = GameObject.FindGameObjectsWithTag(gameObject.tag);
        GameObject player = GameObject.Find("Player");
        float closedistance = Mathf.Infinity;
        
        foreach (GameObject obj in taggedMonsters)
        {
            if (obj == gameObject) continue;

            float distanceToTag = Vector3.Distance(transform.position, obj.transform.position);
            if (closedistance > distanceToTag)
            {
                closedistance = distanceToTag;
                _target = obj.transform;
            }
        }
        
        /*
        _target = taggedMonsters
        .Where(obj => obj != gameObject) // 현재 게임 오브젝트 제외
        .OrderByDescending(obj => Vector3.Distance(transform.position, obj.transform.position)) // 거리가 먼 순으로 정렬
        .Select(obj => obj.transform) // Transform만 선택
        .FirstOrDefault(); // 가장 먼 Transform 선택
        */

        _distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (_distanceToPlayer < closedistance)
        {
            _target = player.transform;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(gameObject.tag))
        {
            if(other.gameObject.GetInstanceID() < gameObject.GetInstanceID())
            {
                // 낮은쪽을 파괴
                transform.localScale += new Vector3(0.3f, 0.3f, 0.3f);
                HP = 5;
                Destroy(other.gameObject);
            }
            else
            {
                other.transform.localScale += new Vector3(0.3f, 0.3f, 0.3f);
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerMovement>().HP-=5;
        }
    }
}

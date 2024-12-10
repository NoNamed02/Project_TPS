//using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;


namespace BehaviorDesigner.Runtime.Tasks.Test
{
    public abstract class Action : Task{}
    public abstract class Conditional : Task{}
}

namespace Core.AI
{
    public class EnemyAction : Action
    {
        protected Rigidbody rigidbody;
        protected Animator animator;
        protected PlayerMovement player;
        protected NavMeshAgent navMeshAgent;
        public override void OnAwake()
        {
            rigidbody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
            //player = null;
        }
    }

    public class EnemyConditional : Conditional
    {
        protected Rigidbody rigidbody;
        protected Animator animator;
        protected PlayerMovement player;
        protected NavMeshAgent navMeshAgent;
        public override void OnAwake()
        {
            rigidbody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        }
    }

    public class PlayerClose : EnemyAction
    {
        public float detectionRange = 10f;
        public override TaskStatus OnUpdate()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.gameObject.transform.position);
            bool isClose = distanceToPlayer < detectionRange;
            return isClose ? TaskStatus.Success : TaskStatus.Running;
        }
        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            //Gizmos.DrawSphere(transform.position, detectionRange);
        }
    }

    public class MoveToPlayer : EnemyAction
    {
        public float moveSpeed = 5f;
        public float chaseStopDistance = 1f;

        public string animationTriggerName;
        public bool chaseEnded;
    
        public override void OnStart()
        {
            animator.SetTrigger(animationTriggerName);
            navMeshAgent.speed = moveSpeed;

        }
        
        public override TaskStatus OnUpdate()
        {
            navMeshAgent.SetDestination(player.gameObject.transform.position);
            //float distanceToPlayer = navMeshAgent.remainingDistance;
            float distanceToPlayer = Vector3.Distance(transform.position, player.gameObject.transform.position);
            chaseEnded = distanceToPlayer < chaseStopDistance ? true : false;

            return chaseEnded ? TaskStatus.Success : TaskStatus.Running;
        }

/* using Coroutine, but this is not function to allway chase case
        public override void OnStart()
        {
            animator.SetTrigger(animationTriggerName);
            //StartCoroutine(StartChase(DelayTime));
        }
        IEnumerator StartChase(float DelayTime)
        {
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.SetDestination(player.gameObject.transform.position);
            yield return new WaitForSeconds(DelayTime);
            float distanceToPlayer = navMeshAgent.remainingDistance;
            chaseEnded = distanceToPlayer < chaseStopDistance ? true : false;
        }
        public override TaskStatus OnUpdate()
        {
            return chaseEnded ? TaskStatus.Success : TaskStatus.Running;
        }
*/ 
    }
    public class AttackPlayer : EnemyAction
    {
        public string animationTriggerName;
        public string animationTriggerName_GoToIdle;
        public bool isAttack;
        public float DelayTime = 3f;
        public float attackAngle = 30f; // 원뿔의 각도
        public float attackDistance = 3f; // 원뿔의 거리
        public int raysPerAttack = 10; // 원뿔 내에서 발사할 레이 수

        public override void OnStart()
        {
            isAttack = false;
            animator.SetTrigger(animationTriggerName);
            StartCoroutine(StartAttack(DelayTime));
        }

        private IEnumerator StartAttack(float DelayTime)
        {
            yield return new WaitForSeconds(DelayTime);
            ConeRaycast();
            animator.SetTrigger(animationTriggerName_GoToIdle);
            isAttack = true;
        }

        public void ConeRaycast()
        {
            Vector3 forward = transform.forward;

            float Angle = -attackAngle / 2f;
            bool isPlayerHit = false;
            for (int i = 0; i < raysPerAttack; i++)
            {
                Quaternion rotation = Quaternion.Euler(0, Angle, 0);

                Vector3 rayDirection = rotation * forward;

                if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, attackDistance) && !isPlayerHit)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red, 1f); // 디버그 선
                    if (hit.collider.CompareTag("Player"))
                    {
                        Debug.Log("Player hit by attack ray!");
                        hit.collider.GetComponent<PlayerMovement>().HP -= 5;
                        // 플레이어에 데미지를 입히는 로직 추가
                        isPlayerHit = true;
                    }
                }
                else
                {
                    Debug.DrawRay(transform.position, rayDirection * attackDistance, Color.blue, 5f);
                }
                Angle += (attackAngle*2) / raysPerAttack;
            }
        }

        public override TaskStatus OnUpdate()
        {
            return isAttack ? TaskStatus.Success : TaskStatus.Running;
        }
    }

}

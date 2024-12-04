using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Core.AI
{
    public class BossAttack : AttackPlayer
    {
        public float Force = 50f;
        public override void OnStart()
        {
            isAttack = false;
            animator.SetTrigger(animationTriggerName);
            StartCoroutine(StartBossAttack(DelayTime));
        }
        private IEnumerator StartBossAttack(float DelayTime)
        {
            yield return new WaitForSeconds(DelayTime);
            ConeRaycastForBoss();
            animator.SetTrigger(animationTriggerName_GoToIdle);
            isAttack = true;
        }
        public void ConeRaycastForBoss()
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
                        // 플레이어에 데미지를 입히는 로직 추가
                        isPlayerHit = true;
                        Vector3 forceDirection = -(player.gameObject.transform.forward).normalized;
                        player.gameObject.GetComponent<Rigidbody>().AddForce(forceDirection * Force, ForceMode.Impulse);
                    }
                }
                else
                {
                    Debug.DrawRay(transform.position, rayDirection * attackDistance, Color.blue, 1f);
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

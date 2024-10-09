using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Test
{
    
    [TaskCategory("Test")]
    public class TestObjectFind : Action
    {
        public SharedGameObject targetObject;
        public SharedFloat fieldOfViewAngle = 30;
        public SharedFloat viewDistance = 1000;

        public SharedBool isFind = new SharedBool();

        public override TaskStatus OnUpdate()
        {
            Debug.Log(isFind.Value + ": Find Object");
            float halfFOV = fieldOfViewAngle.Value * 0.5f;
            int rayCount = 10;
            for (int i = 0; i <= rayCount; i++)
            {
                float angle = -halfFOV + (i * (fieldOfViewAngle.Value / rayCount));
                Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction, out hit, viewDistance.Value))
                {
                    if (hit.collider.gameObject == targetObject.Value)
                    {
                        isFind.Value = true;
                        return TaskStatus.Success;
                    }
                }
            }
            isFind.Value = false;
            return TaskStatus.Failure;
        }
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var oldColor = UnityEditor.Handles.color;
            var color = Color.yellow;
            color.a = 0.1f;
            UnityEditor.Handles.color = color;

            var halfFOV = fieldOfViewAngle.Value * 0.5f;
            var beginDirection = Quaternion.AngleAxis(-halfFOV, Vector3.up) * Owner.transform.forward;
            UnityEditor.Handles.DrawSolidArc(Owner.transform.position, Owner.transform.up, beginDirection, fieldOfViewAngle.Value, viewDistance.Value);

            UnityEditor.Handles.color = oldColor;
#endif
        }
    }

    [TaskCategory("Test")]
    public class TestMove : Action
    {
        public SharedFloat speed = 2;
        public SharedGameObject target;
        protected NavMeshAgent navMeshAgent;
        public Action TestObjectFind;

        public override void OnAwake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public override TaskStatus OnUpdate()
        {
            TaskStatus findStatus = TestObjectFind.OnUpdate();
            Debug.Log("Move1");
            Debug.Log(findStatus + ": Move");
            
            if (findStatus == TaskStatus.Success)
            {
                navMeshAgent.updateRotation = true;
                Debug.Log("Move2");
                navMeshAgent.SetDestination(target.Value.transform.position);
                navMeshAgent.speed = speed.Value;

                // 도착했는지 확인
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    return TaskStatus.Success;
                }
                return TaskStatus.Running;
            }
            else
            {
                navMeshAgent.updateRotation = false;
                navMeshAgent.velocity = Vector3.zero;
            }
            return TaskStatus.Failure;
        }
    }
    /*
    bool ShootRay(SharedFloat fieldOfViewAngle, SharedFloat viewDistance, SharedGameObject targetObject)
    {
        float halfFOV = fieldOfViewAngle.Value * 0.5f;
        int rayCount = 10;
        for (int i = 0; i <= rayCount; i++)
        {
            float angle = -halfFOV + (i * (fieldOfViewAngle.Value / rayCount));
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, viewDistance.Value))
            {
                if (hit.collider.gameObject == targetObject.Value)
                {
                    return true;
                }
            }
        }
        return false;
    }
    */
}
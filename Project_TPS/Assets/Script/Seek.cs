using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]
    [TaskIcon("Assets/Behavior Designer Tutorials/Tasks/Editor/{SkinColor}SeekIcon.png")]
    public class Seek : Action
    {
        public SharedFloat speed = 1;
        public SharedFloat angularSpeed = 50;
        public SharedFloat arriveDistance = 0.2f;
        public SharedGameObject target;
        public SharedVector3 targetPosition;

        protected UnityEngine.AI.NavMeshAgent navMeshAgent;

        public override void OnAwake()
        {
            navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        public override void OnStart()
        {
            navMeshAgent.speed = speed.Value;
            navMeshAgent.angularSpeed = angularSpeed.Value;
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
            navMeshAgent.Resume();
#else
            navMeshAgent.isStopped = false;
#endif

            SetDestination(Target());
        }
        public override TaskStatus OnUpdate()
        {
            if (HasArrived()) {
                return TaskStatus.Success;
            }

            SetDestination(Target());

            return TaskStatus.Running;
        }

        private Vector3 Target()
        {
            if (target.Value != null) {
                return target.Value.transform.position;
            }
            return targetPosition.Value;
        }

        private bool SetDestination(Vector3 destination)
        {
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
            navMeshAgent.Resume();
#else
            navMeshAgent.isStopped = false;
#endif
            return navMeshAgent.SetDestination(destination);
        }

        private bool HasArrived()
        {
            float remainingDistance;
            if (navMeshAgent.pathPending) {
                remainingDistance = float.PositiveInfinity;
            } else {
                remainingDistance = navMeshAgent.remainingDistance;
            }

            return remainingDistance <= arriveDistance.Value;
        }
        private void Stop()
        {
            if (navMeshAgent.hasPath) {
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
                navMeshAgent.Stop();
#else
                navMeshAgent.isStopped = true;
#endif
            }
        }
        public override void OnEnd()
        {
            Stop();
        }
        public override void OnBehaviorComplete()
        {
            Stop();
        }
    }
}
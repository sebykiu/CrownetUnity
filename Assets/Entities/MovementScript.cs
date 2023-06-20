using System;
using UnityEngine;

namespace Entities
{
    public class MovementScript : MonoBehaviour
    {
        public string id;
        public Vector3 targetPosition;
        public float speed;
        private Animator _animator;
        private static readonly int AnimationSpeed = Animator.StringToHash("Speed");


        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void Update()
        {
            Vector3 previousPosition = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Rotate the object to face the target.
            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            if (directionToTarget != Vector3.zero) // To avoid errors when the direction vector is zero.
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }          
            
            float actualSpeed = Vector3.Distance(transform.position, previousPosition) / Time.deltaTime;
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                actualSpeed = 0;
            }

            _animator.SetFloat(AnimationSpeed, actualSpeed);
        }

        public void SetTargetPosition(Vector3 newTarget)
        {
            targetPosition = newTarget;
        }

        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
        }
    }
}
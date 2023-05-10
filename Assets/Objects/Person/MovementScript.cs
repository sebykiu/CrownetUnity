using System;
using UnityEngine;

namespace Objects.Person
{
    public class MovementScript : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int Speed = Animator.StringToHash("Speed");

        private void Start()
        {
            _animator = GetComponent<Animator>();

        }

        private void Update()
        {
            var velocity = Vector3.forward * 2.0f;
            transform.Translate(velocity * Time.deltaTime);
            _animator.SetFloat(Speed, velocity.magnitude);
        }
    }
}   
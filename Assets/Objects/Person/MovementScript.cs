using UnityEngine;

namespace Objects.Person
{
    public class MovementScript : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private float _timer;
        private const float WalkDuration = 5f;
        private const float IdleDuration = 10f;


        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            Debug.Log("[MovementScript - Update]");

            // if (!(_timer < WalkDuration))
            // {
            //     if (_timer < IdleDuration)
            //     {
            //         _animator.SetFloat(Speed, 0.001f);
            //         _timer += Time.deltaTime;
            //     }
            //     else
            //     {
            //         _timer = 0;
            //     }
            // }
            // else
            // {
            //     var velocity = Vector3.forward * 2.0f;
            //     transform.Translate(velocity * Time.deltaTime);
            //     _animator.SetFloat(Speed, velocity.magnitude);
            //     _timer += Time.deltaTime;
            // }
        }
    }
}
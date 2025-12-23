using UnityEngine;

namespace DFC2000.Core
{
    [RequireComponent(typeof(CharacterController))]
    public class DFCMotor : MonoBehaviour
    {
        [Header("Kinematic Feel")]
        [Tooltip("Time in seconds to reach max speed from standstill.")]
        [SerializeField] private float accelerationTime = 0.3f;
        
        [Tooltip("Time in seconds to come to a full stop.")]
        [SerializeField] private float frictionTime = 0.15f;
        
        [Tooltip("Max movement speed.")]
        [SerializeField] private float maxSpeed = 6.0f;
        
        [Tooltip("Rotation speed in degrees per second. Should be 'heavy'.")]
        [SerializeField] private float rotationSpeed = 360f;

        [Header("Procedural Animation")]
        [SerializeField] private Transform modelTransform;
        [SerializeField] private float leanAmount = 2.5f;
        [SerializeField] private float leanSmoothTime = 0.1f;

        [Header("Ground Handling")]
        [SerializeField] private float gravity = 20.0f;
        [SerializeField] private float stickToGroundForce = 10.0f;

        // Internal State
        private CharacterController _characterController;
        private Vector3 _currentVelocity;
        private Vector3 _targetVelocity;
        private Vector3 _verticalVelocity;
        
        // Public properties for UI/Feedback
        public float CurrentSpeed => new Vector2(_currentVelocity.x, _currentVelocity.z).magnitude;
        public Vector3 GroundNormal { get; private set; } = Vector3.up;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        public void Move(Vector3 moveDirection, float speedMultiplier = 1.0f)
        {
            // 1. Calculate Target Velocity
            Vector3 targetVel = moveDirection.normalized * (maxSpeed * speedMultiplier);
            
            // 2. Apply Acceleration / Friction (Inertia)
            // If we are trying to move, accelerate. If input is zero, decelerate (friction).
            float smoothTime = (moveDirection.magnitude > 0.01f) ? accelerationTime : frictionTime;
            
            // Using SmoothDamp for that "Liquid/Aero" feel
            Vector3 currentPlanarVel = new Vector3(_currentVelocity.x, 0, _currentVelocity.z);
            Vector3 targetPlanarVel = new Vector3(targetVel.x, 0, targetVel.z);
            Vector3 velocityRef = Vector3.zero; // Needed for SmoothDamp, but we are keeping state in _currentVelocity manually for more control? 
            // Actually, let's implement the custom Accel/Friction model requested:
            // "Start-up lean and Braking slide"
            
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, targetPlanarVel, (maxSpeed / smoothTime) * Time.deltaTime);

            // 3. Rotation (Procedural Slerp)
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // 3.5 Procedural Lean
            if (modelTransform != null)
            {
                // Lean forward based on speed ratio
                float speedRatio = CurrentSpeed / maxSpeed;
                float targetLeanX = speedRatio * leanAmount; // Lean forward
                
                // Apply to local rotation (assuming model is child)
                Quaternion targetLean = Quaternion.Euler(targetLeanX, 0, 0);
                modelTransform.localRotation = Quaternion.Slerp(modelTransform.localRotation, targetLean, Time.deltaTime / leanSmoothTime);
            }

            // 4. Ground Handling & Gravity
            if (_characterController.isGrounded)
            {
                _verticalVelocity = Vector3.down * stickToGroundForce; // Stick to ground
                
                // Slope handling can be enhanced here by projecting movement onto plane
                // standard CC handles simple slopes, but for "Precise slope-sticking":
                // We keep it simple for now, relying on CC's slope limit, but ensure strict grounding.
            }
            else
            {
                _verticalVelocity += Vector3.down * gravity * Time.deltaTime;
            }

            // 5. Final Move
            Vector3 finalMove = _currentVelocity + _verticalVelocity;
            _characterController.Move(finalMove * Time.deltaTime);
        }
        
        public void Teleport(Vector3 position)
        {
            _characterController.enabled = false;
            transform.position = position;
            _characterController.enabled = true;
        }
    }
}

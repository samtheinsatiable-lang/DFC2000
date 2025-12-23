using UnityEngine;

namespace DFC2000.Camera
{
    public class DFCCamera : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(-10, 10, -10); // Isometric Offset
        
        [Header("Smoothness")]
        [SerializeField] private float smoothTime = 0.2f;
        
        [Header("Isometric Settings")]
        [Tooltip("True 35.264 degrees for perfect isometric projection.")]
        [SerializeField] private bool enforceIsometricAngle = true;

        private Vector3 _currentVelocity;
        private UnityEngine.Camera _cam;

        private void Awake()
        {
            _cam = GetComponent<UnityEngine.Camera>();
            if (_cam != null)
            {
                _cam.orthographic = true;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // 1. Follow Position
            Vector3 targetPos = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _currentVelocity, smoothTime);

            // 2. Enforce Rotation
            if (enforceIsometricAngle)
            {
                // Pitch: 35.264 (asin(1/sqrt(3))), Yaw: 45
                transform.rotation = Quaternion.Euler(35.264f, 45f, 0f);
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}

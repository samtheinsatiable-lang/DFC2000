using UnityEngine;

namespace DFC2000.Interaction
{
    public class DFCInteraction : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float detectionRadius = 1.5f;
        [SerializeField] private float forwardOffset = 1.0f;
        [SerializeField] private LayerMask interactableLayer;

        [Header("Debug")]
        [SerializeField] private bool showGizmos = true;

        private IInteractable _currentInteractable;

        public bool CanInteract => _currentInteractable != null;

        private void Update()
        {
            CheckForInteractables();
        }

        private void CheckForInteractables()
        {
            // Sphere cast in front of player
            Vector3 origin = transform.position + transform.forward * forwardOffset;
            Collider[] hits = Physics.OverlapSphere(origin, detectionRadius, interactableLayer);

            _currentInteractable = null;

            foreach (var hit in hits)
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    _currentInteractable = interactable;
                    // TODO: Show Glossy "Aero" Icon Feedback here
                    // e.g., UIManager.Instance.ShowInteractPrompt(interactable.Prompt);
                    break; // Prioritize first found
                }
            }
        }

        public void TriggerInteraction()
        {
            if (_currentInteractable != null)
            {
                Debug.Log($"<color=cyan>DFC2000 > Interacting with {_currentInteractable.Name}</color>");
                _currentInteractable.OnInteract(gameObject); 
            }
        }

        private void OnDrawGizmos()
        {
            if (showGizmos)
            {
                Gizmos.color = new Color(0, 0, 1, 0.4f); // Blue
                Vector3 origin = transform.position + transform.forward * forwardOffset;
                Gizmos.DrawSphere(origin, detectionRadius);
            }
        }
    }

    public interface IInteractable
    {
        string Name { get; }
        void OnInteract(GameObject interactor);
    }
}

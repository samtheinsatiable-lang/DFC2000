using UnityEngine;

namespace DFC2000.NPC
{
    public class DFCNPC : MonoBehaviour, DFC2000.Interaction.IInteractable
    {
        [Header("Identity")]
        [SerializeField] private string npcName = "Dad";
        
        [Header("Tutorial State")]
        [SerializeField] private Transform moveTarget;
        [SerializeField] private float moveSpeed = 3.0f;
        [SerializeField] private float stoppingDistance = 3.5f; // New: stop before hitting the dino/target
        
        [Header("Inventory Logic")]
        public static string CurrentFood = ""; 

        private int _interactionStage = 0;
        private bool _isMoving = false;

        public string Name => npcName;

        private void Update()
        {
            if (_isMoving && moveTarget != null)
            {
                float distance = Vector3.Distance(transform.position, moveTarget.position);
                
                // Stop if we are within stopping distance
                if (distance <= stoppingDistance)
                {
                    _isMoving = false;
                    Debug.Log($"<color=green>{npcName} arrived at safe observation distance.</color>");
                    return;
                }

                // Move towards target
                transform.position = Vector3.MoveTowards(transform.position, moveTarget.position, moveSpeed * Time.deltaTime);
                
                // Rotate towards target
                Vector3 direction = (moveTarget.position - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 5f * Time.deltaTime);
                }
            }
        }

        public void OnInteract(GameObject interactor)
        {
            switch (_interactionStage)
            {
                case 0:
                    Debug.Log($"<color=yellow>{npcName}: 'Welcome to DFC2000, Researcher. Use [W,A,S,D] to move. Press [E] to talk.'</color>");
                    _interactionStage++;
                    break;

                case 1:
                    Debug.Log($"<color=yellow>{npcName}: 'See that Nigersaurus? Let me get a closer look.'</color>");
                    _isMoving = true;
                    _interactionStage++;
                    break;

                case 2:
                    var inventory = interactor.GetComponent<DFC2000.Core.DFCController>().Inventory;
                    if (inventory != null)
                    {
                        Debug.Log($"<color=yellow>{npcName}: 'Here, take this Fern-Apple. Try feeding it.'</color>");
                        inventory.AddItem("Fern-Apple");
                        _interactionStage++;
                    }
                    break;

                default:
                    Debug.Log($"<color=yellow>{npcName}: 'Go on, use the Digitizer [F] to catch it!'</color>");
                    break;
            }
        }
    }
}

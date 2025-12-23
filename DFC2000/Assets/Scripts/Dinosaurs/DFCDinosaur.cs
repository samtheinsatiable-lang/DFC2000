using UnityEngine;

namespace DFC2000.Dinosaurs
{
    public class DFCDinosaur : MonoBehaviour, DFC2000.Interaction.IInteractable
    {
        [Header("Data")]
        [SerializeField] private string speciesName = "Nigersaurus";
        [SerializeField] private string dinosaurID; // Unique ID
        
        [Header("State")]
        [SerializeField] private bool isEating = false;
        
        public string SpeciesName => speciesName;
        public string Name => speciesName;
        public bool IsEating => isEating;

        private void Awake()
        {
            if (string.IsNullOrEmpty(dinosaurID))
            {
                dinosaurID = System.Guid.NewGuid().ToString();
            }
        }

        public void SetEating(bool eating)
        {
            isEating = eating;
            if(isEating) Debug.Log($"<color=green>{speciesName} is happily eating the Fern-Apple.</color>");
        }

        public void OnInteract(GameObject interactor)
        {
            if (isEating) return;

            var inventory = interactor.GetComponent<DFC2000.Core.DFCController>().Inventory;
            if (inventory != null && inventory.HasItem("Fern-Apple"))
            {
                inventory.RemoveItem("Fern-Apple");
                SetEating(true);
            }
            else
            {
                Debug.Log($"<color=orange>{speciesName} looks hungry. Maybe it wants a Fern-Apple?</color>");
            }
        }
    }
}

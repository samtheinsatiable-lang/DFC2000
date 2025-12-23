using UnityEngine;

namespace DFC2000.Interaction
{
    public class DFCDigitizer : MonoBehaviour
    {
        [Header("Specs")]
        [SerializeField] private float captureRange = 5.0f;
        [SerializeField] private float baseCatchRate = 0.05f; // 5%
        [SerializeField] private float eatingCatchRate = 0.90f; // 90%

        [Header("State")]
        public bool IsMenuOpen = false;

        [Header("Digital Storage")]
        [SerializeField] private int maxStorageCapacity = 3;
        [SerializeField] private System.Collections.Generic.List<string> digitizedDinos = new System.Collections.Generic.List<string>();

        public System.Collections.Generic.List<string> DigitizedDinos => digitizedDinos;

        public void UpgradeStorage(int amount)
        {
            maxStorageCapacity += amount;
            Debug.Log($"<color=cyan>DIGITIZER >> UPGRADED. Capacity: {maxStorageCapacity}</color>");
        }

        public void ToggleMenu()
        {
            IsMenuOpen = !IsMenuOpen;
            
            // UI Feedback
            string status = IsMenuOpen ? "<color=blue>DIGITIZER >> ONLINE</color>" : "<color=grey>DIGITIZER >> STANDBY</color>";

            if(IsMenuOpen) status += $" [Stored: {digitizedDinos.Count}/{maxStorageCapacity}]";
            Debug.Log(status);
        }

        public void AttemptDigitize()
        {
            if (!IsMenuOpen) return;

            bool captureAttempted = false;

            Collider[] hits = Physics.OverlapSphere(transform.position, captureRange);
            foreach (var hit in hits)
            {
                var dinosaur = hit.GetComponent<DFC2000.Dinosaurs.DFCDinosaur>();
                if (dinosaur == null) continue;

                captureAttempted = true;
                float chance = baseCatchRate;
                if (dinosaur.IsEating) 
                {
                     chance = eatingCatchRate;
                }

                Debug.Log($"<color=cyan>Target Found: {dinosaur.SpeciesName}. Capture Chance: {chance * 100}%</color>");

                if (digitizedDinos.Count >= maxStorageCapacity)
                {
                    Debug.Log("<color=red>STORAGE FULL. Upgrade Required.</color>");
                    return;
                }

                if (Random.value < chance) {
                       Debug.Log($"<color=cyan>DIGITIZATION SUCCESS! {dinosaur.SpeciesName} added to device.</color>");
                       digitizedDinos.Add(dinosaur.SpeciesName);
                       Destroy(dinosaur.gameObject); 
                       break; // Catch one at a time
                } else {
                       Debug.Log("<color=red>CAPTURE FAILED.</color>");
                }
            }

            if (!captureAttempted)
            {
                Debug.Log("No Valid Target in Range.");
            }

            // User Feedback Implies: "When I press F twice... it stays in sneak mode".
            // Fix: Close menu after any attempt (auto-holster).
            ToggleMenu(); 
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, captureRange);
        }
    }
}

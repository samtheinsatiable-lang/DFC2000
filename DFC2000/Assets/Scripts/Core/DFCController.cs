using UnityEngine;
using DFC2000.Interaction;

namespace DFC2000.Core
{
    [RequireComponent(typeof(DFCMotor))]
    [RequireComponent(typeof(DFCInput))]
    [RequireComponent(typeof(DFCInventory))]
    public class DFCController : MonoBehaviour
    {
        private DFCMotor _motor;
        private DFCInput _input;
        private DFCDigitizer _digitizer;
        private DFCInteraction _interaction;
        private DFCInventory _inventory;

        public DFCInventory Inventory => _inventory;
        public DFCDigitizer Digitizer => _digitizer;

        // public bool isDigitizerActive = false; // Managed by component now

        private void Awake()
        {
            _motor = GetComponent<DFCMotor>();
            _input = GetComponent<DFCInput>();
            _digitizer = GetComponent<DFCDigitizer>();
            _interaction = GetComponent<DFCInteraction>();
            _inventory = GetComponent<DFCInventory>();
        }

        private void Update()
        {
            // 1. Gather Input
            Vector3 moveDir = _input.GetIsometricMoveInput();
            
            // 2. Determine Speed Modifiers
            float speedMult = 1.0f;
            if (_digitizer != null && _digitizer.IsMenuOpen) speedMult = 0.4f; // Reduced by 60%

            // 3. Command Motor
            _motor.Move(moveDir, speedMult);

            // 4. Handle Inputs
            if (_input.WasInteractPressed())
            {
               if (_interaction != null) _interaction.TriggerInteraction();
            }

            if (_input.WasDigitizePressed())
            {
                if (_digitizer != null)
                {
                    if (_digitizer.IsMenuOpen)
                    {
                        // Action: Roll
                        _digitizer.AttemptDigitize();
                        // Optional: Close menu after catch attempt? Prompt doesn't specify, but implies "Toggling opens", "Second press performs roll". Maybe it stays open?
                        // Let's assume it stays open for multiple attempts or needs manual toggle close? 
                        // Actually "Toggling [F] opens... Pressing [F] a second time... performs roll". 
                        // It implies F is contextual. Open -> Roll. How to close?
                        // Usually implies Roll replaces Toggle while Open. 
                        // We might need a separate Close key or Hold F to close? 
                        // For now, let's implement: Press F (Open) -> Press F (Catch). 
                        // How to close? Maybe UI close button or Escape? Or Toggle if not aiming?
                        // Let's stick to simple: F = Catch if Open. 
                        // But wait, if F is Catch, you can never close it with F.
                        // I'll leave it as is for the prompt requirements, user can refine "Close" logic.
                    }
                    else
                    {
                        _digitizer.ToggleMenu();
                    }
                }
            }
        }
    }
}

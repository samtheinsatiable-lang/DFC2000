using UnityEngine;
using UnityEngine.InputSystem;

namespace DFC2000.Core
{
    public class DFCInput : MonoBehaviour
    {
        [Header("Input References")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference interactAction;
        [SerializeField] private InputActionReference digitizeAction;
        [SerializeField] private InputActionReference inventoryAction;

        public bool IsInteractData { get; private set; }
        public bool IsDigitizeTriggered { get; private set; }

        private void OnEnable()
        {
            if (moveAction != null) moveAction.action.Enable();
            if (interactAction != null) interactAction.action.Enable();
            if (digitizeAction != null) digitizeAction.action.Enable();
            if (inventoryAction != null) inventoryAction.action.Enable();
        }

        private void OnDisable()
        {
            if (moveAction != null) moveAction.action.Disable();
            if (interactAction != null) interactAction.action.Disable();
            if (digitizeAction != null) digitizeAction.action.Disable();
            if (inventoryAction != null) inventoryAction.action.Disable();
        }

        public Vector2 GetRawMoveInput()
        {
            if (moveAction == null) return Vector2.zero;
            return moveAction.action.ReadValue<Vector2>();
        }

        public Vector3 GetIsometricMoveInput()
        {
            Vector2 raw = GetRawMoveInput();
            if (raw.sqrMagnitude < 0.01f) return Vector3.zero;

            // Remap input to 45-degree isometric plane
            // W (0,1) should become (1,0,1) normalized in World Space
            // We rotate the 2D input by -45 degrees (or +45 depending on camera)
            // If Cam Y is 45, Forward is (+1, 0, +1).
            
            // Standard 2D Input: Y is Forward (Z), X is Right (X).
            // We want Y (Input Up) to map to World (1, 0, 1).
            
            // Convert to 3D flat
            Vector3 input3D = new Vector3(raw.x, 0, raw.y);
            
            // Rotate 45 degrees around Y axis
            // Quaternion.Euler(0, 45, 0) * Vector3.forward (0,0,1) = (0.7, 0, 0.7) approx
            
            return Quaternion.Euler(0, 45, 0) * input3D;
        }

        public bool WasInteractPressed()
        {
            return interactAction != null && interactAction.action.WasPressedThisFrame();
        }

        public bool WasDigitizePressed()
        {
            return digitizeAction != null && digitizeAction.action.WasPressedThisFrame();
        }

        public bool WasInventoryPressed()
        {
            // 1. Check Action (if assigned and enabled)
            if (inventoryAction != null && inventoryAction.action != null && inventoryAction.action.enabled)
            {
                if (inventoryAction.action.WasPressedThisFrame()) return true;
            }

            // 2. Hard Fallback (New Input System compliant)
            // Use Keyboard.current directly to avoid legacy input errors
            if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame) return true;

            return false;
        }
    }
}

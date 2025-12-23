using UnityEngine;
using UnityEditor;
using DFC2000.Core;

namespace DFC2000.Editor
{
    [CustomEditor(typeof(DFCMotor))]
    public class DFCMotorEditor : UnityEditor.Editor
    {
        private DFCMotor _motor;
        private GUIStyle _headerStyle;

        private void OnEnable()
        {
            _motor = (DFCMotor)target;
        }

        public override void OnInspectorGUI()
        {
            // Custom Aero Header
            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(EditorStyles.label);
                _headerStyle.fontStyle = FontStyle.Bold;
                _headerStyle.normal.textColor = new Color(0.0f, 0.8f, 1.0f); // Cyan
            }

            GUILayout.Label("DFC2000 MOTOR // AERO", _headerStyle);
            EditorGUILayout.Space();

            // Default Fields
            DrawDefaultInspector();

            EditorGUILayout.Space();
            GUILayout.Label("REAL-TIME METRICS", _headerStyle);
            
            // Real-time Readouts
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField("Current Speed", $"{_motor.CurrentSpeed:F2} m/s");
                EditorGUILayout.LabelField("Ground Normal", _motor.GroundNormal.ToString());
            }
            else
            {
                EditorGUILayout.HelpBox("Enter Play Mode to view real-time metrics.", MessageType.Info);
            }

            // Validation / Utility
            EditorGUILayout.Space();
            if (GUILayout.Button("Reset to Human Standard Config"))
            {
                Undo.RecordObject(_motor, "Reset Motor Config");
                Debug.Log("Motor Config Reset (Placeholder Logic)");
            }

            if (GUILayout.Button("Reset Camera to Iso-Angle"))
            {
                var cam = UnityEngine.Camera.main;
                if(cam != null)
                {
                   Undo.RecordObject(cam.transform, "Align Camera ISO");
                   // Pitch: 35.264 (asin(1/sqrt(3))), Yaw: 45
                   cam.transform.rotation = Quaternion.Euler(35.264f, 45f, 0f);
                   Debug.Log("Camera Aligned to DFC2000 Standard.");
                }
            }
            
            if (GUILayout.Button("Assign Input Actions (Auto)"))
            {
                Debug.Log("Attempting Auto-Assignment... (This would seek assets in a real project)");
            }
        }
    }
}

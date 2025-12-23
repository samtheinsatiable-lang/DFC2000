using UnityEngine;
using DFC2000.Core;

namespace DFC2000.UI
{
    public class DFCInventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DFCController controller;

        [Header("Aesthetic")]
        [SerializeField] private Color glassColor = new Color(0, 0.8f, 1f, 0.1f); // Aero Cyan
        [SerializeField] private Color glassBorder = new Color(0, 0.8f, 1f, 0.5f);
        
        private bool _isOpen = false;
        private int _selectedTab = 0; // 0 = Items, 1 = Dinos
        private string[] _tabs = new string[] { "ITEMS", "DINOS" };

        private void Start()
        {
            if (controller == null) controller = GetComponent<DFCController>();
            if (controller == null) controller = FindAnyObjectByType<DFCController>();
        }

        private void Update()
        {
            // Toggle with I
            // We access Input via controller if possible, or direct if needed.
            // Since DFCInput is on the same object usually:
            var input = controller != null ? controller.GetComponent<DFCInput>() : null;
            if (input != null && input.WasInventoryPressed())
            {
                Debug.Log($"<color=cyan>INVENTORY >> TOGGLE requested. Currently: {_isOpen}</color>");
                _isOpen = !_isOpen;
            }
        }

        private void OnGUI()
        {
            if (!_isOpen || controller == null) return;

            // --- STYLES ---
            // Create styles every frame is okay for OnGUI prototyping, or cache them.
            GUIStyle containerStyle = new GUIStyle(GUI.skin.box);
            containerStyle.normal.background = Texture2D.whiteTexture;
            
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontSize = 42;
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.normal.textColor = new Color(0.8f, 1f, 0.9f); // LCD Pale Greenish White

            GUIStyle tabStyle = new GUIStyle(GUI.skin.button);
            tabStyle.fontSize = 24;
            tabStyle.fontStyle = FontStyle.Bold;
            tabStyle.normal.textColor = Color.white;
            tabStyle.hover.textColor = Color.cyan;

            // --- FULLSCREEN LAYOUT ---
            // Leave a small margin for that "Monitor Bezel" feel
            float margin = 40;
            Rect screenRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));

            // Background (LCD Backlight)
            GUI.color = new Color(0, 0.2f, 0.3f, 0.9f); // Dark Teal LCD background
            GUI.Box(screenRect, "", containerStyle);

            // Scanlines Effect (Horizontal lines)
            GUI.color = new Color(0, 0, 0, 0.1f);
            for (float y = screenRect.y; y < screenRect.yMax; y += 4)
            {
                GUI.DrawTexture(new Rect(screenRect.x, y, screenRect.width, 1), Texture2D.whiteTexture);
            }

            GUI.color = Color.white;

            GUILayout.BeginArea(screenRect);
            {
                GUILayout.Space(20);

                // HEADER
                GUILayout.Label("DFC-2000 PERSONAL ORGANIZER", headerStyle);
                GUILayout.Label("------------------------------------------------", headerStyle);

                GUILayout.Space(30);

                // TABS (Big and Chunky)
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                float tabWidth = 200;
                float tabHeight = 60;
                
                // Custom toggle logic for "Bubbly" Feel (Active tab is Cyan, Inactive is Gray)
                Color oldColor = GUI.backgroundColor;
                
                GUI.backgroundColor = _selectedTab == 0 ? Color.cyan : Color.gray;
                if (GUILayout.Button("ITEMS", tabStyle, GUILayout.Width(tabWidth), GUILayout.Height(tabHeight))) _selectedTab = 0;
                
                GUILayout.Space(20);
                
                GUI.backgroundColor = _selectedTab == 1 ? Color.cyan : Color.gray;
                if (GUILayout.Button("DINOS", tabStyle, GUILayout.Width(tabWidth), GUILayout.Height(tabHeight))) _selectedTab = 1;
                
                GUI.backgroundColor = oldColor;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(30);

                // CONTENT AREA
                GUILayout.BeginVertical();
                {
                    if (_selectedTab == 0) DrawItems();
                    else DrawDinos();
                }
                GUILayout.EndVertical();

                // FOOTER
                GUILayout.FlexibleSpace();
                
                GUIStyle footerStyle = new GUIStyle(GUI.skin.label);
                footerStyle.alignment = TextAnchor.MiddleCenter;
                footerStyle.fontSize = 18;
                footerStyle.normal.textColor = Color.cyan;
                GUILayout.Label("[PRESS 'I' TO POWER OFF]", footerStyle);
                
                GUILayout.Space(20);
            }
            GUILayout.EndArea();
        }

        private void DrawItems()
        {
            if (controller.Inventory == null) return;
            
            var items = controller.Inventory.Items;
            
            // Item Style
            GUIStyle itemStyle = new GUIStyle(GUI.skin.box);
            itemStyle.fontSize = 28;
            itemStyle.alignment = TextAnchor.MiddleLeft;
            itemStyle.padding = new RectOffset(20, 0, 10, 10);
            itemStyle.normal.textColor = Color.white;
            
            if (items.Count == 0)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontSize = 28;
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = new Color(1, 1, 1, 0.5f);
                GUILayout.Label("- NO ITEMS -", style);
                return;
            }

            foreach (var item in items)
            {
                // Draw a simple "pill" shape box
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box($"> {item}", itemStyle, GUILayout.Width(600), GUILayout.Height(60));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
        }

        private void DrawDinos()
        {
            if (controller.Digitizer == null) return;

            var dinos = controller.Digitizer.DigitizedDinos;
            
            // Dino Style
            GUIStyle dinoStyle = new GUIStyle(GUI.skin.box);
            dinoStyle.fontSize = 28;
            dinoStyle.alignment = TextAnchor.MiddleLeft;
            dinoStyle.padding = new RectOffset(20, 0, 10, 10);
            dinoStyle.normal.textColor = Color.cyan;

            if (dinos.Count == 0)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontSize = 28;
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = new Color(1, 1, 1, 0.5f);
                GUILayout.Label("- NO DATA -", style);
                return;
            }

            foreach (var dino in dinos)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                
                // Content
                string content = $"DINO-ID: {dino.ToUpper()}";
                GUILayout.Box(content, dinoStyle, GUILayout.Width(600), GUILayout.Height(60));
                
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
        }
    }
}

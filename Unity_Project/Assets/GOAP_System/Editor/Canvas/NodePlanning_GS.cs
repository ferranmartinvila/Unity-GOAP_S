using UnityEngine;
using UnityEditor;
using GOAP_S.AI;

namespace GOAP_S.UI
{
    [InitializeOnLoad]
    public sealed class NodePlanning_GS : EditorWindow
    {
        //UI fields
        private static Texture2D _back_texture = null; //Texture in the background of the window
        //Target fields
        private static Agent_GS _selected_agent = null; //The agent selected by the user

        //Static instance of this class
        private static NodePlanning_GS _Instance;

        //Property to get/set static instance
        public static NodePlanning_GS Instance
        {
            get
            {
                //Check if the instance is null, in null case generates a new one
                if (_Instance == null)
                {
                    _Instance = EditorWindow.GetWindow<NodePlanning_GS>(typeof(NodeEditor_GS));
                }
                return _Instance;
            }
            set
            {
                _Instance = value;
            }
        }

        //Loop Methods ================
        private void OnFocus()
        {
            if (_selected_agent == null)
            {
                OnSelectionChange();
            }
        }

        private void OnSelectionChange()
        {
            if(Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<Agent_GS>() == null)
            {
                _selected_agent = null;
            }
            else if(_selected_agent != Selection.activeGameObject.GetComponent<Agent_GS>())
            {
                _selected_agent = Selection.activeGameObject.GetComponent<Agent_GS>();
            }
        }

        private void OnEnable()
        {
            //Configure window on enable(title)
            ConfigureWindow();
            //Check selected agent
            OnSelectionChange();
        }

        private void OnGUI()
        {
            //Draw background texture 
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), back_texture, ScaleMode.StretchToFill);

            if (_selected_agent == null) return;

            GUILayout.Label("Test");
        }

        //Functionality Methods =======
        public static bool IsOpen()
        {
            return _Instance != null;
        }

        private void ConfigureWindow()
        {
            this.titleContent.text = "Planning";
            this.minSize = new Vector2(800.0f, 500.0f);
        }

        private void GenerateAgentUI()
        {

        }

        //Get/Set Methods =================
        private Texture2D back_texture
        {
            get
            {
                //Generate background texture
                if (_back_texture == null)
                {
                    _back_texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                    _back_texture.SetPixel(0, 0, new Color(0.15f, 0.15f, 0.15f));
                    _back_texture.Apply();
                }
                return _back_texture;
            }
        }
    }
}

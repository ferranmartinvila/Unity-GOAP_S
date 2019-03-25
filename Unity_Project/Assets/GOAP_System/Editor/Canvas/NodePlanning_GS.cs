using UnityEngine;
using UnityEditor;
using GOAP_S.AI;

namespace GOAP_S.UI
{
    [InitializeOnLoad]
    public sealed class NodePlanning_GS : ZoomableCanvas_GS
    {
        //Static instance of this class
        private static NodePlanning_GS _Instance;
        private AgentBehaviour_GS_Editor _agent_behaviour_editor = null; //Editor of the focused agent blackboard

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

        //Constructors ================
        static NodePlanning_GS()
        {
            //Reset selection on load
            _selected_agent = null;
        }

        //Loop Methods ================
        private void OnFocus()
        {
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            if(Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<Agent_GS>() == null)
            {
                //In null case set agent to null
                _selected_agent = null;
            }
            else if(_selected_agent != Selection.activeGameObject.GetComponent<Agent_GS>())
            {
                //Set selected agent
                _selected_agent = Selection.activeGameObject.GetComponent<Agent_GS>();
                //Generate selected agent UI
                GenerateAgentUI();
            }
        }

        private void OnEnable()
        {
            //Configure window on enable(title)
            ConfigureWindow();
            //Reset selection
            _selected_agent = null;
            //Check selected agent
            OnSelectionChange();
        }

        private void OnGUI()
        {
            //Draw background texture 
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), back_texture, ScaleMode.StretchToFill);

            if (_selected_agent == null)
            {
                //Show non agent title
                GUILayout.Label("No agent selected", UIConfig_GS.center_big_white_style);

                //Empty node editor inputs
                if (EditorWindow.focusedWindow == this && EditorWindow.mouseOverWindow == this)//Check if focus is on this windows
                {
                    //Right click
                    if (Event.current.button == 1)
                    {
                        //Get mouse pos
                        Vector2 _mouse_pos = Event.current.mousePosition;
                        //Show empty node editor popup menu
                        PopupWindow.Show(new Rect(_mouse_pos.x, _mouse_pos.y, 0, 0), new EmptyCanvasPopMenu_GS(this));
                    }
                }
                return;
            }

            //Selected agent title
            GUILayout.Label("Agent: " + _selected_agent.name, UIConfig_GS.center_big_white_style);

            //Window inputs
            if (EditorWindow.focusedWindow == this && EditorWindow.mouseOverWindow == this)//Check if focus is on this windows
            {
                //Right click
                if (Event.current.button == 1)
                {
                    //Get mouse pos
                    Vector2 _mouse_pos = Event.current.mousePosition;
                    //Show node editor popup menu
                    PopupWindow.Show(new Rect(_mouse_pos.x, _mouse_pos.y, 0, 0), new NodePlanningPopMenu_GS());
                }
            }

            BeginWindows();

            //Draw agent behaviour editor
            GUILayout.Window(_agent_behaviour_editor.id, _agent_behaviour_editor.window, _agent_behaviour_editor.DrawUI, "Behaviour", UIConfig_GS.canvas_window_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            //TODO: Draw planning nodes

            EndWindows();
        }

        //Functionality Methods =======
        public static bool IsOpen()
        {
            return _Instance != null;
        }

        public void CheckSelection()
        {
            OnSelectionChange();
        }

        private void ConfigureWindow()
        {
            this.titleContent.text = "Planning";
            this.minSize = new Vector2(800.0f, 500.0f);
        }

        private void GenerateAgentUI()
        {
            //Generate agent behaviour editor
            _agent_behaviour_editor = new AgentBehaviour_GS_Editor(_selected_agent);
            _agent_behaviour_editor.window_size = new Vector2(250.0f, 100.0f);
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

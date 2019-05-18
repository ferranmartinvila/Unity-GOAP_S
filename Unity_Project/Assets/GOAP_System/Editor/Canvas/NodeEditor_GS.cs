using UnityEngine;
using UnityEditor;
using GOAP_S.AI;
using GOAP_S.Tools;

namespace GOAP_S.UI
{
    [InitializeOnLoad]
    public sealed class NodeEditor_GS : ZoomableCanvas_GS
    {
        //Target fields
        private ActionNode_GS_Editor[] _action_node_editors = null; //Array where all the action nodes ui are stored
        private int _action_node_editors_num = 0; //Number node editors allocated in the array
        private Blackboard_GS_Editor _blackboard_editor = null; //Editor of the focused agent blackboard

        //Static instance of this class
        private static NodeEditor_GS _Instance;

        //Property to get/set static instance
        public static NodeEditor_GS Instance
        {
            get
            {
                //Check if the instance is null, in null case generates a new one
                if (_Instance == null)
                {
                    _Instance = EditorWindow.GetWindow<NodeEditor_GS>(typeof(NodePlanning_GS));
                }
                return _Instance;
            }
            set
            {
                _Instance = value;
            }
        }

        //Constructors ================
        static NodeEditor_GS()
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
            base.OnInspectorUpdate();

            if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<Agent_GS>() == null)
            {
                //In null case set agent to null
                _selected_agent = null;
                _action_node_editors = null;
                _action_node_editors_num = 0;

                Repaint();

                return;
            }
            else if(_selected_agent != Selection.activeGameObject.GetComponent<Agent_GS>())
            {
                //Set selected agent
                _selected_agent = Selection.activeGameObject.GetComponent<Agent_GS>();

                //Generate selected agent UI
                GenerateTargetAgentUI();

                Repaint();
            }
        }

        private void OnEnable()
        {
            //Configure window on enable
            ConfigureWindow();
            //Reset selection
            _selected_agent = null;
            //Check selected agent
            OnSelectionChange();
        }

        private void OnGUI()
        {
            //Draw background texture 
            GUI.DrawTexture(new Rect(0, 0, position.width, position.height), back_texture, ScaleMode.StretchToFill);

            //Check if there is an agent selected
            if(_selected_agent == null)
            {
                //Handle no agent input
                HandleNoAgentInput();
                return;
            }

            //Zoomable layout
            BeginZoomableLayout();

            //Zoomable layout area
            Rect area_rect = new Rect(-_zoom_position.x, -_zoom_position.y, ProTools.NODE_EDITOR_CANVAS_SIZE, ProTools.NODE_EDITOR_CANVAS_SIZE);
            GUILayout.BeginArea(area_rect);

            //Temp for guide
            for (int k = 0; k < area_rect.width; k += 200)
            {
                for (int y = 0; y < area_rect.height; y += 200)
                {
                    GUI.Label(new Rect(y, k, 120.0f, 25.0f), y + "||" + k);
                }
            }

            //Mark the beginning area of the popup windows
            BeginWindows();


            //Draw action nodes
            for (int k = 0; k < _action_node_editors_num; k++)
            {
                //Focus action node
                ActionNode_GS node = _selected_agent.action_nodes[k];
                //Focus action node editor
                ActionNode_GS_Editor node_editor = _action_node_editors[k];
                //Show node window
                node.window_rect = GUILayout.Window(node.id, node.window_rect, node_editor.DrawUI, node.name, UIConfig_GS.Instance.node_window_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                //Show description label
                GUI.Label(new Rect(node.window_position.x - node_editor.label_size.x, node.window_position.y, node_editor.label_size.x, node_editor.label_size.y), node.description, UIConfig_GS.left_white_style);
            }

            //Reset matrix to keep blackboard window scale 
            GUI.matrix = Matrix4x4.identity;
            //Update blackboard window to simulate static position
            _blackboard_editor.window_position = new Vector2(_zoom_position.x + position.width - ProTools.BLACKBOARD_MARGIN, 0 + _zoom_position.y);
            //Display blackboard window
            GUILayout.Window(_blackboard_editor.id, _blackboard_editor.window, _blackboard_editor.DrawUI, "Blackboard", UIConfig_GS.canvas_window_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            //End area of popup windows
            EndWindows();

            //End zoomable layout area
            GUILayout.EndArea();

            //End zoomable layout
            EndZoomableLayout();

            //Handle input
            HandleInput();
        }

        protected override void HandleNoAgentInput()
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
        }

        protected override void HandleInput()
        {


            //Window inputs
            if (EditorWindow.focusedWindow == this && EditorWindow.mouseOverWindow == this)//Check if focus is on this windows
            {
                //Right click
                if (Event.current.button == 1)
                {
                    //Get mouse pos
                    Vector2 _mouse_pos = Event.current.mousePosition;
                    //Show node editor popup menu
                    PopupWindow.Show(new Rect(_mouse_pos.x, _mouse_pos.y, 0, 0), new NodeEditorPopMenu_GS());
                    return;
                }
            }

            //Zoom input
            HandleZoomInput();
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
            //Set a window title
            titleContent.text = "Node Editor";
            //Set window min size
            minSize = new Vector2(ProTools.MIN_CANVAS_WIDTH, ProTools.MIN_CANVAS_HEIGHT);
            //Set canvas size
            _canvas_size = new Vector2(ProTools.NODE_EDITOR_CANVAS_SIZE, ProTools.NODE_EDITOR_CANVAS_SIZE);
            //Set canvas camera initial position
            _zoom_position = new Vector2(ProTools.NODE_EDITOR_CANVAS_SIZE * 0.5f, ProTools.NODE_EDITOR_CANVAS_SIZE * 0.5f);
        }

        private void GenerateTargetAgentUI()
        {
            //Alloc node editors array
            _action_node_editors_num = 0;
            _action_node_editors = new ActionNode_GS_Editor[_selected_agent.action_nodes_num == 0 ? ProTools.INITIAL_ARRAY_SIZE : _selected_agent.action_nodes_num];

            //Generate action nodes editors
            for (int k = 0; k < _selected_agent.action_nodes_num; k++)
            {
                //Allocate action node editor
                ActionNode_GS_Editor new_node_editor = new ActionNode_GS_Editor(_selected_agent.action_nodes[k]);
                //Add on the node editor
                _action_node_editors[_action_node_editors_num] = new_node_editor;
                //Update nodes count
                _action_node_editors_num += 1;
            }

            //Generate blackboard editor
            _blackboard_editor = new Blackboard_GS_Editor(_selected_agent.blackboard);
            _blackboard_editor.window_size = new Vector2(ProTools.BLACKBOARD_MARGIN, 100);
        }

        public void AddTargetAgentActionNodeEditor(ActionNode_GS new_node)
        {
            //Set new node agent
            new_node.agent = _selected_agent;

            //Check if we need to allocate more items in the array
            if (_action_node_editors_num == _action_node_editors.Length)
            {
                //Double array capacity
                ActionNode_GS_Editor[] new_array = new ActionNode_GS_Editor[_action_node_editors_num * 2];
                //Copy values
                for(int k = 0; k < _action_node_editors_num; k++)
                {
                    new_array[k] = _action_node_editors[k];
                }
                //Set new array
                _action_node_editors = new_array;
            }

            //Generate the new action node editor
            ActionNode_GS_Editor new_node_editor = new ActionNode_GS_Editor(new_node);
            //Add it to the array
            _action_node_editors[_action_node_editors_num] = new_node_editor;
            //Update editors num
            _action_node_editors_num += 1;
        }

        public void RemoveTargetAgentActionNodeEditor(ActionNode_GS_Editor target_node_editor)
        {
            for (int k = 0; k < _action_node_editors_num; k++)
            {
                if (_action_node_editors[k] == target_node_editor)
                {
                    if (k == _action_node_editors.Length - 1)
                    {
                        _action_node_editors[k] = null;
                    }
                    else
                    {
                        for (int i = k; i < _action_node_editors_num - 1; i++)
                        {
                            _action_node_editors[i] = _action_node_editors[i + 1];
                        }
                    }
                    //Update node count
                    _action_node_editors_num -= 1;
                }
            }
        }

        public void ClearPlanning()
        {
            //Clear action nodes
            for (int k = 0; k < _action_node_editors_num; k++)
            {
                _action_node_editors[k] = null;
            }
            _action_node_editors_num = 0;
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
                    _back_texture.SetPixel(0, 0, new Color(0.35f, 0.35f, 0.35f));
                    _back_texture.Apply();
                }
                return _back_texture;
            }
        }

        public ActionNode_GS_Editor[] action_node_editors
        {
            get
            {
                return _action_node_editors;
            }
        }

        public int action_node_editors_num
        {
            get
            {
                return _action_node_editors_num;
            }
        }

        public Blackboard_GS_Editor blackboard_editor
        {
            get
            {
                return _blackboard_editor;
            }
            set
            {
                _blackboard_editor = value;
            }
        }
    }
}
using UnityEngine;
using UnityEditor;
using GOAP_S.AI;
using GOAP_S.PT;

namespace GOAP_S.UI
{
    [InitializeOnLoad]
    public sealed class NodeEditor_GS : EditorWindow
    {
        //State fields
        private EventType _last_event_type; //Last event type
        //UI fields
        private static Texture2D _back_texture = null; //Texture in the background of the window
        private static Vector2 _mouse_position = Vector2.zero; //Used to track the mouse position in this window
        private Vector2 _mouse_motion = Vector2.zero; //Track the mouse motion, usefull in drag functionality
        private float _mouse_motion_relation = 1.2f; //Mouse motion is multiplied for this value to accurate the drag speed
        //Target fields
        private static Agent_GS _selected_agent = null; //The agent selected by the user
        private ActionNode_GS_Editor[] _action_node_editors = null; //List where all the action nodes ui are stored
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
            //Set selected agent to null on load
            _selected_agent = null;
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
            if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<Agent_GS>() == null)
            {
                _selected_agent = null;
                _action_node_editors = null;
                _action_node_editors_num = 0;

                Repaint();

                return;
            }
            else if(_selected_agent != Selection.activeGameObject.GetComponent<Agent_GS>())
            {
                _selected_agent = Selection.activeGameObject.GetComponent<Agent_GS>();

                GenerateTargetAgentUI();

                Repaint();
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

            if (_selected_agent == null)
            {
                //Empty node editor inputs
                if (EditorWindow.focusedWindow == this && EditorWindow.mouseOverWindow == this)//Check if focus is on this windows
                {
                    //Right click
                    if (Event.current.button == 1)
                    {
                        //Get mouse pos
                        Vector2 _mouse_pos = Event.current.mousePosition;
                        //Show empty node editor popup menu
                        PopupWindow.Show(new Rect(_mouse_pos.x, _mouse_pos.y, 0, 0), new EmptyCanvasPopMenu_GS());
                    }
                }
                return;
            }

            //Track mouse position and mouse motion
            if (_last_event_type == EventType.MouseUp ||_last_event_type == EventType.MouseDown)
            {
                _mouse_motion = Vector2.zero;
                _mouse_position = Event.current.mousePosition;
            }
            else
            {
                _mouse_motion = (Event.current.mousePosition - _mouse_position) * _mouse_motion_relation;
                _mouse_position = Event.current.mousePosition;
            }
        
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
                }

                //Middle click
                if (Event.current.button == 2)
                {
                    //Check if motion is not null
                    if (_mouse_motion.x != 0.0f || _mouse_motion.y != 0.0f)
                    {
                        //Iterate all the nodes 
                        for (int k = 0; k < _selected_agent.action_nodes_num; k++)
                        {
                            //Get the current action node
                            ActionNode_GS node = ((ActionNode_GS)_selected_agent.action_nodes[k]);
                            //Modify node position
                            node.window_position = new Vector2(node.window_position.x + (int)_mouse_motion.x, node.window_position.y + (int)_mouse_motion.y);
                        }
                    }
                    //Save the last event related with the mouse input(MouseUp is always the last)
                    _last_event_type = Event.current.type;
                    //On drag input you need to update UI constantly
                    Repaint();
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
                Rect node_rect = GUILayout.Window(node.id, node.window_rect, node_editor.DrawUI, node.name, UIConfig_GS.Instance.node_window_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                //Show description label
                GUI.Label(new Rect(node.window_position.x - node_editor.label_size.x, node.window_position.y, node_editor.label_size.x, node_editor.label_size.y), node.description, UIConfig_GS.Instance.node_description_style);

                //Move the node if it's position is editable
                if (node_rect.x != node.window_position.x || node_rect.y != node.window_position.y)
                {
                    node.window_position = new Vector2(node_rect.x, node_rect.y);
                }
            }
            
            //Draw agent blackboard
            GUILayout.Window(_selected_agent.blackboard.id, _blackboard_editor.window, _blackboard_editor.DrawUI, "Blackboard", UIConfig_GS.Instance.blackboard_window_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            
            //End area of popup windows
            EndWindows();
        }

        //Functionality Methods =======
        public static bool IsOpen()
        {
            return _Instance != null;
        }

        private void ConfigureWindow()
        {
            this.titleContent.text = "Node Editor"; //Set a window title
            this.minSize = new Vector2(800.0f, 500.0f);
        }

        private void GenerateTargetAgentUI()
        {
            //Check if window has been configured
            //ConfigureWindow();

            //Alloc node editors array
            _action_node_editors_num = 0;
            _action_node_editors = new ActionNode_GS_Editor[ProTools.INITIAL_ARRAY_SIZE];

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

        public Agent_GS selected_agent
        {
            get
            {
                return _selected_agent;
            }
            set
            {
                _selected_agent = value;
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

        public Vector2 mouse_position
        {
            get
            {
                return _mouse_position;
            }
        }
    }
}
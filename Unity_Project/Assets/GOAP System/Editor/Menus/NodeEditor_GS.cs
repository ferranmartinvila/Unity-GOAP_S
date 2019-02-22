using UnityEngine;
using UnityEditor;
using GOAP_S.AI;
using GOAP_S.PT;

namespace GOAP_S.UI
{
    public class NodeEditor_GS : EditorWindow
    {
        //State fields
        private static string _id = ""; //Node ID used to set pop window id
        private EventType _last_event_type; //Last event type
        //UI fields
        public UIConfig_GS UI_configuration = new UIConfig_GS(); //The UI configuration of the action node
        private static Texture2D _back_texture = null; //Texture in the background of the window
        private static Vector2 _mouse_position = Vector2.zero; //Used to track the mouse position in this window
        private Vector2 _mouse_motion = Vector2.zero; //Track the mouse motion, usefull in drag functionality
        private float _mouse_motion_relation = 1.2f; //Mouse motion is multiplied for this value to accurate the drag speed
        //Target fields
        private GameObject _selected_object = null;
        private Agent_GS _selected_agent = null;
        private ActionNode_GS_Editor[] _action_node_editors = null; //List where all the action nodes ui are stored
        private int _action_node_editors_num = 0; //Number node editors allocated in the array
        private Blackboard_GS_Editor _blackboard_editor = null; //Editor of the focused agent blackboard

        //Window Menu Item ================
        [MenuItem("Tools / GOAP / Node Editor")]
        static void ShowNodeEditor()
        {
            EditorWindow.GetWindow(typeof(NodeEditor_GS));
        }

        [MenuItem("Tools / GOAP / Node Editor", true)]
        static bool CheckAgentToShowNodeEditor()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Agent_GS>() != null;
        }

        //Loop Methods ====================     
        private void OnEnable()
        {
            //Configure window on enable(title, back texture, id content, ...)
            ConfigureWindow();
        }

        private void OnGUI()
        {
            //Draw background texture 
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), _back_texture, ScaleMode.StretchToFill);

            //Check if the current selected object is the same that the windows is focusing
            if (Selection.activeGameObject != _selected_object)
            {
                //Set agent to null in case of no object selected
                if (Selection.activeGameObject == null)
                {
                    _selected_agent = null;
                }
                //Set selected agent from the agent comp of the selected obj, if is null data is not shown
                else
                {
                    _selected_agent = Selection.activeGameObject.gameObject.GetComponent<Agent_GS>();
                    if(_selected_agent != null)
                    {
                        GenerateTargetAgentUI();
                    }
                }
                //Change the selected object and repaint the window content
                _selected_object = Selection.activeGameObject;
                //Repaint the window when the target object is changed
                Repaint();
            }

            //Check if the current selected object have agent if selected agent is null
            if (_selected_agent == null || _selected_agent.action_nodes == null)
            {
                //Get agent and return, so if is null is check in the next loop
                if (_selected_object != null)
                {
                    _selected_agent = _selected_object.GetComponent<Agent_GS>();
                    if(_selected_agent != null)
                    {
                        GenerateTargetAgentUI();
                    }
                    else
                    {
                        return;
                    }
                }
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
                    PopupWindow.Show(new Rect(_mouse_pos.x, _mouse_pos.y, 0, 0), new GOAP_S.UI.NodeEditorPopMenu_GS(this));
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
                //Focus target action node
                ActionNode_GS node = _selected_agent.action_nodes[k];
                //Generate the window
                Rect node_rect = GUILayout.Window(node.id, node.window_rect, _action_node_editors[k].DrawUI, node.name, UI_configuration.node_window_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                //Generate the node description label
                //First generate content to calculate the size
                GUIContent content = new GUIContent(node.description);
                //Get content size
                Vector2 label_size = UI_configuration.node_description_style.CalcSize(content);
                //Show the description in a position 
                GUI.Label(new Rect(node.window_position.x - label_size.x, node.window_position.y, label_size.x, label_size.y), node.description, UI_configuration.node_description_style);

                //Move the node if it's position is editable
                if (node_rect.x != node.window_position.x || node_rect.y != node.window_position.y)
                {
                    node.window_position = new Vector2(node_rect.x, node_rect.y);
                }
            }
            
            //Draw agent blackboard
            GUILayout.Window(_selected_agent.blackboard.id, _blackboard_editor.window, _blackboard_editor.DrawUI, "Blackboard", UI_configuration.blackboard_window_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            
            //End area of popup windows
            EndWindows();
        }

        /*void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 4);
        }*/

        //Functionality Methods =======
        private void ConfigureWindow()
        {
            this.titleContent.text = "Node Editor"; //Set a window title
            _id = System.Guid.NewGuid().ToString(); //Generate window UUID
            //Generate back texture
            if (_back_texture == null)
            {
                _back_texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                _back_texture.SetPixel(0, 0, new Color(0.35f, 0.35f, 0.35f));
                _back_texture.Apply();
            }
        }

        private void GenerateTargetAgentUI()
        {
            //Alloc node editors array
            _action_node_editors_num = 0;
            _action_node_editors = new ActionNode_GS_Editor[ProTools.INITIAL_ARRAY_SIZE];

            //Generate action nodes editors
            for (int k = 0; k < _selected_agent.action_nodes_num; k++)
            {
                //Allocate action node editor
                ActionNode_GS_Editor new_node_editor = new ActionNode_GS_Editor(_selected_agent.action_nodes[k], this);
                //Add on the node editor
                _action_node_editors[_action_node_editors_num] = new_node_editor;
                //Update nodes count
                _action_node_editors_num += 1;
            }

            //Generate blackboard editor
            _blackboard_editor = new Blackboard_GS_Editor(_selected_agent.blackboard, this);
            _blackboard_editor.window_size = new Vector2(250, 100);
        }

        public void AddTargetAgentNodeUI(ActionNode_GS new_node)
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
            ActionNode_GS_Editor new_node_editor = new ActionNode_GS_Editor(new_node, this);
            //Add it to the array
            _action_node_editors[_action_node_editors_num] = new_node_editor;
            //Update editors num
            _action_node_editors_num += 1;
        }

        public void DeleteTargetAgentNodeUI(ActionNode_GS_Editor target_node_editor)
        {
            int len = _action_node_editors.Length;
            for (int k = 0; k < len; k++)
            {
                if (_action_node_editors[k] == target_node_editor)
                {
                    if (k == len - 1) _action_node_editors[k] = null;
                    for (int i = k; i < len - 1; i++)
                    {
                        _action_node_editors[i] = _action_node_editors[i + 1];
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
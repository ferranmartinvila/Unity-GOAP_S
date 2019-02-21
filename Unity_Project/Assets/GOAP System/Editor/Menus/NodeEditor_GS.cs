using UnityEngine;
using UnityEditor;
using GOAP_S.AI;
using System.Collections;

namespace GOAP_S.UI
{
    public class NodeEditor_GS : EditorWindow
    {
        //State fields
        private static string _id = ""; //Node ID used to set pop window id
        private static NodeEditor_GS _window; //Reference to this node editor window
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
        private int _action_node_editors_num = 0;

        /*Rect start_node;
        Rect window1;
        Rect window2;
        Rect window2_;*/



        //Window Menu Item ================
        [MenuItem("Tools / GOAP / Node Editor")]
        static void ShowNodeEditor()
        {
            _window = (NodeEditor_GS)EditorWindow.GetWindow(typeof(NodeEditor_GS));
            ConfigureWindow();
            
        }

        [MenuItem("Tools / GOAP / Node Editor", true)]
        static bool CheckAgentToShowNodeEditor()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Agent_GS>() != null;
        }

        //Loop Methods ====================
        public void Awake()
        {
            //Set the selected agent to draw its canvas
            //selected_agent = EditorWindow.GetWindow<Manager_GS>().target_obj.GetComponent<Agent_GS>();

            //Check if the selected agent is initialized
            //if (!selected_agent.GetAgentInit()) selected_agent.Initialize();

            //Iterate all the nodes and place them in the canvas
            /*start_node = new Rect(210, 10, 100, 100);
            window1 = new Rect(10, 10, 100, 100);
            window2 = new Rect(210, 210, 100, 100);
            window2_ = new Rect(210, 210, 100, 100);*/
        }
        
        void OnGUI()
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

            //If agent is null or there's no actions blit is avoided
            if (_selected_agent == null || _selected_agent.action_nodes == null)
            {
                //Get agent and return, so if is null is check in the next loop
                _selected_agent = _selected_object.GetComponent<Agent_GS>();
                return;
            }

            //Initialize necessary variables
            int num = _selected_agent.action_nodes_num;
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
        
            /*DrawNodeCurve(window1, window2); //curve is drawn under the windows
            DrawNodeCurve(start_node, window2); 
            DrawNodeCurve(window2_, window1); */
            /*
            //Planning
            GUI.backgroundColor = Color.white;
            window1 = GUI.Window(1, window1, DrawNodeWindow, "Window 1");   // Updates the Rect's when these are dragged
            window2 = GUI.Window(2, window2, DrawNodeWindow, "Window 2");
            window2_ = GUI.Window(4, window2_, DrawNodeWindow, "Window 7");
            */

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
                        for (int k = 0; k < num; k++)
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
            for (int k = 0; k < num; k++)
            {
                //Get the current action node
                ActionNode_GS node = ((ActionNode_GS)_selected_agent.action_nodes[k]);
                //Generate a node editor for the current node
                ActionNode_GS_Editor node_editor = new ActionNode_GS_Editor(node, this);
                //Generate the window
                Rect node_rect = GUILayout.Window(node.id, node.window_rect, node_editor.DrawUI, node.name, UI_configuration.node_window_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
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
            /*Blackboard_GS_Editor bb_editor = new Blackboard_GS_Editor(selected_agent.blackboard, this);
            bb_editor.window_position = new Vector2(_window.position.width - 250, 0);
            bb_editor.window_size = new Vector2(250, 100);
            GUILayout.Window(_selected_agent.blackboard.id, bb_editor.window, bb_editor.DrawUI, "Blackboard", UI_configuration.blackboard_window_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            */
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
        private static void ConfigureWindow()
        {
            _window.titleContent.text = "Node Editor"; //Set a window title
            _id = System.Guid.NewGuid().ToString(); //Generate window UUID
            _back_texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            _back_texture.SetPixel(0, 0, new Color(0.35f, 0.35f, 0.35f));
            _back_texture.Apply();
            _mouse_position = Event.current.mousePosition;
        }

        private void GenerateTargetAgentUI()
        {
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
        }

        public void AddTargetAgentNodeUI(ActionNode_GS new_node)
        {
            //TODO
        }

        public void RemoveTargetAgentNodeUI(ActionNode_GS_Editor target_node_editor)
        {
            //TODO
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

        public Vector2 mouse_position
        {
            get
            {
                return _mouse_position;
            }
        }
    }
}
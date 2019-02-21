using UnityEngine;
using UnityEditor;

namespace GOAP_S.UI
{
    public class NodeEditor_GS : EditorWindow
    {
        //State fields
        private static string _id = ""; //Node ID used to set pop window id
        private static NodeEditor_GS _window; //Reference to this node editor window
        //UI fields
        public UIConfig_GS UI_configuration = new UIConfig_GS(); //The UI configuration of the action node
        private static Texture2D _back_texture = null; //Texture in the background of the window
        //Target fields
        private GameObject _selected_object = null;
        private Agent_GS _selected_agent = null;

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

        //Window Configuration ============
        static void ConfigureWindow()
        {
            _window.titleContent.text = "Node Editor"; //Set a window title
            _id = System.Guid.NewGuid().ToString(); //Generate window UUID
            _back_texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            _back_texture.SetPixel(0, 0, new Color(0.35f, 0.35f, 0.35f));
            _back_texture.Apply();
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

        /*private void OnProjectChange()
        {
            //Node editor UI configuration is initialized on project change
            if(!UI_configuration.GetInitialized()) UI_configuration.InitializeConfig();
        }
        private void OnEnable()
        {
            //Node editor UI configuration is initialized on window enable
            if (!UI_configuration.GetInitialized()) UI_configuration.InitializeConfig();
        }*/

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
                }
                //Change the selected object and repaint the window content
                _selected_object = Selection.activeGameObject;
                Repaint();
            }

            //If agent is null or there's no actions blit is avoided
            if (_selected_agent == null || _selected_agent.action_nodes == null) return;

            //Initialize necessary variables
            int num = _selected_agent.action_nodes.Count;

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

            BeginWindows();

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

                //Midle click
                if(Event.current.button == 2)
                {
                    //Get mouse motion
                    float mouse_x_motion = Input.GetAxis("Mouse X");
                    float mouse_y_motion = Input.GetAxis("Mouse Y");
                    //Check if motion is not null
                    if (mouse_y_motion != 0.0f|| mouse_x_motion != 0.0f)
                    {
                        //Iterate all the nodes 
                        for (int k = 0; k < num; k++)
                        {
                            //Get the current action node
                            ActionNode_GS node = ((ActionNode_GS)_selected_agent.action_nodes[k]);
                            //Modify node position
                            node.window_position = new Vector2(node.window_position.x + mouse_x_motion, node.window_position.y + mouse_y_motion);
                        }
                    }
                }
            }

            //Draw action nodes
            //Here iterate all the nodes, now just draw them
            //Generate an output to know the node state is the next step
            for (int k = 0; k < num; k++)
            {
                //Get the current action node
                ActionNode_GS node = ((ActionNode_GS)_selected_agent.action_nodes[k]);
                //Generate a node editor for the current node
                ActionNode_GS_Editor node_editor = new ActionNode_GS_Editor(node, this);
                //Generate the window
                Rect node_rect = GUILayout.Window(node.id, node.window_rect, node_editor.DrawUI, node.name, UI_configuration.node_window_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                //Move the node if it's position is editable
                if (node.editable_position)
                {
                    node.window_position = new Vector2(node_rect.x, node_rect.y);
                }
            }

            //Draw agent blackboard
            Blackboard_GS_Editor bb_editor = new Blackboard_GS_Editor(selected_agent.blackboard, this);
            bb_editor.window_position = new Vector2(_window.position.width - 250, 0);
            bb_editor.window_size = new Vector2(250, 100);
            GUILayout.Window(_selected_agent.blackboard.id, bb_editor.window, bb_editor.DrawUI, "Blackboard", UI_configuration.blackboard_window_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            
            EndWindows();
        }

        void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 4);
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
    }
}
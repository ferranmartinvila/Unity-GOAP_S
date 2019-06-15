using UnityEngine;
using UnityEditor;
using GOAP_S.AI;
using GOAP_S.Tools;

namespace GOAP_S.UI
{
    [InitializeOnLoad]
    public sealed class NodePlanning_GS : ZoomableCanvas_GS
    {
        //Target fields
        private AgentBehaviour_GS_Editor _agent_behaviour_editor = null; //Editor of the focused agent blackboard
        private ActionNode_GS_Debugger[] _action_node_debuggers = null; //Array with all the target agent path action nodes debuggers
        private int _action_node_debuggers_num = 0; //Num of action node debuggers
        //Config fields
        private float refresh_rate = 0.5f;
        private float timer = 0.0f;

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
            base.OnInspectorUpdate();

            if(Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<Agent_GS>() == null)
            {
                //In null case set agent to null and reset
                ResetPlanChangeDelegate();
                _selected_agent = null;

                Repaint();

                return;
            }
            else if(_selected_agent != Selection.activeGameObject.GetComponent<Agent_GS>())
            {
                //Reset prev agent
                ResetPlanChangeDelegate();
                //Set selected agent
                _selected_agent = Selection.activeGameObject.GetComponent<Agent_GS>();
                //Set new agent delegate
                _selected_agent.on_agent_plan_change_delegate += () => UpdateAgentUI();
                //Generate selected agent UI
                GenerateTargetAgentUI();

                Repaint();
            }
        }

        private void OnEnable()
        {
            //Configure window on enable(title)
            ConfigureWindow();
            //Reset selection
            ResetPlanChangeDelegate();
            _selected_agent = null;
            //Check selected agent
            OnSelectionChange();
        }

        private void OnGUI()
        {
            //Draw background texture 
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), back_texture, ScaleMode.StretchToFill);

            //Check if there is an agent selected
            if (_selected_agent == null)
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
                    GUI.Label(new Rect(y, k, 120.0f, 25.0f), y + "||" + k, UIConfig_GS.left_white_style);
                }
            }

            //Mark the beginning area of the popup windows
            BeginWindows();

            if (_action_node_debuggers == null)
            {
                GenerateTargetAgentUI();
            }
            else
            {
                //Draw action nodes debuggers
                Color prev_color = GUI.color;
                GUISkin prev_skin = GUI.skin;
                GUI.skin = UIConfig_GS.debug_node_skin;
                Vector2 prev_middle_right = new Vector2();

                for (int i = 0 , y = 0; y < 18 && i < _action_node_debuggers.Length; y++)
                {
                    for (int x = 0; x < 8 && i < _action_node_debuggers.Length; x++, i++)
                    {
                        //Change window color depending of the state 
                        switch (_action_node_debuggers[i].target_action_node.action.state)
                        {
                            case Planning.Action_GS.ACTION_STATE.A_IDLE: GUI.color = new Color(0.2f, 0.2f, 0.2f, 1.0f); break;
                            case Planning.Action_GS.ACTION_STATE.A_START: GUI.color = new Color(0.6f, 1.0f, 0.4f, 1.0f); break;
                            case Planning.Action_GS.ACTION_STATE.A_UPDATE: GUI.color = new Color(0.0f, 0.7f, 1.0f, 1.0f); break;
                            case Planning.Action_GS.ACTION_STATE.A_COMPLETE: GUI.color = new Color(0.0f, 1.0f, 0.2f, 1.0f); break;
                        }

                        //Show node debugger window
                        Rect cur_window_rect = GUILayout.Window(_action_node_debuggers[i].window_uuid, new Rect(300.0f + 200.0f * x, 100.0f + 150.0f * y, 100.0f, 100.0f), _action_node_debuggers[i].DrawUI, "", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                        //Draw node connection
                        if (prev_middle_right != Vector2.zero)
                        {
                            Vector2 rect_middle_left = cur_window_rect.MiddleLeft();
                            if (y > x && x == 0)
                            {
                                Handles.DrawBezier(prev_middle_right, prev_middle_right + new Vector2(0.0f, 75.0f), prev_middle_right + new Vector2(50.0f, 0.0f), prev_middle_right + new Vector2(50.0f, 75.0f), Color.black, null, 2.0f);
                                Handles.DrawBezier(prev_middle_right + new Vector2(0.0f, 75.0f), rect_middle_left - new Vector2(0.0f, 75.0f), prev_middle_right + new Vector2(0.0f, 75.0f), rect_middle_left - new Vector2(0.0f, 75.0f), Color.black, null, 2.0f);
                                Handles.DrawBezier(rect_middle_left, rect_middle_left - new Vector2(0.0f, 75.0f), rect_middle_left - new Vector2(50.0f, 0.0f), rect_middle_left - new Vector2(50.0f, 75.0f), Color.black, null, 2.0f);
                            }
                            else
                            {
                                Handles.DrawBezier(prev_middle_right, rect_middle_left, prev_middle_right, rect_middle_left, Color.black, null, 2.0f);
                            }
                            //Draw arrow shape
                            Handles.DrawBezier(rect_middle_left, rect_middle_left + new Vector2(-5.0f, 5.0f), rect_middle_left, rect_middle_left + new Vector2(-5.0f, 5.0f), Color.black, null, 2.0f);
                            Handles.DrawBezier(rect_middle_left, rect_middle_left + new Vector2(-5.0f, -5.0f), rect_middle_left, rect_middle_left + new Vector2(-5.0f, -5.0f), Color.black, null, 2.0f);
                        }
                        prev_middle_right = cur_window_rect.MiddleRight();
                    }
                }

                GUI.skin = prev_skin;
                GUI.color = prev_color;
            }

            //Reset matrix to keep behaviour window and agent name scale 
            GUI.matrix = Matrix4x4.identity;

            //Selected agent name in zoom position coords
            GUI.Label(new Rect(_zoom_position.x + position.width * 0.5f, _zoom_position.y, 200.0f, 30.0f), "Agent: " + _selected_agent.name, UIConfig_GS.center_big_white_style);

            //Selection change can set the agent behaviour to null
            if (_agent_behaviour_editor == null)
            {
                UpdateAgentUI();
            }
            else
            {
                //Update behaviour window to simulate static position
                _agent_behaviour_editor.window_position = _zoom_position;

                //Draw agent behaviour editor
                GUILayout.Window(_agent_behaviour_editor.id, _agent_behaviour_editor.window, _agent_behaviour_editor.DrawUI, "Behaviour", UIConfig_GS.canvas_window_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
            //End area of popup windows
            EndWindows();

            //End zoomable layout area
            GUILayout.EndArea();

            //End zoomable layout
            EndZoomableLayout();

            //Handle input
            HandleInput();
        }

        private void Update()
        {
            //Screen refresh
            timer += Time.deltaTime;
            if (timer > refresh_rate)
            {
                timer = 0.0f;
                Repaint();
            }
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
                    PopupWindow.Show(new Rect(_mouse_pos.x, _mouse_pos.y, 0, 0), new NodePlanningPopMenu_GS());
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

        private void ResetPlanChangeDelegate()
        {
            if(_selected_agent != null)
            {
                _selected_agent.on_agent_plan_change_delegate = null;
            }
        }

        private void ConfigureWindow()
        {
            //Set a window title
            titleContent.text = "Planning";
            //Set window min size
            minSize = new Vector2(ProTools.MIN_CANVAS_WIDTH, ProTools.MIN_CANVAS_HEIGHT);
            //Set canvas size
            _canvas_size = new Vector2(ProTools.BEHAVIOUR_EDITOR_CANVAS_SIZE, ProTools.BEHAVIOUR_EDITOR_CANVAS_SIZE);
            //Set canvas camera initial position
            _zoom_position = new Vector2(ProTools.BEHAVIOUR_EDITOR_CANVAS_SIZE * 0.5f, ProTools.BEHAVIOUR_EDITOR_CANVAS_SIZE * 0.5f);
        }

        private void GenerateTargetAgentUI()
        {
            //First reset current data
            _agent_behaviour_editor = null;

            //Generate agent behaviour editor
            _agent_behaviour_editor = new AgentBehaviour_GS_Editor(_selected_agent);
            _agent_behaviour_editor.window_size = new Vector2(250.0f, 100.0f);

            //Generate current plan debugger
            UpdateAgentUI();
        }

        private void UpdateAgentUI()
        {
            if(_agent_behaviour_editor == null)
            {
                GenerateTargetAgentUI();
                return;
            }

            //First reset current data
            _action_node_debuggers = null;
            _action_node_debuggers_num = 0;

            //Check agent current plan
            ActionNode_GS [] plan = _selected_agent.current_plan.ToArray();
            ActionNode_GS[] completed_plan = _selected_agent.popped_actions.ToArray();
            if (plan == null || plan.Length == 0)
            {
                //Reset debug data
                _action_node_debuggers =  new ActionNode_GS_Debugger[0];
                _action_node_debuggers_num = 0;

                return;
            }

            //In valid plan case generate the plan debugger
            //Allocate node debuggers array
            _action_node_debuggers = new ActionNode_GS_Debugger[plan.Length + completed_plan.Length];
            _action_node_debuggers_num = plan.Length + completed_plan.Length;
            //Iterate completed plan nodes
            for(int k = 0; k < completed_plan.Length; k++)
            {
                //Generate plan action debugger
                ActionNode_GS_Debugger node_debugger = new ActionNode_GS_Debugger(completed_plan[k]);
                //Add the new debugger to the array
                _action_node_debuggers[k] = node_debugger;
            }
            //Iterate plan nodes
            for (int k = completed_plan.Length; k < _action_node_debuggers_num; k++)
            {
                //Generate plan action debugger
                ActionNode_GS_Debugger node_debugger = new ActionNode_GS_Debugger(plan[k - completed_plan.Length]);
                //Add the new debugger to the array
                _action_node_debuggers[k] = node_debugger;
            }
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
                    _back_texture.SetPixel(0, 0, new Color(0.5f, 0.5f, 0.5f));
                    _back_texture.Apply();
                }
                return _back_texture;
            }
        }
    }
}

using UnityEngine;
using UnityEditor;
using GOAP_S.AI;
using GOAP_S.Tools;

namespace GOAP_S.UI
{
    public class AgentBehaviour_GS_Editor
    {
        //UI fields
        private Rect _window = Rect.zero; //Rect used to place the editor window
        //Content fields
        private Agent_GS _target_agent = null; //The agent is the editor pointing
        private string _id = null; //Window UUID

        //Constructor =================
        public AgentBehaviour_GS_Editor(Agent_GS _agent)
        {
            //Set target agent
            _target_agent = _agent;
        }

        //Loop Methods ================
        public void DrawUI(int id)
        {
            //Separaion between title and variables
            GUILayout.BeginVertical();
            GUILayout.Space(15);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.EndVertical();

            //No behaviour selected case
            if (_target_agent.behaviour == null)
            {
                GUILayout.Label("No Behaviour", UIConfig_GS.center_big_white_style);
                //Behaviour select button
                if (GUILayout.Button(new GUIContent("Set Behaviour","Choose a behaviour script from the assets"), UIConfig_GS.Instance.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new BehaviourSelectMenu_GS(_target_agent));
                }
            }
            //Behaviour selected case
            else
            {
                //Behaviour name
                GUILayout.BeginHorizontal("HelpBox");
                GUILayout.FlexibleSpace();
                GUILayout.Label(_target_agent.behaviour.name, UIConfig_GS.Instance.node_elements_style, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                //Behaviour select button
                if (GUILayout.Button(new GUIContent("Change Behaviour", "Choose a behaviour script from the assets"), UIConfig_GS.Instance.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new BehaviourSelectMenu_GS(_target_agent));
                }

                //Behaviour edit button
                if (GUILayout.Button(new GUIContent("Edit Behaviour", "Open the currently selected behaviour script with the code editor"), UIConfig_GS.Instance.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                {
                    ProTools.OpenScriptEditor(_target_agent.behaviour.GetType());
                }
            }

            //Separaion between behaviour script and idle action script
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            //Idle action title
            GUILayout.Label(new GUIContent("Idle Action", "Action that will be executed when there is no planning action to execute"), UIConfig_GS.center_big_style);
            //Line separator
            GUILayout.Space(-5);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.EndVertical();



            //No idle action selected case
            if (_target_agent.idle_action == null)
            {
                GUILayout.Label("No Action", UIConfig_GS.center_big_white_style);
                //Action select dropdown
                if (GUILayout.Button("Set Action", UIConfig_GS.Instance.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new ActionSelectMenu_GS(null));
                }
            }
            //Idle action selected case
            else
            {
                //Action name
                GUILayout.BeginHorizontal("HelpBox");
                GUILayout.FlexibleSpace();
                GUILayout.Label(_target_agent.idle_action.name, UIConfig_GS.Instance.node_elements_style, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                //Action select dropdown
                if (GUILayout.Button("Change Action", UIConfig_GS.Instance.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new ActionSelectMenu_GS(null));
                }

                //Behaviour edit button
                if (GUILayout.Button("Edit Action", UIConfig_GS.Instance.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                {
                    ProTools.OpenScriptEditor(_target_agent.idle_action.GetType());
                }
            }
        }

        //Get/Set methods =============
        public int id
        {
            get
            {
                if(string.IsNullOrEmpty(_id))
                {
                    _id = System.Guid.NewGuid().ToString();
                }
                return _id.GetHashCode();
            }
        }

        public Rect window
        {
            get
            {
                return _window;
            }
            set
            {
                _window = value;
            }
        }

        public Vector2 window_size
        {
            get
            {
                return new Vector2(_window.width, _window.height);
            }
            set
            {
                _window.width = value.x;
                _window.height = value.y;
            }
        }

        public Vector2 window_position
        {
            get
            {
                return new Vector2(_window.x, _window.y);
            }
            set
            {
                _window.x = value.x;
                _window.y = value.y;
            }
        }
    }
}

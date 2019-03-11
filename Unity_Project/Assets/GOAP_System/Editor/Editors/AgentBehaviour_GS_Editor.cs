using UnityEngine;
using UnityEditor;
using GOAP_S.AI;
using GOAP_S.Tools;

namespace GOAP_S.UI
{
    public class AgentBehaviour_GS_Editor
    {
        //UI fields
        private Rect _window = Rect.zero; //Rect used to place bb window
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
                if (GUILayout.Button("Set Behaviour", UIConfig_GS.Instance.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new BehaviourSelectMenu_GS(_target_agent));
                }
            }
            //Behaviour selected case
            else
            {
                GUILayout.BeginHorizontal("HelpBox");
                GUILayout.FlexibleSpace();
                GUILayout.Label(_target_agent.behaviour.name, UIConfig_GS.Instance.node_elements_style, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                //Behaviour select button
                if (GUILayout.Button("Change Behaviour", UIConfig_GS.Instance.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new BehaviourSelectMenu_GS(_target_agent));
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

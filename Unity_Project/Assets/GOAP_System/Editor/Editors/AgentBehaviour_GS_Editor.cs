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
            //Update position
            /*if (window_position.x != NodePlanning_GS.Instance.position.x)
            {
                window_position = new Vector2(NodePlanning_GS.Instance.position.x, 0.0f);
            }*/

            //Separaion between title and variables
            GUILayout.BeginVertical();
            GUILayout.Space(15);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.EndVertical();
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

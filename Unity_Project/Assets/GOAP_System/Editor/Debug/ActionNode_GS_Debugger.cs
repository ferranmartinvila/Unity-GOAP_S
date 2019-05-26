using UnityEngine;
using GOAP_S.AI;

namespace GOAP_S.UI
{
    public class ActionNode_GS_Debugger
    {
        //Target fields
        private ActionNode_GS _target_action_node = null;
        private int _window_uuid = 0;

        //Constructors ================
        public ActionNode_GS_Debugger(ActionNode_GS new_target)
        {
            //Set targets
            _target_action_node = new_target;
            //Generate uuid
            _window_uuid = System.Guid.NewGuid().ToString().GetHashCode();
        }

        //Loop Methods ================
        public void DrawUI(int id)
        {
            //Action node name
            GUILayout.Label(_target_action_node.name);
            //Action name
            GUILayout.Label(_target_action_node.action.name);
            //Draw aciton debug info
            _target_action_node.action.BlitDebugUI();
        }

        //Get/Set Methods =============
        public int window_uuid
        {
            get
            {
                return _window_uuid;
            }
        }

        public ActionNode_GS target_action_node
        {
            get
            {
                return _target_action_node;
            }
        }
    }
}

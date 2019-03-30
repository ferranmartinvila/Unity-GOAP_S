using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP_S.AI;

namespace GOAP_S.UI
{
    public class ActionNode_GS_Debugger
    {
        //Target fields
        private ActionNode_GS _target_action_node = null;

        //Constructors ================
        public ActionNode_GS_Debugger(ActionNode_GS new_target)
        {
            //Set targets
            _target_action_node = new_target;
        }

        //Loop Methods ================
        public void DrawUI(int id)
        {
            GUILayout.Label(_target_action_node.action.name);
        }
    }
}

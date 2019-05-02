using UnityEngine;
using GOAP_S.Blackboard;
using UnityEditor;

namespace GOAP_S.UI
{
    [CustomEditor(typeof(GlobalBlackboardComp_GS))]
    public class GlobalBlackboardComp_GS_Editor : BlackboardComp_GS_Editor
    {
        public override void OnInspectorGUI()
        {
            //Target blackboard
            Blackboard_GS target_blackboard = ((GlobalBlackboardComp_GS)target).blackboard;

            //Display global blackboard
            GUILayout.Label("Global Variables", UIConfig_GS.left_big_style);
            DisplayBlackboardVariables(GlobalBlackboard_GS.blackboard);
        }
    }
}

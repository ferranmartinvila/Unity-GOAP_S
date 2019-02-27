using UnityEngine;
using UnityEditor;
using GOAP_S.Blackboard;

namespace GOAP_S.UI
{
    [CustomEditor(typeof(BlackboardComp_GS))]
    public class BlackboardComp_GS_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            //Blackboard component can be removed immediately so we need to check if still exists 
            if (((BlackboardComp_GS)target).agent == null)
            {
                EditorApplication.delayCall += () => DestroyImmediate(target);
                return;
            }

            //Focus target blackboard
            Blackboard_GS target_bb = ((BlackboardComp_GS)target).blackboard;

            //Varaibles list
            GUILayout.BeginVertical();
            foreach (Variable_GS variable in target_bb.variables.Values)
            {
                GUILayout.Label(variable.name);
            }
            GUILayout.EndVertical();
        }
    }
}
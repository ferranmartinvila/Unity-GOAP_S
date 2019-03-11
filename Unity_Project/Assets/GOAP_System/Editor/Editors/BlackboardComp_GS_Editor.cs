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
            Blackboard_GS target_blackboard = ((BlackboardComp_GS)target).blackboard;

            //Varaibles list
            GUILayout.BeginVertical();
            foreach (Variable_GS variable in target_blackboard.variables.Values)
            {
                GUILayout.BeginHorizontal();

                //Show variable type
                GUILayout.Label(variable.type.ToString().Replace('_', ' '), UIConfig_GS.Instance.left_bold_style, GUILayout.MaxWidth(40.0f));

                //Show variable name
                GUILayout.Label(variable.name, GUILayout.MaxWidth(100.0f));

                if (!variable.is_binded)
                {
                    GUILayout.Label(variable.value.ToString(), GUILayout.MaxWidth(100.0f));
                }
                else
                {
                    GUILayout.Label(variable.display_field_short_path, GUILayout.MaxWidth(100.0f));
                }

                //Free space margin
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    //Delete variable
                    EditorApplication.delayCall += () => target_blackboard.RemoveVariable(variable.name);
                    //Delete variable editor
                    EditorApplication.delayCall += () => NodeEditor_GS.Instance.blackboard_editor.DeleteVariableEditor(variable.name);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}
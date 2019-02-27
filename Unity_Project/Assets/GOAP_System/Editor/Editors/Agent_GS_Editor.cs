using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using GOAP_S.AI;

namespace GOAP_S.UI
{
    [CustomEditor(typeof(Agent_GS))]
    public class Agent_GS_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            //Fields
            ((Agent_GS)target).name = EditorGUILayout.TextField("Name", ((Agent_GS)target).name);

            //Editor Button
            if (GUILayout.Button("Open Editor"))
            {
                //Open the editor menu in the desired path (no spaces in the path)
                EditorApplication.ExecuteMenuItem("Tools/GOAP/Node Editor");
            }

            //Remove button
            GUI.backgroundColor = new Color(1.0f, 0.2f, 0.2f, 1.0f);
            if (GUILayout.Button("Remove Agent", GUILayout.ExpandWidth(true)))
            {
                //Destroy agent
                DestroyImmediate((Agent_GS)target);
                //Blackboard will detect that there's no agent and will destroy itself
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            //Check if blackboard existst (the real use is to creat a new bb component when is destroyed if the agent component is still alive)
            if (((Agent_GS)target).blackboard == null)
            {
                Debug.LogWarning("Blackboard lost!");
            }
        }
    }
}

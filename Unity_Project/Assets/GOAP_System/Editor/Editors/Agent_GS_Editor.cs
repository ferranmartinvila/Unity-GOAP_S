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
                //Add remove this agent to accept menu delegates callback
                SecurityAcceptMenu_GS.on_accept_delegate += () => DestroyImmediate((Agent_GS)target);
                //Add mark scene dirty to accept menu delegates calback
                SecurityAcceptMenu_GS.on_accept_delegate += () => EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                //Blackboard will detect that there's no agent and will destroy itself

                //Get mouse current position
                Vector2 mousePos = Event.current.mousePosition;
                //Open security accept menu on mouse position
                PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new SecurityAcceptMenu_GS());
            }

            //Check if blackboard existst (the real use is to creat a new bb component when is destroyed if the agent component is still alive)
            if (((Agent_GS)target).blackboard == null)
            {
                Debug.LogWarning("Blackboard lost!");
            }
        }
    }
}

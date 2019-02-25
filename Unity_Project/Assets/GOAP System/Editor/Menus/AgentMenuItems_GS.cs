using UnityEditor;
using GOAP_S.AI;

namespace GOAP_S.UI
{
    public class AgentMenuItems_GS
    {

        //Show Planning Item ==============
        [MenuItem("Tools / GOAP / Selected Agent / Show Planning")]
        static void ShowPlanning()
        {
            NodeEditor_GS editor = (NodeEditor_GS)EditorWindow.GetWindow(typeof(NodeEditor_GS));
        }

        [MenuItem("Tools / GOAP / Selected Agent / Show Planning", true)]
        static bool CheckSelectedObjectToShowPlanning()
        {
            return (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Agent_GS>() != null);
        }

        //Clear Planning Item =============
        [MenuItem("Tools / GOAP / Selected Agent / Clear")]
        static void ClearAgentPlanning()
        {
            //Clear agent planning
            Agent_GS agent = Selection.activeGameObject.GetComponent<Agent_GS>();
            agent.ClearPlanning();
            //Clear node editor planning
            NodeEditor_GS editor = (NodeEditor_GS)EditorWindow.GetWindow(typeof(NodeEditor_GS));
            editor.ClearPlanning();
        }

        [MenuItem("Tools / GOAP / Selected Agent / Clear", true)]
        static bool CheckSelectedObjectToClear()
        {
            return Selection.activeGameObject != null
                && Selection.activeGameObject.GetComponent<Agent_GS>() != null
                && Selection.activeGameObject.GetComponent<Agent_GS>().action_nodes.Length > 0;
        }
    }
}

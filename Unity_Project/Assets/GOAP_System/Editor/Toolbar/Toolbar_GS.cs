using UnityEditor;
using GOAP_S.AI;
using UnityEngine;

namespace GOAP_S.UI
{
    public class Toolbar_GS
    {
        ///Node Editor --------------------------
        //Node Editor Menu Item ================
        [MenuItem("Tools / GOAP / Node Editor")]
        static void ShowNodeEditor()
        {
            EditorWindow.GetWindow(typeof(NodeEditor_GS));
        }

        ///Info ---------------------------------
        //Github Link =====================
        [MenuItem("Tools / GOAP / Github")]
        static void LinkGithub()
        {
            Application.OpenURL("https://github.com/ferranmartinvila/PRE_TFG_");
        }

        ///Selected Agent -----------------------
        //Show Planning Item ==============
        [MenuItem("Tools / GOAP / Selected Agent / Show Planning")]
        static void ShowPlanning()
        {
            NodeEditor_GS.Instance = (NodeEditor_GS)EditorWindow.GetWindow(typeof(NodeEditor_GS));
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

        //Add Agent Item ==============
        [MenuItem("Tools / GOAP / Add Agent")]
        static void AddAgent()
        {
            Selection.activeGameObject.AddComponent<Agent_GS>();
        }

        [MenuItem("Tools / GOAP / Add Agent", true)]
        static bool CheckAddAgent()
        {
            return Selection.activeGameObject != null &&  Selection.activeGameObject.GetComponent<Agent_GS>() == null;
        }

        //Remove Agent Item ===========
        [MenuItem("Tools / GOAP / Selected Agent / Remove")]
        static void RemoveAgent()
        {
            EditorApplication.delayCall += () =>  Object.DestroyImmediate(Selection.activeGameObject.GetComponent<Agent_GS>());
        }

        [MenuItem("Tools / GOAP / Selected Agent / Remove", true)]
        static bool CheckRemoveAgent()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Agent_GS>() != null;
        }
    }
}

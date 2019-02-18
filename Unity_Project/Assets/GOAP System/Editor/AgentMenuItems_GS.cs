using UnityEditor;


public class AgentMenuItems_GS {

    //EditorGUIUtility.ShowObjectPicker<MonoScript>(action, false, "", EditorGUIUtility.GetControlID(FocusType.Passive) + 100);
    //EditorGUILayout.ObjectField(action, typeof(Action_GS), true);


    /*[MenuItem("Tools / GOAP / Selected Agent /")]
    static void test()
    {
        //return (Selection.activeGameObject.GetComponent<Agent_GS>() != null);
    }

    [MenuItem("Tools / GOAP / Selected Agent /",true)]
    static bool CheckSelectedAgentToEnableItems()
    {
        return (Selection.activeGameObject.GetComponent<Agent_GS>() != null);
    }*/

    //Show Planning Item ==============
    [MenuItem ("Tools / GOAP / Selected Agent / Show Planning")]
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
    [MenuItem ("Tools / GOAP / Selected Agent / Clear")]
    static void ClearAgentPlanning()
    {
        Agent_GS agent = Selection.activeGameObject.GetComponent<Agent_GS>();
        agent.ClearPlanning();
    }

    [MenuItem("Tools / GOAP / Selected Agent / Clear", true)]
    static bool CheckSelectedObjectToClear()
    {
        return Selection.activeGameObject != null 
            && Selection.activeGameObject.GetComponent<Agent_GS>() != null
            && Selection.activeGameObject.GetComponent<Agent_GS>().list_action_nodes.Count > 0;
    }
}

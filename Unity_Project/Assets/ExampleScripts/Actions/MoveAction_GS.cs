using UnityEngine;
using UnityEditor;
using GOAP_S.Planning;

public class MoveAction_GS : Action_GS
{
    //Fields
    public float speed = 5.0f;
    public Vector3 target_position = Vector3.zero;

    private Vector3 direction_vector = Vector3.zero;

    //Called on the first action loop
    public override ACTION_RESULT ActionStart()
    {
        direction_vector = target_position - agent.transform.position;
        return ACTION_RESULT.A_NEXT;
    }

    //Called on the action update
    public override ACTION_RESULT ActionUpdate()
    {
        agent.transform.position = new Vector3(agent.transform.position.x + direction_vector.x * speed * Time.deltaTime, agent.transform.position.y + direction_vector.y * speed * Time.deltaTime, agent.transform.position.z + direction_vector.z * speed * Time.deltaTime);

        if (Vector3.Distance(agent.transform.position, target_position) < 0.1f)
        {
            return ACTION_RESULT.A_NEXT;
        }
        else
        {
            return ACTION_RESULT.A_CURRENT;
        }
    }

    //Called when the action ends correctly
    public override ACTION_RESULT ActionEnd()
    {
        return ACTION_RESULT.A_NEXT;
    }

    /*
    Called when the action process ends with errors
    When an action ends with errors the agent dont look for the next action connected with this
    The actions path is recalculed from the start action node
    */
    public override void ActionBreak()
    {

    }

    public override void BlitUI()
    {
        /*EditorGUIUtility.ShowObjectPicker<GameObject>(target_obj, true, "", EditorGUIUtility.GetControlID(FocusType.Passive) + 100);

        if (Event.current.commandName == "ObjectSelectorClosed")
        {
            Debug.Log(EditorGUIUtility.GetObjectPickerObject());
        }*/

        //EditorGUI.ObjectField(new Rect(10, 20, 100, 20), target_obj,typeof(GameObject));
        GUILayout.BeginVertical();

        GUILayout.Label("Direction: " + direction_vector);
        GUILayout.Label("Distance: " + Vector3.Distance(agent.transform.position, target_position));

        GUILayout.EndVertical();
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetTest(int k, int js, int ds, float j, string d)
    {
        return k;
    }
}

using UnityEngine;
using UnityEditor;
using GOAP_S.Planning;

public class MoveAction_GS : Action_GS
{
    public float time = 0.2f;
    private float timer = 0.0f;

    public override ACTION_RESULT ActionUpdate()
    {
        Debug.Log(agent.name);

        timer += Time.deltaTime;
        if (timer > time)
        {
            return ACTION_RESULT.A_NEXT;
        }

        return ACTION_RESULT.A_CURRENT;
    }
    /*//Fields
    public float speed = 5.0f;
    public Vector3 target_position = Vector3.zero;

    private Vector3 direction_vector = Vector3.zero;

    //Called on the first action loop
    public override ACTION_RESULT ActionStart()
    {
        direction_vector = Vector3.Normalize(target_position - agent.transform.position);

        return ACTION_RESULT.A_NEXT;
    }

    //Called on the action update
    public override ACTION_RESULT ActionUpdate()
    {
        agent.transform.position += direction_vector * speed * Time.deltaTime;

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

    
    //Called when the action process ends with errors
    //When an action ends with errors the agent dont look for the next action connected with this
    //The actions path is recalculed from the start action node
    
    public override void ActionBreak()
    {

    }

    public override void BlitUI()
    {
        /*EditorGUIUtility.ShowObjectPicker<GameObject>(target_obj, true, "", EditorGUIUtility.GetControlID(FocusType.Passive) + 100);

        if (Event.current.commandName == "ObjectSelectorClosed")
        {
            Debug.Log(EditorGUIUtility.GetObjectPickerObject());
        }

        //EditorGUI.ObjectField(new Rect(10, 20, 100, 20), target_obj,typeof(GameObject));
        GUILayout.BeginVertical();

        GUILayout.Label("Direction: " + direction_vector);
        //GUILayout.Label("Distance: " + Vector3.Distance(agent.transform.position, target_position));

        GUILayout.EndVertical();
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetTest(int k, int js, int ds, float j, string d)
    {
        return j * k;
    }*/
}

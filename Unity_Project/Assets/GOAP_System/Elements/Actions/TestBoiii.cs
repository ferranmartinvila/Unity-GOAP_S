using UnityEngine;


public class TestBoiii : Action_GS
    {
        public GameObject target_obj = null;


    public TestBoiii()
    {
        Debug.Log("CALLED");
    }

        //Called on the first action loop
        public override ACTION_RESULT ActionStart()
        {
            return ACTION_RESULT.CONTINUE;
        }

        //Called on the action update
        public override ACTION_RESULT ActionUpdate()
        {
            target_obj.transform.position.Set(target_obj.transform.position.x + 0.5f, target_obj.transform.position.y, target_obj.transform.position.z);

            return ACTION_RESULT.CONTINUE;
        }

        //Called when the action ends correctly
        public override void ActionEnd()
        {

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
            GUILayout.Label("TEST ADDED!");
            //EditorGUI.ObjectField(new Rect(10, 20, 100, 20), target_obj,typeof(GameObject));
        }
    }

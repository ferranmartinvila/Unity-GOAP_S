using UnityEngine;
using GOAP_S.PT;

namespace GOAP_S.Planning
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = true)]
    public class Action_Attribute_GS : System.Attribute
    {

    }

    [Action_Attribute_GS]
    //Actions inherit from this class and can manage all kind of scene data
    public abstract class Action_GS
    {
        //Content fields
        [SerializeField] private string _name = "no_name";
        /*
        Action update return the state of the action process
        ActionStart also uses this enum, 
        so the user can check if the action can be executed with no problem
        */
        public enum ACTION_RESULT
        {
            ERROR = 0,
            CONTINUE = 1,
            END = 2
        }

        //Called on the first action loop
        public abstract ACTION_RESULT ActionStart();

        //Called on the action update
        public abstract ACTION_RESULT ActionUpdate();

        //Called when the action ends correctly
        public abstract void ActionEnd();

        /*
        Called when the action process ends with errors
        When an action ends with errors the agent dont look for the next action connected with this
        The actions path is recalculed from the start action node
        */
        public abstract void ActionBreak();

        /*
        Blit action UI inside the action node
         */
        public abstract void BlitUI();

        //Get/Set Methods =================
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
    }
}
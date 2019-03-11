using UnityEngine;
using System;
using GOAP_S.Blackboard;
using GOAP_S.AI;

namespace GOAP_S.Planning
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class Action_Attribute_GS : Attribute
    {

    }

    //Actions inherit from this class and can manage all kind of scene data
    [Action_Attribute_GS]
    public class Action_GS
    {

        /*
        Actions have process state
        This state basically for user information
        */
        public enum ACTION_STATE
        {
            A_IDLE,
            A_START,
            A_UPDATE,
            A_COMPLETE
        }

        /*
        Action update return the state of the action process
        ActionStart also uses this enum, 
        so the user can check if the action can be executed with no problem
        */
        public enum ACTION_RESULT
        {
            A_ERROR = 0,
            A_CONTINUE = 1,
            A_END = 2
        }

        //Content fields
        [SerializeField] private string _name = "no_name";
        [NonSerialized] private ACTION_STATE _action_state = ACTION_STATE.A_IDLE;
        [NonSerialized] private Agent_GS _agent = null;

        //Constructors ================
        public Action_GS()
        {
            //Set action target agent to null
            _agent = null;
        }

        //Loop Methods ================
        //Action awake is called once at the app start or when the agent is spawned
        public virtual bool ActionAwake()
        {
            return true;
        }

        //Called on the first action loop
        public virtual bool ActionStart()
        {
            return true;
        }

        //Called on the action update
        public virtual ACTION_RESULT ActionUpdate()
        {
            return ACTION_RESULT.A_END;
        }

        //Called when the action ends correctly
        public virtual void ActionEnd()
        {

        }

        /*
        Called when the action process ends with errors
        When an action ends with errors the agent dont look for the next action connected with this
        The actions path is recalculed from the start action node
        */
        public virtual void ActionBreak()
        {
            _action_state = ACTION_STATE.A_COMPLETE;
        }


        //Blit action UI inside the action node
        public virtual void BlitUI()
        {

        }

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

        public Blackboard_GS blackboard
        {
            get
            {
                return _agent.blackboard;
            }
        }

        public Agent_GS agent
        {
            get
            {
                return _agent;
            }
            set
            {
                _agent = value;
            }
        }
    }
}
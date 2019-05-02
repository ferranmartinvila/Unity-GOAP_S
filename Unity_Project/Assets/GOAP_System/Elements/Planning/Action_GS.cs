using UnityEngine;
using System;
using GOAP_S.Blackboard;
using GOAP_S.AI;

namespace GOAP_S.Planning
{
    //Action class attribute
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class Action_Attribute_GS : Attribute
    {

    }

    //Actions have a custom class attribute usefull to detect action derived classes
    [Action_Attribute_GS, Serializable]
    public class Action_GS
    {

        /*
        Actions have process state
        This state is basically for user information
        */
        public enum ACTION_STATE
        {
            A_IDLE = 0,
            A_AWAKE,
            A_START,
            A_UPDATE,
            A_COMPLETE
        }

        /*
        Action loop methods return the state of the action process
        So the user can check if the action has been executed correctly
        - Error: Error on loop method and action end. Ex. Update -> End
        - Current: In the next iteration the action will execute the same current loop method. Ex. Update -> Update
        - Next: In the next iteration the action will execute the next loop method. Ex. Awake -> Start
        */
        public enum ACTION_RESULT
        {
            A_ERROR = 0, //Action loop method
            A_CURRENT,
            A_NEXT
        }

        //Content fields
        [SerializeField] private string _name = "no_name";
        [SerializeField] private int _cost = 1;
        [NonSerialized] private ACTION_STATE _state = ACTION_STATE.A_IDLE;
        [NonSerialized] private Agent_GS _agent = null;

        //Constructors ================
        public Action_GS()
        {
            //Set action target agent to null
            _agent = null;
        }

        //Loop Methods ================
        //Action awake is called once at the app start or when the agent is spawned
        public virtual ACTION_RESULT ActionAwake()
        {
            return ACTION_RESULT.A_NEXT;
        }

        //Called on the first action loop
        public virtual ACTION_RESULT ActionStart()
        {
            return ACTION_RESULT.A_NEXT;
        }

        //Called on the action update
        public virtual ACTION_RESULT ActionUpdate()
        {
            return ACTION_RESULT.A_NEXT;
        }

        //Called when the action ends correctly
        public virtual ACTION_RESULT ActionEnd()
        {
            return ACTION_RESULT.A_NEXT;
        }

        /*
        Called when the action process ends with errors
        When an action ends with errors the agent dont look for the next action connected with this
        The actions path is recalculed from the start action node
        */
        public virtual void ActionBreak()
        {
            _state = ACTION_STATE.A_COMPLETE;
        }

        //Used to iterate all the previous loop methods
        public ACTION_RESULT Execute()
        {
            //Action execution result
            Action_GS.ACTION_RESULT execution_result = Action_GS.ACTION_RESULT.A_ERROR;
            //Execute the current action
            switch (_state)
            {
                case Action_GS.ACTION_STATE.A_AWAKE:
                    {
                        execution_result = ActionAwake();
                    }
                    break;
                case Action_GS.ACTION_STATE.A_START:
                    {
                        execution_result = ActionStart();
                    }
                    break;
                case Action_GS.ACTION_STATE.A_UPDATE:
                    {
                        //Execute behaviour in action update
                        execution_result = ActionUpdate();
                    }
                    break;
                case Action_GS.ACTION_STATE.A_COMPLETE:
                    {
                        execution_result = ActionEnd();
                    }
                    break;
            }
            //React with the action result
            if (execution_result == Action_GS.ACTION_RESULT.A_NEXT)
            {
                _state += 1;
            }
            else if (execution_result == Action_GS.ACTION_RESULT.A_ERROR)
            {
                ActionBreak();
            }

            return execution_result;
        }

        //Blit action UI inside the action node
        public virtual void BlitUI()
        {

        }

        //Get/Set Methods =================
        [BlockedProperty_GS]
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

        public int cost
        {
            get
            {
                return _cost;
            }
            set
            {
                _cost = value;
            }
        }

        public ACTION_STATE state
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        [BlockedProperty_GS]
        public Blackboard_GS blackboard
        {
            get
            {
                return _agent.blackboard;
            }
        }

        [BlockedProperty_GS]
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
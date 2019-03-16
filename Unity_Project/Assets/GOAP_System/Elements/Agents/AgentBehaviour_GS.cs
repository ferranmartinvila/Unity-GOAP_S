using UnityEngine;
using System;
using GOAP_S.Blackboard;
using GOAP_S.Tools;
using GOAP_S.Planning;

namespace GOAP_S.AI
{
    //AgentBehaviour class attribute
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class AgentBehaviour_Attribute_GS: Attribute
    {

    }

    //Agent behaviour have a custom class attribute usefull to detect agent behaviour derived classes
    [AgentBehaviour_Attribute_GS]
    public class AgentBehaviour_GS
    {
        //Content fields
        [NonSerialized] private Agent_GS _agent = null; //The agent is under control of this behaviour
        [SerializeField] private string _name = null; //Behaviour name

        //Loop Methods ================
        public virtual void Start()
        {
            //Agent behaviour can initialize necessary stuff at start
        }

        public virtual void Update()
        {
            //Here we code the actions the agent should do depending of the results/state
        }

        //Functionality Methods =======
        protected void SetGoal(string variable_name,OperatorType operator_type, object value)
        {
            //Check of the new goal value uses the correct type
            if (value.GetType() != _agent.blackboard.GetValue<object>(variable_name).GetType())
            {
                //In different types case the goal is not valid
                Debug.LogError("Behaviour " + _name + " is trying to set a " + value.GetType() + "goal to a " + _agent.blackboard.GetValue<object>(variable_name).GetType() + "variable");
            }
            else
            {
                //In correct goal case we simply add it to the goal world state
                agent.goal_world_state.Add(variable_name, new Property_GS(variable_name,value.GetType().ToString().ToVariableType(), operator_type, value));
            }
        }

        protected void RemoveGoal(string variable_name)
        {
            agent.goal_world_state.Remove(variable_name);
        }

        protected void ClearAllGoals()
        {
            agent.goal_world_state.Clear();
        }

        protected void AbortPlan()
        {
            //TODO
        }

        //Get/Set Methods =============
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

        public Blackboard_GS blackboard
        {
            get
            {
                return _agent.blackboard;
            }
        }
    }
}

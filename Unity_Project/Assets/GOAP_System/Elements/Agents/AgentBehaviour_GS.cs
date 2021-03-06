﻿using UnityEngine;
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
            //Idle 
            //Here we code the actions that the agent should do depending of the results/state generated from the previous actions
        }

        public virtual bool InActionUpdate(Action_GS.ACTION_RESULT current_action_result, Action_GS.ACTION_STATE current_action_state)
        {
            //InAction
            //Here we code the actions that the agent should do depending of the results/state generated from the previous actions
            return true;
        }

        //Functionality Methods =======
        protected void SetGoal(string variable_name, OperatorType operator_type, object variable_value, VariableLocation location = VariableLocation._local)
        {
            //First check if the variable exists in the current world state
            Variable_GS target_variable = null;
            if (location == VariableLocation._local)
            {
                target_variable = _agent.blackboard.GetObjectVariable(variable_name);
            }
            else
            {
                target_variable = GlobalBlackboard_GS.blackboard.GetObjectVariable(variable_name);
            }

            if (target_variable == null)
            {
                //In null case goal set is cancelled
                Debug.LogError("Variable: " + variable_name + " does not exist and can't be setted as goal");
                return;
            }

            //Check of the new goal value uses the correct type
            if (variable_value.GetType() != target_variable.value.GetType())
            {
                //In different types case the goal is not valid
                Debug.LogError("Behaviour " + _name + " is trying to set a " + variable_value.GetType() + "goal to a " + target_variable.value.GetType() + "variable");
                return;
            }

            string prefixed_name = (location == VariableLocation._local ? "Local/" : "Global/") + variable_name;

            //Check if the new goal is already defined in the goal world state
            Property_GS already_existing_property = null;
            if (agent.goal_world_state.properties.TryGetValue(prefixed_name, out already_existing_property))
            {
                //If the goal already exists we only change the goal value
                already_existing_property.value = variable_value;
            }
            else
            {
                //In correct and new goal case we simply add it to the goal world state
                agent.goal_world_state.SetGoal(prefixed_name, new Property_GS(prefixed_name, variable_value.GetType().ToVariableType(), operator_type, variable_value, target_variable.planning_value));
            }
        }
        protected void RemoveGoal(string variable_name)
        {
            agent.goal_world_state.RemoveGoal(variable_name);
        }

        protected void ClearAllGoals()
        {
            agent.goal_world_state.ClearAllGoals();
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

        public Blackboard_GS global_blackboard
        {
            get
            {
                return GlobalBlackboard_GS.blackboard;
            }
        }
    }
}

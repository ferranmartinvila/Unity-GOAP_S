using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using GOAP_S.Planning;
using GOAP_S.AI;
using GOAP_S.Tools;
using System;

namespace GOAP_S.Blackboard
{
    [Serializable]
    public class Blackboard_GS
    {
        //Content fields
        [NonSerialized] private Agent_GS _target_agent = null; //The agent is this blackboard pointing
        [SerializeField] private Dictionary<string, Variable_GS> _variables = new Dictionary<string, Variable_GS>(); //Variables of the agent

        //Constructors ================
        public Blackboard_GS(Agent_GS new_target_agent)
        {
            //The agent is this blackboard pointing
            _target_agent = new_target_agent;
        }

        //Varibles Methods ============
        public Variable_GS AddVariable(string name, VariableType type, object value)
        {
            //Check if exists a variable with the same name
            Variable_GS old_variable;
            if (variables.TryGetValue(name,out old_variable))
            {
                Debug.LogWarning("There is already a variable named " + name + " !");
                return null;
            }

            //Generate the new variable
            //First get system type of the object value
            System.Type variable_system_type = typeof(TVariable_GS<>).MakeGenericType(new System.Type[] { value.GetType() });
            //Instantiate a variable with the system type
            Variable_GS new_variable = (Variable_GS)System.Activator.CreateInstance(variable_system_type);
            //Set new var name 
            new_variable.name = name;
            //Set new var GOAP type
            new_variable.type = type;
            //Set new var object value
            new_variable.object_value = value;

            //Add the new var to the bb list
            _variables.Add(new_variable.name, new_variable);

            //Mark scene dirty
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            return new_variable;
        }

        public void RemoveVariable(string key, bool global)
        {
            Variable_GS find_var = null;
            //Variable found case
            if (_variables.TryGetValue(key, out find_var) == false)
            {
                return;
            }

            //Remove variable from blackboard
            bool ret = _variables.Remove(key);

            //Remove the variable planning instances(conditions and effects)
            if ((ret == true || global == true) && _target_agent != null)
            {
                //Remove all the planning properties that use the target variable
                RemoveVariablePlanning((global ? "Global/" : "Local/") + key);
            }

            //Mark scene dirty
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        public void RemoveVariablePlanning(string key)
        {
            //Remove the target variable from the agent planning
            //Used when a blackboard variable is removed

            //Iterate the agent action nodes
            for (int k = 0; k < _target_agent.action_nodes_num; k++)
            {
                //Focused action node
                ActionNode_GS target_node = _target_agent.action_nodes[k];
                bool repaint = false;

                //Iterate the action node conditions
                for (int n = 0; n < target_node.conditions_num; n++)
                {
                    Property_GS target_condition = target_node.conditions[n];
                    //Check if the condition uses the target variable
                    if (string.Compare(key, target_condition.A_key) == 0 || string.Compare(key, target_condition.B_key) == 0)
                    {
                        //Delete the condition if the target variable is in
                        target_node.RemoveCondition(target_condition);
                        repaint = true;
                    }
                }

                //Iterate the action node effects
                for (int n = 0; n < target_node.effects_num; n++)
                {
                    Property_GS target_effect = target_node.effects[n];
                    //Check if the effect uses the target variable
                    if (string.Compare(key, target_effect.A_key) == 0 || string.Compare(key, target_effect.B_key) == 0)
                    {
                        //Delete the effect if the target variable is in
                        target_node.RemoveEffect(target_effect);
                        repaint = true;
                    }
                }

                //Update node editor window
                if (repaint)
                {
                    target_node.window_size = Vector2.zero;
                }
            }
        }

        public void ClearBlackboard()
        {
            //Clear all the variables in the blackboard
            _variables.Clear();
        }

        //Planning Methods ============
        public WorldState_GS GenerateWorldState()
        {
            //First allocate a hashset of properties
            WorldState_GS world_state = new WorldState_GS();

            //Bool to generate the local/global prefix
            bool global = this == GlobalBlackboard_GS.blackboard;

            //Iterate all variables and generate a propery from them
            foreach(Variable_GS variable in variables.Values)
            {
                //Generate world property
                Property_GS property = new Property_GS((global ? "Global/" : "Local/") + variable.name, variable.type, OperatorType._is_equal, variable.object_value, variable.planning_value);
                //Add the generated property in the current world state
                world_state.SetGoal((global ? "Global/" : "Local/") + variable.name, property);
            }

            //Finally return the generated hashset
            return world_state;
        }


        //Get/Set Methods =============
        public Agent_GS target_agent
        {
            get
            {
                return _target_agent;
            }
            set
            {
                _target_agent = value;
            }
        }

        public Dictionary<string, Variable_GS> variables
        {
            get
            {
                return _variables;
            }
        }

        public TVariable_GS<T> GetVariable<T>(string name)
        {
            //First try to get variable by name in the dictionary
            try 
            {
                //Return the variable casted to the real class and with the T type
                return (TVariable_GS<T>)_variables[name];
            }
            catch
            {
                //If variable is not found we display a warning in console with the variable name
                Debug.LogWarning("Variable:" + name + "not found!");
                return null;
            }
        }

        public Variable_GS GetObjectVariable(string name)
        {
            try
            {
                //Return the variable without no casts
                return _variables[name];
            }
            catch
            {
                //If variable is not found we display a warning in console with the variable name
                Debug.LogWarning("Variable:" + name + " not found!");
                return null;
            }
        }

        public TVariable_GS<T> SetVariable<T>(string name, object value)
        {
            //First try to get variable by name in de dictionary
            try
            {
                //Set variable value
                _variables[name].value = value;
                //Return the resulting variable
                return (TVariable_GS<T>)_variables[name];
            }
            catch
            {
                //If variable is not found we display a warning in console with the variable name
                Debug.LogWarning("Variable:" + name + "not found!");
                return null;
            }
        }

        public Variable_GS SetObjectVariable(string name, object value)
        {
            //First try to get variable by name in de dictionary
            try
            {
                //Set variable value
                _variables[name].value = value;
                //Binded variable alert
                if (_variables[name].is_field_binded || _variables[name].is_method_binded)
                {
                    Debug.LogWarning("You are changing the object value of an already binded variable!");
                }
                //Return the resulting variable
                return _variables[name];
            }
            catch
            {
                //If variable is not found we display a warning in console with the variable name
                Debug.LogWarning("Variable:" + name + "not found!");
                return null;
            }
        }

        public T GetValue<T>(string name)
        {
            try
            {
                //Try to retur the value i the variable exists
                return (T)_variables[name].value;
            }
            catch
            {
                //If variable is not found we display a warning in console with the variable name
                Debug.LogWarning("Variable:" + name + "not found!");
                return default(T);
            }
        }

        public string[] GetKeys()
        {
            //First allocate a string array with the size of the variables num
            string[] keys = new string[variables.Count];
            //Get variable keys(names)
            variables.Keys.CopyTo(keys,0);
            //Return the generated strings array
            return keys;
        }

        public string [] GetKeysByVariableType(VariableType target_type)
        {
            //First allocate a string list
            List<string> keys = new List<string>();
            //Get the variable keys of the variables that have the target type
            foreach (Variable_GS var in variables.Values)
            {
                if(var.type == target_type)
                {
                    keys.Add(var.name);
                }
            }
            //Return the generated strings list transformed to an array
            return keys.ToArray();
        }
    }
}

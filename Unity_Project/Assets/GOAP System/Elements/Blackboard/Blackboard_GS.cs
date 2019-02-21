using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP_S.Blackboard
{
    public class Blackboard_GS
    {
        [SerializeField] private string _id;
        [SerializeField] private Dictionary<string, Variable_GS> _variables = new Dictionary<string, Variable_GS>();

        //Varibles methods ================
        public Variable_GS AddVariable(string name, PT.VariableType type, object value)
        {
            //Generate the new variable
            Variable_GS new_variable = new Variable_GS(name, type, value);
            //Add the new var to the bb list
            _variables.Add(new_variable.id, new_variable);

            return new_variable;
        }

        public bool RemoveVariable(string key)
        {
            Variable_GS find_var = null;
            //Variable found case
            if (_variables.TryGetValue(key, out find_var))
            {
                Debug.Log(find_var.name + "||" + find_var.type.ToString() + "|| Correctly Removed");
                return _variables.Remove(key);
            }
            //Variable not found case
            else
            {
                return false;
            }
        }

        public void ClearBlackboard()
        {
            //Clear all the variables in the blackboard
            _variables.Clear();
        }

        //Get/Set methods =================
        public Dictionary<string, Variable_GS> variables
        {
            get
            {
                return _variables;
            }
        }

        public int id
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    _id = System.Guid.NewGuid().ToString();
                }
                return _id.GetHashCode();
            }
        }
    }
}

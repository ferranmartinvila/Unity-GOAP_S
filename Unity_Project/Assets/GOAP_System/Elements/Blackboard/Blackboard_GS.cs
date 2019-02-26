using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
            System.Type dataType = typeof(TVariable_GS<>).MakeGenericType(new System.Type[] { value.GetType() });
            Variable_GS newData = (Variable_GS)System.Activator.CreateInstance(dataType);
            newData.type = type;
            newData.object_value = value;

            //Variable_GS new_variable = new TVariable_GS(name, type, value);
            //Add the new var to the bb list
            _variables.Add(newData.id, newData);

            return newData;
        }

        public bool RemoveVariable(string key)
        {
            Variable_GS find_var = null;
            //Variable found case
            if (_variables.TryGetValue(key, out find_var))
            {
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

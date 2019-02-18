using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard_GS
{
    [SerializeField] private string _id;
    [SerializeField] private Dictionary<string, Variable_GS> _variables = new Dictionary<string, Variable_GS>();

    //Varibles methods ================
    public Variable_GS AddVariable(string name, object value)
    {
        return 
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
            if(string.IsNullOrEmpty(_id))
            {
                _id = System.Guid.NewGuid().ToString();
            }
            return _id.GetHashCode();
        }
    }
}

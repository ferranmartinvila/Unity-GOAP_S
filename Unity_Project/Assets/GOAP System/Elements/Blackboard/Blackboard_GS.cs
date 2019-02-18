using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard_GS {

    [SerializeField] private Dictionary<string, Variable_GS> variables = new Dictionary<string, Variable_GS>();

    public Dictionary<string, Variable_GS> dic_variables
    {
       get
        {
            return variables;
        }
    }
}

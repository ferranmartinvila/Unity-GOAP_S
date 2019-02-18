using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard_GS {

    [SerializeField] private Dictionary<string, Variable_GS> _variables = new Dictionary<string, Variable_GS>();

    public Dictionary<string, Variable_GS> variables
    {
       get
        {
            return _variables;
        }
    }

    public void DrawUI(int id)
    {

    }
}

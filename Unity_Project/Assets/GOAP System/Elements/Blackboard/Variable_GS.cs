using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Variable_GS {

    //Content fields
    [SerializeField] private string name;
    [SerializeField] private string id;
    [SerializeField] private object content;
    [SerializeField] private bool protect;

    //Fields Methods =====================
    public string s_name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    public string s_id
    {
        get
        {
            if(string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }
            return id;
        }
        set
        {
            id = value;
        }
    }

    public object obj_value
    {
        get
        {
            return content;
        }
        set
        {
            content = value;
        }
    }

    public bool b_protected
    {
        get
        {
            return protect;
        }
        set
        {
            protect = value;
        }
    }
}

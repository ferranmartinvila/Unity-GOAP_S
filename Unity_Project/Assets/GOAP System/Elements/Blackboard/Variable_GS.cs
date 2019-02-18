using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Variable_GS {

    //Content fields
    [SerializeField] private string _name;
    [SerializeField] private string _id;
    [SerializeField] private object _value;
    [SerializeField] private System.Type _type;
    [SerializeField] private bool _protect;

    //Contructors =====================

    public Variable_GS(string name, object value)
    {
        //Set variable value
        _value = value;
        //Set variable name
        _name = name;
        //Set variable type
        _type = _value.GetType();
    }

    //Loop Methods ====================
    public void DrawUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(_name);
        GUILayout.EndHorizontal();
    }

    //Get/Set Methods =================
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

    public string id
    {
        get
        {
            if(string.IsNullOrEmpty(_id))
            {
                _id = Guid.NewGuid().ToString();
            }
            return _id;
        }
        set
        {
            _id = value;
        }
    }

    public object value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
        }
    }

    public System.Type type
    {
        get
        {
            return _type;
        }
        set
        {
            _type = type;
        }
    }

    public bool protect
    {
        get
        {
            return _protect;
        }
        set
        {
            _protect = value;
        }
    }
}

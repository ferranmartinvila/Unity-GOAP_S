using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Variable_GS {

    //Content fields
    [SerializeField] private string _name;
    [SerializeField] private string _id;
    [SerializeField] private object _value;
    [SerializeField] private bool _protect;

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

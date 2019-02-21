using UnityEngine;
using System;
using GOAP_S.PT;

namespace GOAP_S.Blackboard
{
    public class Variable_GS
    {
        //Content fields
        [SerializeField] private string _name = null;
        [SerializeField] private string _id = null;
        [SerializeField] private object _value = null;
        [SerializeField] private System.Type _system_type = null;
        [SerializeField] private VariableType _type = VariableType._undefined;
        [SerializeField] private bool _protect = false;

        //Contructors =====================
        public Variable_GS()
        {

        }

        public Variable_GS(string name, object value)
        {
            //Set variable value
            _value = value;
            //Set variable name
            _name = name;
            //Set variable type
            if(value != null)_system_type = _value.GetType();
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
                if (string.IsNullOrEmpty(_id))
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

        public System.Type system_type
        {
            get
            {
                return _system_type;
            }
            set
            {
                _system_type = value;
            }
        }

        public VariableType type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
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
}

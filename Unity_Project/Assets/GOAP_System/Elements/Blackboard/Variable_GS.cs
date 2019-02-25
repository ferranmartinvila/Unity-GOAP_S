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
        [SerializeField] private object _object_value = null;
        [SerializeField] private VariableType _type = VariableType._undefined;
        [SerializeField] private bool _protect = false;

        //Contructors =====================
        public Variable_GS()
        {

        }

        public Variable_GS(string name,VariableType type, object value)
        {
            //Set variable name
            _name = name;
            //Set variable type
            _type = type;
            //Set variable value
            _object_value = value;
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

        virtual public object object_value
        {
            get
            {
                return _object_value;
            }
            set
            {
                _object_value = value;
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

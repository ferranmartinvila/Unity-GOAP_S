using UnityEngine;
using System;
using GOAP_S.Tools;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace GOAP_S.Blackboard
{
    /*
     This class is the abstract base of TVariable, this is a non format class that can be used generally with a non concrete type.
     So in this class we can define all the base fields like, bind path, name, id, GOAP var type, actions delegates, ...
         */
    public abstract class Variable_GS
    {
        //Content fields
        [SerializeField] protected string _name = null;
        [SerializeField] protected object _object_value = null;
        [SerializeField] protected VariableType _type = VariableType._undefined_var_type;

        //Bind fields
        [SerializeField] protected string _binded_field_path = null;
        [SerializeField] protected string _binded_method_path = null;
        [SerializeField] protected KeyValuePair<string,object>[] _binded_method_input = null;

        protected MethodInfo _binded_method_info = null;
        protected object _binded_method_instance = null;

        //Actions
        private event Action<string> _OnNameChange; //Callback when variable name changes
        private event Action<string,object> _OnValueChange; //Callback when variable value changes

        //Bind methods
        public abstract bool BindField(string field_path);
        public abstract bool InitializeFieldBinding(GameObject target_obj);
        public abstract void UnbindField();

        public abstract bool BindMethod(string method_path);
        public abstract bool InitializeMethodBinding(GameObject target_obj);
        public abstract void UnbindMethod();

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
                //Check if new value if different from name
                if(_name != value)
                {
                    _name = value;
                    //Check if there's a delegate for name change action
                    if (_OnNameChange != null)
                    {
                        _OnNameChange(_name);
                    }
                }
            }
        }

        public virtual object object_value
        {
            get
            {
                return object_value;
            }
            set
            {
                object_value = value;
            }
        }
      
        //In base class value has no T type
        public object value
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

        //System type is defined in the TVariable class
        public abstract System.Type system_type { get; }

        public bool is_field_binded
        {
            get
            {
                return !string.IsNullOrEmpty(_binded_field_path);
            }
        }

        public bool is_method_binded
        {
            get
            {
                return !string.IsNullOrEmpty(_binded_method_path);
            }
        }

        public string binded_field_path
        {
            get
            {
                return _binded_field_path;
            }
        }

        public string binded_field_long_path
        {
            get
            {
                //Returns null if there's no path
                if (!is_field_binded)
                {
                    return null;
                }
                //Split path
                string[] parts = _binded_field_path.Split('.');
                return (parts[parts.Length - 2] + "." + parts.Last());
            }
        }

        public string binded_field_short_path
        {
            get
            {
                //Returns null if there's no path
                if (!is_field_binded)
                {
                    return null;
                }
                //Split path
                string[] parts = _binded_field_path.Split('.');
                return ("." + parts.Last());
            }
        }

        public string binded_method_path
        {
            get
            {
                return _binded_method_path;
            }
        }

        public string binded_method_short_path
        {
            get
            {
                if(!is_method_binded)
                {
                    return null;
                }
                //Split path
                string[] parts = _binded_method_path.Split('.');
                return ("." + parts.Last());
            }
        }

        public MethodInfo binded_method_info
        {
            get
            {
                return _binded_method_info;
            }
        }

        public KeyValuePair<string,object>[] binded_method_input
        {
            get
            {
                return _binded_method_input;
            }
            set
            {
                _binded_method_input = value;
            }
        }

        //Actions =====================
        protected void OnValueChange(string name, object value)
        {
            _OnValueChange(name, value);
        }
    }
}

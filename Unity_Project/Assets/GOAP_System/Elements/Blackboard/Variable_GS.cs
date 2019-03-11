using UnityEngine;
using System;
using GOAP_S.PT;
using System.Reflection;
using System.Linq;

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
        [SerializeField] protected VariableType _type = VariableType._undefined;

        //Bind fields
        [SerializeField] protected string _field_path = null;

        //Actions
        private event Action<string> _OnNameChange; //Callback when variable name changes
        private event Action<string,object> _OnValueChange; //Callback when variable value changes

        //Bind methods
        public abstract bool BindField(string field_path, GameObject target_obj);
        public abstract void UnbindField();
        public abstract bool InitializeBinding(GameObject target_obj);

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

        public bool is_binded
        {
            get
            {
                return !string.IsNullOrEmpty(_field_path);
            }
        }

        public string field_path
        {
            get
            {
                return _field_path;
            }
        }

        public string display_field_long_path
        {
            get
            {
                //Returns null if there's no path
                if (!is_binded)
                {
                    return null;
                }
                //Split path
                string[] parts = _field_path.Split('.');
                return (parts[parts.Length - 2] + "." + parts.Last());
            }
        }

        public string display_field_short_path
        {
            get
            {
                //Returns null if there's no path
                if (!is_binded)
                {
                    return null;
                }
                //Split path
                string[] parts = _field_path.Split('.');
                return ("." + parts.Last());
            }
        }
        //Actions =====================
        protected void OnValueChange(string name, object value)
        {
            _OnValueChange(name, value);
        }
    }
}

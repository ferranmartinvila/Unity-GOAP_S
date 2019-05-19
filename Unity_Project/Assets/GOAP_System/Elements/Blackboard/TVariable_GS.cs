using UnityEngine;
using System;
using System.Reflection;
using GOAP_S.Tools;
using GOAP_S.AI;
using System.Collections.Generic;

namespace GOAP_S.Blackboard
{
    [Serializable]
    public class TVariable_GS<T> : Variable_GS
    {
        //Delegates (pointers to methods)
        //Getter
        private Func<T> getter = null;
        //Setter
        private Action<T> setter = null;

        //Get/Set Methods =============
        public override object object_value
        {
            get
            {
                return _object_value;
            }

            set
            {
                _object_value = (T)value;
            }
        }

        //Override base object value get/set
        new public T value
        {
            get
            {
                //Field binded case
                if(is_field_binded)
                {
                    return getter();
                }
                
                //Method binded case
                else if(is_method_binded)
                {
                    return (T)_binded_method_info.Invoke(_binded_method_instance, ProcessMethodInput());
                }
                
                //No bind case
                else
                {
                    return (T)_object_value;
                }
            }
            set
            {
                //Check if the input value is the same as the var value
                if (object.Equals(value, _object_value)) return;
                
                //Method binded variables can not be set
                if(is_method_binded)
                {
                    Debug.LogError("The method binded variable: " + name + " can not be set!");
                    return;
                }

                //Set var value
                _object_value = value;
                
                //Set binded field value
                if (setter != null)
                {
                    setter(value);
                }
                
                //Call value change delegate
                OnValueChange(_name, value);
            }
        }

        public override Type system_type
        {
            get
            {
                return typeof(T);
            }
        }

        //Bind Methods ================
        public override bool BindField(string field_path)
        {
            //Get bind field path adding field full name and field name in dot between format
            _binded_field_path = field_path; 

            return true;
        }

        public override bool InitializeFieldBinding(GameObject target_obj)
        {
            //Reset getter/setter
            getter = null;
            setter = null;

            int last_dot_index = _binded_field_path.LastIndexOf('.');
            //Get field type in string format
            string type_string = _binded_field_path.Substring(0, last_dot_index);
            //Get field full name(what includes class inheritance)
            string unformatted_field_path_string = _binded_field_path.Substring(last_dot_index + 1);
            //Search System.Type from type string
            System.Type field_type = type_string.ToSystemType();

            //In null type case field can not be binded
            if(field_type == null)
            {
                Debug.LogWarning("Binding ERROR: Field type not found!");
                return false;
            }


            /*
             A field is a variable that is declared directly in a class or struct. 
             A property is a member that provides a flexible mechanism
             to read, write, or compute the value of a private field.
             */
            //Get field property info
            PropertyInfo field_poperty_info = field_type.GetProperty(unformatted_field_path_string);
            if(field_poperty_info != null)
            {
                //Get all the necessary field propery info
                //Get set methods
                MethodInfo get_method_info = field_poperty_info.GetGetMethod();
                MethodInfo set_method_info = field_poperty_info.GetSetMethod();
                //Check if methods are static
                bool is_static = (get_method_info != null && get_method_info.IsStatic) || (set_method_info != null && set_method_info.IsStatic);
                //Get field instance if the property is not static
                Component field_instance = is_static ? null : target_obj.GetComponent(field_type);
                if(field_instance == null && !is_static)
                {
                    Debug.Log("Variable bind missing component: " + type_string);
                    return false;
                }
                //Check if property is readable
                if(field_poperty_info.CanRead)
                {
                    /*
                        AOT compiler: compiles before running
                        JIT compiler: compiles while running
                     */
                    try //JIT
                    {
                        getter = get_method_info.CreateDelegate<Func<T>>(field_instance);
                    }
                    catch //AOT
                    {
                        getter = () => { return (T)get_method_info.Invoke(field_instance, null); };
                    }
                }
                else
                {
                    //In case property can not be read we display a error message
                    getter = ()=> {
                        Debug.LogError("You are trying to get a non readable property! Name:" + name );
                        return default(T);
                    };
                }

                //Check if property is editable
                if(field_poperty_info.CanWrite)
                {
                    try //JIT
                    {
                        setter = set_method_info.CreateDelegate<Action<T>>(field_instance);
                    }
                    catch //AOT
                    {
                        setter = (input) => { set_method_info.Invoke(field_instance, new object[] { input }); };
                    }
                }
                else
                {
                    //In case property can not be set we display a error message
                    setter = (input) => { Debug.LogError("You are trying to set a non editable property! Name:" + name); };
                }

                return true;
            }

            //Get field field info
            FieldInfo field_field_info = field_type.GetField(unformatted_field_path_string);
            if(field_field_info != null)
            {
                //If field is not static get the instance of it
                Component field_instance = field_field_info.IsStatic ? null : target_obj.GetComponent(field_type);
                //If the field is not static and the instance is null means that the field has not been found
                if (field_instance == null && !field_field_info.IsStatic)
                {
                    Debug.Log("Variable bind missing component: " + type_string);
                    return false;
                }
                //Check if the field is read only
                if(field_field_info.IsInitOnly || field_field_info.IsLiteral)
                {
                    //Get field value from field field info
                    T value = (T)field_field_info.GetValue(field_instance);
                    //Set getter as return value
                    getter = () => { return value; };
                }
                else
                {
                    //Set getter as get value from field instance using field info
                    getter = () => { return (T)field_field_info.GetValue(field_instance); };
                    //Set setter as set value from field instance using field info passing value called input
                    setter = (input) => { field_field_info.SetValue(field_instance, input); };
                }
                return true;
            }

            return false;
        }

        public override void UnbindField()
        {
            //Set bind data to null, so we basically reset the information about the binded field
            _binded_field_path = null;
            getter = null;
            setter = null;
        }

        public override bool BindMethod(string method_path)
        {
            //Set the bind method path
            _binded_method_path = method_path;
            
            return true;
        }

        public override bool InitializeMethodBinding(GameObject target_obj)
        {
            //Reset getter/setter
            getter = null;
            setter = null;

            //Get the target method instance
            KeyValuePair<MethodInfo, object> target_method = ProTools.FindMethodFromPath(_binded_method_path, system_type, target_obj);

            //In null instance case the variable is unbind
            if(target_method.Value == null)
            {
                Debug.LogError("Binded method not found!");

                UnbindMethod();
                return false;
            }

            //Set method info
            _binded_method_info = target_method.Key;
            //Set method instance
            _binded_method_instance = target_method.Value;

            for (int k = 0; k < _binded_method_input.Length; k++)
            {
                //Current input info
                KeyValuePair<string, object> input = _binded_method_input[k];

                //Binded value case
                if (string.IsNullOrEmpty(input.Key) == false)
                {
                    //Check if is a local or global variable
                    //Global case
                    string[] input_info = input.Key.Split('/'); //Input location and variable name
                    if (string.Compare(input_info[0], "Global") == 0)
                    {
                        _binded_method_input[k] = new KeyValuePair<string, object>(_binded_method_input[k].Key, GlobalBlackboard_GS.blackboard.GetObjectVariable(input_info[1]));
                    }
                    //Local case
                    else
                    {
                        _binded_method_input[k] = new KeyValuePair<string, object>(_binded_method_input[k].Key, target_obj.GetComponent<Agent_GS>().blackboard.GetObjectVariable(input_info[1]));
                    }
                }
                //Not binded value case
                else if (input.Value == null)
                {
                    Debug.LogError("The method bind process for the variable " + name + " has an input error in index " + k + " !");
                }
            }
            return true;
        }

        private object[] ProcessMethodInput()
        {
            object[] input = new object[_binded_method_input.Length];
            for (int k = 0; k < input.Length; k++)
            {
                if(string.IsNullOrEmpty(_binded_method_input[k].Key) == false)
                {
                    input[k] = ((Variable_GS)_binded_method_input[k].Value).value;
                }
                else
                {
                    input[k] = _binded_method_input[k].Value;
                }
            }
            return input;
        }

        public override void UnbindMethod()
        {
            //Set bind data to null, so we basically reset the information about the binded method
            getter = null;
            setter = null;
            _binded_method_path = null;
            _binded_method_info = null;
            _binded_method_instance = null;
            _binded_method_input = null;
            _binded_method_input = null;
        }
    }
}

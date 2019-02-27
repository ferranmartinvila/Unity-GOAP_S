using UnityEngine;
using System;
using System.Reflection;
using GOAP_S.PT;

namespace GOAP_S.Blackboard
{
    public class TVariable_GS<T> : Variable_GS
    {
        //Content fields
        [SerializeField]private T _value = default(T);
        
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
                //If the var is not binded it has its own value
                if(getter == null)
                {
                    return _value;
                }
                //In the other case we return the value of the binded field
                else
                {
                    return getter();
                }
            }
            set
            {
                //Checl if the input value is the same as the var value
                if (object.Equals(value, _value)) return;
                //Set var value
                _value = value;
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
        public override bool BindField(MemberInfo field_info, GameObject target_obj)
        {
            //Check if the member info is info of a field or a property
            if (!(field_info is FieldInfo) && !(field_info is PropertyInfo))
            {
                Debug.LogWarning("BindField ERROR: Member info non correct format.");
                return false;
            }

            //Get bind field path adding field full name and field name in dot between format
            _field_path = string.Format("{0}.{1}", field_info.ReflectedType.FullName, field_info.Name);

            //If target GameObject is not null field bind is initialized
            if(target_obj != null)
            {
                return InitializeBinding(target_obj);
            }

            return true;
        }

        public override bool InitializeBinding(GameObject target_obj)
        {
            //Check if the variable has a field path to bind
            if (!is_binded) return false;

            //Reset getter/setter
            getter = null;
            setter = null;

            int last_dot_index = _field_path.LastIndexOf('.');
            //Get field type in string format
            string type_string = _field_path.Substring(0, last_dot_index);
            //Get field full name(what includes class inheritance)
            string unformatted_field_path_string = _field_path.Substring(last_dot_index + 1);
            //Search System.Type from type string
            System.Type field_type = ProTools.StringToSystemType(type_string);

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
                    //In case property can not be set wedisplay a error message
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
            _field_path = null;
            getter = null;
            setter = null;
        }
    }
}

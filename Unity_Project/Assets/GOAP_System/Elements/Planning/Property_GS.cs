using System;
using UnityEngine;
using GOAP_S.Tools;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace GOAP_S.Planning
{
    //BlockedProperty class attribute
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class BlockedProperty_GS : Attribute
    {

    }

    public class Property_GS
    {
        //Content fields
        [SerializeField] private VariableType _variable_type = VariableType._undefined_var_type; //Variable type we are working with in this condition
        [SerializeField] private string _A_key = null; //A Property key, the key of the first property in the condition
        [SerializeField] private OperatorType _operator = OperatorType._undefined_operator; //Condition operator
        [SerializeField] private string _B_key = null; //B Property key, the key of the second property in the condition
        [SerializeField] private object _value = null; //B value, value that we compare with the A Property value

        //Constructors ================
        public Property_GS()
        {

        }

        public Property_GS(Property_GS copy)
        {
            _variable_type = copy.variable_type;
            _A_key = copy.A_key;
            _operator = copy.operator_type;
            _B_key = copy.B_key;
            _value = copy.value;
        }

        //Used to generate properties form bb variables
        public Property_GS(string new_key, VariableType new_type, OperatorType new_operator, object new_value)
        {
            //Set property fields
            _A_key = new_key;
            _variable_type = new_type;
            _operator = new_operator;
            _value = new_value;
        }

        //Planning Methods ============
        public bool DistanceTo(Property_GS target_property)
        {
            //Check if the target properties have the same variable type
            if(_variable_type != target_property.variable_type)
            {
                Debug.LogError("Comparsion type between variable: " + _variable_type + " and variable: " + target_property.variable_type + " not supported!");
                return false;
            }

            //Check if this property value have the defined value by the target property
            //Do different operations depending of the operator type
            switch (target_property.operator_type)
            {
                case OperatorType._is_equal:
                case OperatorType._equal_equal:
                    {
                        switch (_variable_type)
                        {
                            case VariableType._string: return string.Compare((string)_value, (string)target_property.value) == 0;
                            case VariableType._bool: return (bool)_value == (bool)target_property.value;
                            case VariableType._char: return (char)_value == (char)target_property.value;
                            case VariableType._float: return (float)_value == (float)target_property.value;
                            case VariableType._int: return (int)_value == (int)target_property.value;
                            case VariableType._vector2: return (Vector2)_value == (Vector2)target_property.value;
                            case VariableType._vector3: return (Vector3)_value ==  (Vector3)target_property.value;
                            case VariableType._vector4: return (Vector4)_value == (Vector4)target_property.value;
                        }
                    }
                    break;
                case OperatorType._different:
                    {
                        switch (_variable_type)
                        {
                            case VariableType._string: return string.Compare((string)_value, (string)target_property.value) != 0;
                            case VariableType._bool: return (bool)_value != (bool)target_property.value;
                            case VariableType._char: return (char)_value != (char)target_property.value;
                            case VariableType._float: return (float)_value != (float)target_property.value;
                            case VariableType._int: return (int)_value != (int)target_property.value;
                            case VariableType._vector2: return (Vector2)_value != (Vector2)target_property.value;
                            case VariableType._vector3: return (Vector3)_value != (Vector3)target_property.value;
                            case VariableType._vector4: return (Vector4)_value != (Vector4)target_property.value;
                        }
                    }
                    break;
                case OperatorType._bigger:
                    {
                        switch (_variable_type)
                        {
                            case VariableType._char: return (char)_value > (char)target_property.value;
                            case VariableType._float: return (float)_value > (float)target_property.value;
                            case VariableType._int: return (int)_value > (int)target_property.value;
                            case VariableType._vector2: return ((Vector2)_value).magnitude > ((Vector2)target_property.value).magnitude;
                            case VariableType._vector3: return ((Vector3)_value).magnitude > ((Vector3)target_property.value).magnitude;
                            case VariableType._vector4: return ((Vector4)_value).magnitude > ((Vector4)target_property.value).magnitude;
                        }
                    }
                    break;
                case OperatorType._bigger_or_equal:
                    {
                        switch (_variable_type)
                        {
                            case VariableType._char: return (char)_value >= (char)target_property.value;
                            case VariableType._float: return (float)_value >= (float)target_property.value;
                            case VariableType._int: return (int)_value >= (int)target_property.value;
                            case VariableType._vector2: return ((Vector2)_value).magnitude >= ((Vector2)target_property.value).magnitude;
                            case VariableType._vector3: return ((Vector3)_value).magnitude >= ((Vector3)target_property.value).magnitude;
                            case VariableType._vector4: return ((Vector4)_value).magnitude >= ((Vector4)target_property.value).magnitude;
                        }
                    }
                    break;
                case OperatorType._smaller:
                    {
                        switch (_variable_type)
                        {
                            case VariableType._char: return (char)_value < (char)target_property.value;
                            case VariableType._float: return (float)_value < (float)target_property.value;
                            case VariableType._int: return (int)_value < (int)target_property.value;
                            case VariableType._vector2: return ((Vector2)_value).magnitude < ((Vector2)target_property.value).magnitude;
                            case VariableType._vector3: return ((Vector3)_value).magnitude < ((Vector3)target_property.value).magnitude;
                            case VariableType._vector4: return ((Vector4)_value).magnitude < ((Vector4)target_property.value).magnitude;
                        }
                    }
                    break;
                case OperatorType._smaller_or_equal:
                    {
                        switch (_variable_type)
                        {
                            case VariableType._char: return (char)_value <= (char)target_property.value;
                            case VariableType._float: return (float)_value <= (float)target_property.value;
                            case VariableType._int: return (int)_value <= (int)target_property.value;
                            case VariableType._vector2: return ((Vector2)_value).magnitude <= ((Vector2)target_property.value).magnitude;
                            case VariableType._vector3: return ((Vector3)_value).magnitude <= ((Vector3)target_property.value).magnitude;
                            case VariableType._vector4: return ((Vector4)_value).magnitude <= ((Vector4)target_property.value).magnitude;
                        }
                    }
                    break;
            }
            //In non valid operator type return false
            Debug.LogError("Variable: current " + _A_key + " " + operator_type.ToShortString() + " goal " + target_property.A_key + " is not a valid operation!");
            return false;
        }

        //Get/Set Methods =============
        public VariableType variable_type
        {
            get
            {
                return _variable_type;
            }
            set
            {
                _variable_type = value;
            }
        }

        public string A_key
        {
            get
            {
                return _A_key;
            }
            set
            {
                _A_key = value;
            }
        }

        public string B_key
        {
            get
            {
                return _B_key;
            }
            set
            {
                _B_key = value;
            }
        }

        public OperatorType operator_type
        {
            get
            {
                return _operator;
            }
            set
            {
                _operator = value;
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

        public string display_value
        {
            get
            {
                if (!string.IsNullOrEmpty(_B_key))
                {
                    return _B_key;
                }
                else return _value.ToString();
            }
        }
    }
}

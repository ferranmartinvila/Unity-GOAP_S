using UnityEngine;
using System;
using System.Reflection;

namespace GOAP_S.Blackboard
{
    public class TVariable_GS<T> : Variable_GS
    {
        //Content fields
        [SerializeField]private T _value = default(T);
        //Getter
        private Func<T> getter = null;
        //Setter
        private Action<T> setter = null;

        //Get/Set Methods =============
        public T value
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
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GOAP_S.UI
{
    public class VariableSelectMenu_GS : PopupWindowContent
    {
        //Content fields
        static private Variable_GS _target_variable = null; //Focused variable

        //Contructors =================
        public VariableSelectMenu_GS(Variable_GS target_variable)
        {
            //Set the focused variable
            _target_variable = target_variable;
        }

        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginHorizontal();

            //Here blit variable info
            GUILayout.Label(_target_variable.value.GetType().ToString());

            GUILayout.EndHorizontal();
        }

        //Get/Set Methods =============
        public Variable_GS target_variable
        {
            get
            {
                return _target_variable;
            }
            set
            {
                _target_variable = value;
            }
        }
    }
}

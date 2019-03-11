using UnityEditor;
using UnityEngine;
using GOAP_S.Planning;
using GOAP_S.Blackboard;
using GOAP_S.PT;

namespace GOAP_S.UI
{
    public class Property_GS_Editor
    {
        //Content fields
        private PropertyUIMode _property_UI_mode = PropertyUIMode.IS_UNDEFINED; //Depending of the mode the user will be able to modify the properties with different options
        private Property_GS _target_property = null; //The property is this editor working with
        private Blackboard_GS _target_blackboard = null; //The blackboard is this editor working with

        //Constructors
        public Property_GS_Editor(Property_GS new_property, Blackboard_GS new_bb, PropertyUIMode new_ui_mode)
        {
            //Set target property
            _target_property = new_property;
            //Set target bb
            _target_blackboard = new_bb;
            //Set property editor UI mode
            _property_UI_mode = new_ui_mode;
        }

        //Loop Methods ====================
        public void DrawUI()
        {
            GUILayout.Label("hello");
        }
    }
}

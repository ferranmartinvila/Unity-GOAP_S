using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GOAP_S.Blackboard;

namespace GOAP_S.UI
{
    public static class GlobalBlackboard_GS_Editor
    {
        //Content fields
        [SerializeField] private static Blackboard_GS_Editor _blackboard_editor = null;

        //Loop Methods ================
        public static void DrawGlobalUI()
        {
            blackboard_editor.DrawGlobalUI();
        }

        //Get/Set Methods =============
        public static Blackboard_GS_Editor blackboard_editor
        {
            get
            {
                if(_blackboard_editor == null)
                {
                    _blackboard_editor = new Blackboard_GS_Editor(GlobalBlackboard_GS.blackboard);
                }
                return _blackboard_editor;
            }
        }
    }
}

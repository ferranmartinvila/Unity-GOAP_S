using UnityEngine;
using UnityEditor;

namespace GOAP_S.UI
{
    public sealed class SecurityAcceptMenu_GS : PopupWindowContent
    {
        //Content fields
        public delegate void AcceptCallbackFunction();
        public static AcceptCallbackFunction on_accept_delegate;
        public static AcceptCallbackFunction on_cancel_delegate;

        //Config fields
        private Vector2 window_size = new Vector2(250.0f, 50.0f);



        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            //First set popup window size
            editorWindow.minSize = window_size;
            editorWindow.maxSize = window_size;

            //Menu area
            GUILayout.BeginVertical();

            //Menu title
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("Are you sure you want to do this action?", GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            //Buttons area
            GUILayout.BeginHorizontal();

            //Accept button
            if(GUILayout.Button("Accept", GUILayout.ExpandWidth(true)))
            {
                //First check if there's a method to call
                if (on_accept_delegate != null)
                {
                    //Call the delegate on accept
                    on_accept_delegate();
                    //Reset delegate(deletes method pointers in)
                    on_accept_delegate = null;
                }
                else
                {
                    //If theres no method this menu is useless
                    Debug.LogWarning("SecurityAccept delegate is null!");
                }

                //When action is done close popup
                editorWindow.Close();
            }
            //Cancel button
            if(GUILayout.Button("Cancel", GUILayout.ExpandWidth(true)))
            {
                //First check if there's a method to call
                if (on_cancel_delegate != null)
                {
                    //Call the delegate on cancel
                    on_cancel_delegate();
                    //Reset delegate(deletes method pointers in)
                    on_cancel_delegate = null;
                }

                //Simply close on cancel
                editorWindow.Close();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}

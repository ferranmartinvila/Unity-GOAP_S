using UnityEngine;

/// <summary>
/// Info about GUIStyle
/// https://gist.github.com/MadLittleMods/ea3e7076f0f59a702ecb
/// </summary>
namespace GOAP_S.UI
{
    public class UIConfig_GS
    {
        //UI Configurations fields
        static private GUIStyle _node_window_style = null; //Node title style
        static private GUIStyle _node_selection_buttons_style = null; //Buttons used to select node elements style
        static private GUIStyle _node_modify_button_style = null; //Button used to delete or edit the node data
        static private GUIStyle _node_elements_style = null; //Used in the condition,action,reward names
        static private GUIStyle _node_description_style = null; //Used in the description label of action nodes
        static private GUIStyle _select_menu_title_style = null; //Used to set the pop windows in node editor style
        static private GUIStyle _select_menu_button_style = null; //Used to set the buttons in the selection menus
        static private GUIStyle _blackboard_window_style = null; //Used in the bb base window
        static private GUIStyle _blackboard_variable_style = null; //Used in bb variables
        static private GUIStyle _blackboard_title_style = null; //Used in the title inside the bb window, not window title

        static private GUIStyle _left_bold_style = null;

        //Static instance of this class
        private static UIConfig_GS _Instance;
        
        //Property to get static instance
        public static UIConfig_GS Instance
        {
            get
            {
                //Check if the instance is null, in null case generates a new one
                if (_Instance == null)
                {
                    _Instance = new UIConfig_GS();
                }
                return _Instance;
            }
        }

        //Get/Set methods =================
        public GUIStyle node_window_style
        {
            get
            {
                if (_node_window_style == null)
                {
                    //Configure the node window style
                    _node_window_style = new GUIStyle(GUI.skin.window);
                    _node_window_style.alignment = TextAnchor.UpperCenter;
                    _node_window_style.fontSize = 12;
                }
                return _node_window_style;
            }
        }

        public GUIStyle node_selection_buttons_style
        {
            get
            {
                if (_node_selection_buttons_style == null)
                {
                    //Configure the selection buttons style
                    _node_selection_buttons_style = new GUIStyle(GUI.skin.button);
                    _node_selection_buttons_style.alignment = TextAnchor.UpperCenter;
                }
                return _node_selection_buttons_style;
            }
        }

        public GUIStyle node_modify_button_style
        {
            get
            {
                if (_node_modify_button_style == null)
                {
                    //Configure the modify button style
                    _node_modify_button_style = new GUIStyle("AssetLabel");
                }
                return _node_modify_button_style;
            }
        }

        public GUIStyle node_elements_style
        {
            get
            {
                if (_node_elements_style == null)
                {
                    //Configure the elements style
                    _node_elements_style = new GUIStyle("label");
                    _node_elements_style.fontStyle = FontStyle.Bold;
                    _node_elements_style.fontSize = 12;
                }
                return _node_elements_style;
            }
        }

        public GUIStyle node_description_style
        {
            get
            {
                if (_node_description_style == null)
                {
                    //Configure node description style
                    _node_description_style = new GUIStyle("label");
                    _node_description_style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
                return _node_description_style;
            }
        }


        public GUIStyle select_menu_title_style
        {
            get
            {
                if (_select_menu_title_style == null)
                {
                    //Configure the popup windows style
                    _select_menu_title_style = new GUIStyle("label");
                    _select_menu_title_style.fontStyle = FontStyle.Bold;
                    _select_menu_title_style.fontSize = 15;
                    _select_menu_title_style.alignment = TextAnchor.UpperCenter;
                }
                return _select_menu_title_style;
            }
        }

        public GUIStyle select_menu_button_style
        {
            get
            {
                if (_select_menu_button_style == null)
                {
                    //Configure the popup windows style
                    _select_menu_button_style = new GUIStyle("label");
                }
                return _select_menu_button_style;
            }
        }

        public GUIStyle blackboard_window_style
        {
            get
            {
                if (_blackboard_window_style == null)
                {
                    //Configure the bb window style
                    _blackboard_window_style = new GUIStyle("Box");
                    _blackboard_window_style.alignment = TextAnchor.UpperCenter;
                    _blackboard_window_style.fontStyle = FontStyle.Bold;
                    _blackboard_window_style.fontSize = 15;
                }
                return _blackboard_window_style;
            }
        }

        public GUIStyle blackboard_variable_style
        {
            get
            {
                if (_blackboard_variable_style == null)
                {
                    //Config the bb variable style
                    _blackboard_variable_style = new GUIStyle("Label");
                }
                return _blackboard_variable_style;
            }
        }

        public GUIStyle blackboard_title_style
        {
            get
            {
                if (_blackboard_title_style == null)
                {
                    //Config the bb variable style
                    _blackboard_title_style = new GUIStyle("Label");
                    _blackboard_title_style.fontSize = 12;
                }
                return _blackboard_title_style;
            }
        }

        public GUIStyle left_bold_style
        {
            get
            {
                if (_left_bold_style == null)
                {
                    _left_bold_style = new GUIStyle("label");
                    _left_bold_style.alignment = TextAnchor.MiddleLeft;
                    _left_bold_style.fontStyle = FontStyle.Bold;
                }
                return _left_bold_style;
            }
        }
    }
}

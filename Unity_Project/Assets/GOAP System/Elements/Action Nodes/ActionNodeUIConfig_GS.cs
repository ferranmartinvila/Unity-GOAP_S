using UnityEngine;

/// <summary>
/// Info about GUIStyle
/// https://gist.github.com/MadLittleMods/ea3e7076f0f59a702ecb
/// </summary>
public class ActionNodeUIConfig_GS
{
    //UI Configurations fields
    static private GUIStyle _node_window_style = null; //Node title style
    static private GUIStyle _selection_buttons_style = null; //Buttons used to select node elements style
    static private GUIStyle _modify_button_style = null; //Button used to delete or edit the node data
    static private GUIStyle _elements_style = null; //Used in the condition,action,reward names
    static private GUIStyle _pop_windows_style = null; //Used to set the pop windows in node editor style

    //Get/Set methods =================
    public GUIStyle node_window_style
    {
        get
        {
            if(_node_window_style == null)
            {
                //Configure the node window style
                _node_window_style = new GUIStyle(GUI.skin.window);
                _node_window_style.alignment = TextAnchor.UpperCenter;
                _node_window_style.fontSize = 12;
            }
            return _node_window_style;
        }
    }
    
    public GUIStyle selection_button_style
    {
        get
        {
            if (_selection_buttons_style == null)
            {
                //Configure the selection buttons style
                _selection_buttons_style = new GUIStyle(GUI.skin.button);
                _selection_buttons_style.alignment = TextAnchor.UpperCenter;
            }
            return _selection_buttons_style;
        }
    }

    public GUIStyle modify_button_style
    {
        get
        {
            if (_modify_button_style == null)
            {
                //Configure the modify button style
                _modify_button_style = new GUIStyle("AssetLabel");
            }
            return _modify_button_style;
        }
    }

    public GUIStyle elements_style
    {
        get
        {
            if (_elements_style == null)
            {
                //Configure the elements style
                _elements_style = new GUIStyle("label");
                _elements_style.fontStyle = FontStyle.Bold;
                _elements_style.fontSize = 12;
            }
            return _elements_style;
        }
    }

    public GUIStyle pop_windows_style
    {
        get
        {
            if (_pop_windows_style == null)
            {
                //Configure the popup windows style
                _pop_windows_style = new GUIStyle("label");
                _pop_windows_style.fontStyle = FontStyle.Bold;
                _pop_windows_style.fontSize = 15;
            }
            return _pop_windows_style;
        }
    }
}

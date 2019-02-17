using UnityEngine;

/// <summary>
/// Info about GUIStyle
/// https://gist.github.com/MadLittleMods/ea3e7076f0f59a702ecb
/// </summary>
public class ActionNodeUIConfig_GS
{
    //UI Configurations fields
    static private GUIStyle node_window_style = null; //Node title style
    static private GUIStyle selection_buttons_style = null; //Buttons used to select node elements style
    static private GUIStyle modify_button_style = null; //Button used to delete or edit the node data
    static private GUIStyle elements_style = null; //Used in the condition,action,reward names
    static private GUIStyle pop_windows_style = null; //Used to set the pop windows in node editor style
    //State fields
    private bool initialized = false;
    //UI fields

    //Initialize method ===============
    public void InitializeConfig()
    {
        //GUIStyles -------------------
        //Configure the node window style
        node_window_style = new GUIStyle(GUI.skin.window);
        node_window_style.alignment = TextAnchor.UpperCenter;
        node_window_style.fontSize = 12;

        //Configure the selection buttons style
        selection_buttons_style = new GUIStyle(GUI.skin.button);
        selection_buttons_style.alignment = TextAnchor.UpperCenter;

        //Configure the modify button style
        modify_button_style = new GUIStyle("AssetLabel");

        //Configure the elements style
        elements_style = new GUIStyle("label");
        elements_style.fontStyle = FontStyle.Bold;
        elements_style.fontSize = 12;

        //Configure the popup windows style
        pop_windows_style = new GUIStyle("label");
        pop_windows_style.fontStyle = FontStyle.Bold;
        pop_windows_style.fontSize = 15;

        //Set initialized bool
        initialized = false;
    }

    //Get methods =====================
    public bool GetInitialized()
    {
        return initialized;
    }

    public GUIStyle GetNodeWindowStyle()
    {
        return node_window_style;
    }

    public GUIStyle GetSelectionButtonsStyle()
    {
        return selection_buttons_style;
    }

    public GUIStyle GetModifyButtonStyle()
    {
        return modify_button_style;
    }
    public GUIStyle GetElementsStyle()
    {
        return elements_style;
    }

    public GUIStyle GetPopWindowsStyle()
    {
        return pop_windows_style;
    }
}

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
    }

    //Get/Set methods =================
    public GUIStyle style_node_window
    {
        get
        {
            if(node_window_style == null)
            {
                //Configure the node window style
                node_window_style = new GUIStyle(GUI.skin.window);
                node_window_style.alignment = TextAnchor.UpperCenter;
                node_window_style.fontSize = 12;
            }
            return node_window_style;
        }
    }
    
    public GUIStyle style_selection_button
    {
        get
        {
            if (selection_buttons_style == null)
            {
                //Configure the selection buttons style
                selection_buttons_style = new GUIStyle(GUI.skin.button);
                selection_buttons_style.alignment = TextAnchor.UpperCenter;
            }
            return selection_buttons_style;
        }
    }

    public GUIStyle style_modify_button
    {
        get
        {
            if (modify_button_style == null)
            {
                //Configure the modify button style
                modify_button_style = new GUIStyle("AssetLabel");
            }
            return modify_button_style;
        }
    }

    public GUIStyle style_elements
    {
        get
        {
            if (elements_style == null)
            {
                //Configure the elements style
                elements_style = new GUIStyle("label");
                elements_style.fontStyle = FontStyle.Bold;
                elements_style.fontSize = 12;
            }
            return elements_style;
        }
    }

    public GUIStyle style_pop_windows
    {
        get
        {
            if (pop_windows_style == null)
            {
                //Configure the popup windows style
                pop_windows_style = new GUIStyle("label");
                pop_windows_style.fontStyle = FontStyle.Bold;
                pop_windows_style.fontSize = 15;
            }
            return pop_windows_style;
        }
    }
}

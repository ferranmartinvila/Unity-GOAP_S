using UnityEngine;

/// <summary>
/// Info about GUIStyle
/// https://gist.github.com/MadLittleMods/ea3e7076f0f59a702ecb
/// </summary>
public class ActionNodeUIConfig_GS
{
    //TODO: Check notes
    //UI fields
    [SerializeField] public string node_title; //Node title string
    //UI Config
    [System.NonSerialized] public GUIStyle node_title_style = null; //Node title style
    //UI Config fields
    [SerializeField] private TextAnchor title_text_anchor; //Title text alignment
    [SerializeField] private int title_font_size; //Title font size
    [SerializeField] private FontStyle title_font_style;//Title font style

    //Initialize method ===============
    public void InitializeConfig()
    {
        //Set header string
        node_title = "Action Node";
        //Configure the header style
        node_title_style = new GUIStyle(GUI.skin.window);
        node_title_style.alignment = TextAnchor.UpperCenter;
        node_title_style.fontSize = 12;
        
        //("HelpBox");

        /*node_title_style.alignment = TextAnchor.UpperCenter;
        node_title_style.fontSize = 10;
        node_title_style.margin = new RectOffset(10, 10, 10, 10);*/
        //node_title_style.fontStyle = FontStyle.Bold;
    }

    //Get methods =====================
    public GUIStyle GetNodeTitleStyle()
    {
        return node_title_style;
    }
}

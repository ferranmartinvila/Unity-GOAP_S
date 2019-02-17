using UnityEngine;

public class ActionNodeUIConfig_GS
{
    //TODO: Check notes
    //UI fields
    [SerializeField] public string node_title; //Node title string
    [SerializeField] public GUIStyle node_title_style; //Node title style

    //Initialize method ===============
    public void InitializeConfig()
    {
        //Set header string
        node_title = "Action Node";
        //Configure the header style
        node_title_style = new GUIStyle("label");
        node_title_style.alignment = TextAnchor.UpperCenter;
        node_title_style.fontSize = 15;
        node_title_style.fontStyle = FontStyle.Bold;
    }
}

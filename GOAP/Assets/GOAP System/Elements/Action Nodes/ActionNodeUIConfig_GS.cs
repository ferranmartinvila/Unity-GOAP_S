using UnityEngine;

[System.Serializable]
public class ActionNodeUIConfig_GS
{
    //TODO: Check notes
    //UI fields
    public string node_title; //Node title string
    public GUIStyle node_title_style; //Node title style


    //Constructor =====================
    public ActionNodeUIConfig_GS()
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

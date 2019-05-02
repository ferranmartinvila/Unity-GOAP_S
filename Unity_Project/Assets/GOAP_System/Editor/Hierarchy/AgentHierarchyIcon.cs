using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using GOAP_S.AI;

[InitializeOnLoad]
public sealed class AgentHierarchyIcon
{
    //Content fields
    private static Texture2D _icon_texture = null; //Texture we use as agent hierarchy icon
    private static List<int> _agent_object_ids = null; //List with the gameobjects that containt an agent ids
    
    //Initialize GoapSystem agents hierarchy icon system
    static AgentHierarchyIcon()
    {
        //Load agent hierarchy icon 
        _icon_texture = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/GOAP_System/Editor/Hierarchy/AgentHierarchyIcon.png", typeof(Texture2D));
        //Add goap system hierarchy update to the editor hierarchy change delegate
        EditorApplication.hierarchyChanged += Update_GS_Hierarchy;
        //Add goap system agent hierarchy item method
        EditorApplication.hierarchyWindowItemOnGUI += Hierarchy_GS_Agent;
        //Call update
        Update_GS_Hierarchy();
    }

    //GoapSystem hierarchy update
   static void Update_GS_Hierarchy()
    {
        //Get all game objects scene
        GameObject[] scene_game_objects = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
        //Allocate agent objects ids list
        _agent_object_ids = new List<int>();
        //Iterate scene gameobject and look for agent components
        foreach(GameObject scene_object in scene_game_objects)
        {
            if(scene_object.GetComponent<Agent_GS>() != null)
            {
                //Store the game object with agent id
                _agent_object_ids.Add(scene_object.GetInstanceID());
            }
        }
    }

    //GoapSystem agent in hierarchy method
    static void Hierarchy_GS_Agent(int instanceID, Rect item_rect)
    {
        //Maybe theres no hierarchy change, lets check it
        //icons will be generated in the next change, no problem
        if (_agent_object_ids == null)
        {
            return;
        }

        //Get hierarchy UI item rect
        Rect position = new Rect(item_rect);
        //Set icon x postion and width
        position.x = position.width;
        position.width = 18;

        //Check if the current gameobject id is in our gameobjects with agent ids list
        if (_agent_object_ids.Contains(instanceID))
        {
            //In true case draw goap system agent icon
            GUI.Label(position, _icon_texture);
        }
    }
}

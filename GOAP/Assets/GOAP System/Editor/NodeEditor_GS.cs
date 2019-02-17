using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeEditor_GS : EditorWindow
{
    static NodeEditor_GS window;
    private GameObject selected_object = null;
    private Agent_GS selected_agent = null;
    private Vector2 mouse_pos;
    
    Rect start_node;
    Rect window1;
    Rect window2;
    Rect window2_;

    //Window Menu Item ================
    [MenuItem("Tools / GOAP / Node Editor")]
    static void ShowNodeEditor()
    {
        window = (NodeEditor_GS)EditorWindow.GetWindow(typeof(NodeEditor_GS));
        ConfigureWindow();
    }

    [MenuItem("Tools / GOAP / Node Editor", true)]
    static bool CheckAgentToShowNodeEditor()
    {
        return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Agent_GS>() != null;
    }

    //Window Configuration ============
    static void ConfigureWindow()
    {
        window.titleContent.text = "Node Editor"; //Set a window title
    }

    //Loop Methods ====================
    public void Awake()
    {
        //Set the selected agent to draw its canvas
        //selected_agent = EditorWindow.GetWindow<Manager_GS>().target_obj.GetComponent<Agent_GS>();
        
        //Check if the selected agent is initialized
        //if (!selected_agent.GetAgentInit()) selected_agent.Initialize();

        //Iterate all the nodes and place them in the canvas
        start_node = new Rect(210, 10, 100, 100);
        window1 = new Rect(10, 10, 100, 100);
        window2 = new Rect(210, 210, 100, 100);
        window2_ = new Rect(210, 210, 100, 100);
    }

    private void OnEnable()
    {
        Repaint();
    }

    void OnGUI()
    {
        if (Selection.activeGameObject != selected_object)
        {
            SetSelectedAgent(Selection.activeGameObject.gameObject.GetComponent<Agent_GS>());
        }

        if (selected_agent == null || selected_agent.action_nodes == null) return;

        mouse_pos = Event.current.mousePosition;

        /*DrawNodeCurve(window1, window2); //curve is drawn under the windows
        DrawNodeCurve(start_node, window2); 
        DrawNodeCurve(window2_, window1); */
        BeginWindows();

        //Window inputs
        if (EditorWindow.focusedWindow == this && EditorWindow.mouseOverWindow == this)//Check if focus is on this windows
        {
            //Right click
            if (Event.current.button == 1)
            {
                //Update mouse pos
                mouse_pos = Event.current.mousePosition;
                //Show node editor popup menu
                PopupWindow.Show(new Rect(mouse_pos.x, mouse_pos.y, 0,0), new GOAP_S.UI.NodeEditorPopMenu_GS(this));
            }
        }


        //Here iterate all the nodes, now just draw them
        //Generate an output to know the node state is the next step


        //GUI.backgroundColor = new Color(114f/255f, 202f/255f, 237f/255f); //Change node window color
        int num = selected_agent.action_nodes.Count;
        for (int k = 0; k < num; k++)
        {
            //Get the current action node
            ActionNode_GS node = ((ActionNode_GS)selected_agent.action_nodes[k]);
            //Generate a node editor for the current node
            ActionNode_GS_Editor node_window = new ActionNode_GS_Editor(node);
            //Generate the window
            Rect node_rect = GUI.Window(node.GetNodeID(), node.GetCanvasPos(), node_window.DrawNodeWindow, "");
            
            //Move the node if it's position is editable
            if (node.GetEditablePos()) node.SetCanvasPos(node_rect);
            //Update node editor window if the cur node has been modified
            if (node.GetModified()) Repaint();
        }
       
        /*
        //Planning
        GUI.backgroundColor = Color.white;
        window1 = GUI.Window(1, window1, DrawNodeWindow, "Window 1");   // Updates the Rect's when these are dragged
        window2 = GUI.Window(2, window2, DrawNodeWindow, "Window 2");
        window2_ = GUI.Window(4, window2_, DrawNodeWindow, "Window 7");
        */

        EndWindows();

    }

    void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;

        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 4);
    }

    //Get Methods =====================
    public Agent_GS GetSelectedAgent()
    {
        return selected_agent;
    }

    public Vector2 GetMousePos()
    {
        return mouse_pos;
    }

    //Set Methods =====================
    public void SetSelectedAgent(Agent_GS new_agent)
    {
        selected_agent = new_agent;
        if (selected_agent == null) return;
        
        //Iterate all the agent action nodes to initialize them
        int num = selected_agent.action_nodes.Count;
        for (int k = 0; k < num; k++)
        {
            //Get the current action node
            ActionNode_GS node = ((ActionNode_GS)selected_agent.action_nodes[k]);
            //Check if the current action node is initialized
            if (!node.GetInitialized())
            {
                node.Initialize();
            }
        }
        
    }
}

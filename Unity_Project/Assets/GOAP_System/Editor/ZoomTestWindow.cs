using UnityEngine;
using UnityEditor;
using GOAP_S.UI;
using GOAP_S.Planning;
using GOAP_S.AI;

// Helper Rect extension methods
public static class RectExtensions
{
    public static Vector2 TopLeft(this Rect rect)
    {
        return new Vector2(rect.xMin, rect.yMin);
    }

    public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
    {
        Rect result = rect;
        result.x -= pivotPoint.x;
        result.y -= pivotPoint.y;
        result.xMin *= scale;
        result.xMax *= scale;
        result.yMin *= scale;
        result.yMax *= scale;
        result.x += pivotPoint.x;
        result.y += pivotPoint.y;
        return result;
    }
}

public class EditorZoomArea
{
    private const float kEditorWindowTabHeight = 21.0f;
    private static Matrix4x4 _prevGuiMatrix;

    public static Rect Begin(float zoomScale, Rect screenCoordsArea)
    {
        GUI.EndGroup();        // End the group Unity begins automatically for an EditorWindow to clip out the window tab. This allows us to draw outside of the size of the EditorWindow.

        Rect clippedArea = screenCoordsArea.ScaleSizeBy(1.0f / zoomScale, screenCoordsArea.TopLeft());
        clippedArea.y += kEditorWindowTabHeight;
        GUI.BeginGroup(clippedArea);

        _prevGuiMatrix = GUI.matrix;
        Matrix4x4 translation = Matrix4x4.TRS(clippedArea.TopLeft(), Quaternion.identity, Vector3.one);
        Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f));
        GUI.matrix = translation * scale * translation.inverse * GUI.matrix;

        return clippedArea;
    }

    public static void End()
    {
        GUI.matrix = _prevGuiMatrix;
        GUI.EndGroup();
        GUI.BeginGroup(new Rect(0.0f, kEditorWindowTabHeight, Screen.width * 2.0f, Screen.height));
    }
}



public class ZoomTestWindow : EditorWindow
{
    [MenuItem("Window/Zoom Test")]
    private static void Init()
    {
        ZoomTestWindow window = EditorWindow.GetWindow<ZoomTestWindow>("Zoom Test");
        window.minSize = new Vector2(600.0f, 300.0f);
        window.wantsMouseMove = true;
        window.Show();
        EditorWindow.FocusWindowIfItsOpen<ZoomTestWindow>();// FocusWindowIfItsOpen();
    }

    private const float kZoomMin = 0.1f;
    private const float kZoomMax = 10.0f;

    private readonly Rect _zoomArea = new Rect(0.0f, 0.0f, Screen.width * 2.0f, Screen.height * 2.0f);
    private float _zoom = 1.0f;
    private Vector2 _zoomCoordsOrigin = Vector2.zero;

    private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
    {
        return (screenCoords - _zoomArea.TopLeft()) / _zoom + _zoomCoordsOrigin;
    }

    private void DrawZoomArea()
    {
        // Within the zoom area all coordinates are relative to the top left corner of the zoom area
        // with the width and height being scaled versions of the original/unzoomed area's width and height.
        //this->windo

        EditorZoomArea.Begin(_zoom, _zoomArea);

        //GUI.Box(new Rect(0.0f - _zoomCoordsOrigin.x, 0.0f - _zoomCoordsOrigin.y, 100.0f, 25.0f), "Zoomed Box");

        // You can also use GUILayout inside the zoomed area.
        GUILayout.BeginArea(new Rect(000.0f - _zoomCoordsOrigin.x, 0.0f - _zoomCoordsOrigin.y, Screen.width * 2.0f, Screen.height));

        BeginWindows();

        //_zoom_position = Vector2.zero;
        //Rect node_rect = GUILayout.Window(5, new Rect(50,50,50,50), null,"test");

        //GUILayout.Button("Zoomed Button 1");
       // GUILayout.Button("Zoomed Button 2");

        //Focus action node
        ActionNode_GS node = NodeEditor_GS.Instance.selected_agent.action_nodes[0];
        //Focus action node editor
        ActionNode_GS_Editor node_editor = NodeEditor_GS.Instance.action_node_editors[0];
        //Show node window
        /*Rect test_rect = new Rect(node.window_rect);
        test_rect.x += _zoom_position.x * 2.0f;
        test_rect.y += _zoom_position.y * 2.0f;*/
        //_zoom_position = Vector2.zero;
        GUILayout.Window(node.id, node.window_rect, node_editor.DrawUI, node.name, UIConfig_GS.Instance.node_window_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        EndWindows();

        GUILayout.EndArea();

        EditorZoomArea.End();


    }

    private void DrawNonZoomArea()
    {
        GUI.Box(new Rect(0.0f, 200.0f, 600.0f, 50.0f), "Adjust zoom of middle box with slider or mouse wheel.\nMove zoom area dragging with middle mouse button or Alt+left mouse button.");
        //_zoom = EditorGUI.Slider(new Rect(0.0f, 50.0f, 600.0f, 25.0f), _zoom, kZoomMin, kZoomMax);
        //GUI.Box(new Rect(0.0f, 300.0f - 25.0f, 600.0f, 25.0f), "Unzoomed Box");
    }

    private void HandleEvents()
    {
        // Allow adjusting the zoom with the mouse wheel as well. In this case, use the mouse coordinates
        // as the zoom center instead of the top left corner of the zoom area. This is achieved by
        // maintaining an origin that is used as offset when drawing any GUI elements in the zoom area.
        if (Event.current.type == EventType.ScrollWheel)
        {
            Vector2 screenCoordsMousePos = Event.current.mousePosition;
            Vector2 delta = Event.current.delta;
            Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(screenCoordsMousePos);
            float zoomDelta = -delta.y / 150.0f;
            float oldZoom = _zoom;
            _zoom += zoomDelta;
            _zoom = Mathf.Clamp(_zoom, kZoomMin, kZoomMax);
            _zoomCoordsOrigin += (zoomCoordsMousePos - _zoomCoordsOrigin) - (oldZoom / _zoom) * (zoomCoordsMousePos - _zoomCoordsOrigin);

            Event.current.Use();
        }

        // Allow moving the zoom area's origin by dragging with the middle mouse button or dragging
        // with the left mouse button with Alt pressed.
        if (Event.current.type == EventType.MouseDrag &&
            (Event.current.button == 0 && Event.current.modifiers == EventModifiers.Alt) ||
            Event.current.button == 2)
        {
            Vector2 delta = Event.current.delta;
            delta /= _zoom;
            _zoomCoordsOrigin -= delta;

            Repaint();
            //Event.current.Use();
        }
    }

    public void OnGUI()
    {
        HandleEvents();

        // The zoom area clipping is sometimes not fully confined to the passed in rectangle. At certain
        // zoom levels you will get a line of pixels rendered outside of the passed in area because of
        // floating point imprecision in the scaling. Therefore, it is recommended to draw the zoom
        // area first and then draw everything else so that there is no undesired overlap.
        DrawZoomArea();
        DrawNonZoomArea();
    }
}

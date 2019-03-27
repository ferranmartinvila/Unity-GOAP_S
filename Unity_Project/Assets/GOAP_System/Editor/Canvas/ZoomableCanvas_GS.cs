using UnityEngine;
using UnityEditor;
using GOAP_S.AI;
using GOAP_S.Tools;

//http://martinecker.com/martincodes/unity-editor-window-zooming/

namespace GOAP_S.UI
{
    public class ZoomableCanvas_GS : EditorWindow
    {
        //Content fields
        public delegate void WindowResizeCallbackFunction();
        public WindowResizeCallbackFunction on_window_resize_delegate;

        protected Vector2 _window_size = Vector2.zero;
        protected float _zoomable_window_y_margin = 21.0f;

        protected Matrix4x4 _prev_transform_matrix = Matrix4x4.identity;

        protected const float _min_zoom = 0.4f;
        protected const float _max_zoom = 1.2f;
        protected float _current_zoom = 1.0f;
        protected Rect _zoom_area = Rect.zero;
        protected Vector2 _zoom_position = new Vector2(0.0f, ProTools.CANVAS_SIZE * 0.5f);

        protected Texture2D _back_texture = null; //Texture in the background of the window
        protected static Agent_GS _selected_agent = null; //The agent selected by the user

        //Loop Methods ================
        protected void OnInspectorUpdate()
        {
            //Check window size change
            if(_window_size.x != position.width || _window_size.y != position.height)
            {
                //Update zoom area size
                _window_size = new Vector2(position.width, position.height);
                _zoom_area.width = position.width;
                _zoom_area.height = position.height;

                //Call on window resize delegate
                if (on_window_resize_delegate != null)
                {
                    on_window_resize_delegate();
                }
            }
        }

        protected void HandleZoomInput()
        {
            if (Event.current.type == EventType.ScrollWheel)
            {
                Vector2 screenCoordsMousePos = Event.current.mousePosition;
                Vector2 delta = Event.current.delta;
                Vector2 zoomCoordsMousePos = ScreenCoordsToZoomCoords(screenCoordsMousePos);
                float zoomDelta = -delta.y / 150.0f;
                float oldZoom = _current_zoom;
                _current_zoom += zoomDelta;
                _current_zoom = Mathf.Clamp(_current_zoom, _min_zoom, _max_zoom);
                _zoom_position += (zoomCoordsMousePos - _zoom_position) - (oldZoom / _current_zoom) * (zoomCoordsMousePos - _zoom_position);

                Event.current.Use();
            }

            if (Event.current.type == EventType.MouseDrag &&
                (Event.current.button == 0 && Event.current.modifiers == EventModifiers.Alt) ||
                Event.current.button == 2)
            {
                Vector2 delta = Event.current.delta;
                delta /= _current_zoom;
                _zoom_position -= delta;

                Repaint();
            }
        }

        //Zoom Methods ================
        public Vector2 ScreenCoordsToZoomCoords(Vector2 screen_coords)
        {
            return (screen_coords - _zoom_area.TopLeft()) / _current_zoom + _zoom_position;
        }

        protected Rect BeginZoomableLayout()
        {
            GUI.EndGroup();        // End the group Unity begins automatically for an EditorWindow to clip out the window tab. This allows us to draw outside of the size of the EditorWindow.

            Rect clippedArea = _zoom_area.ScaleSizeBy(1.0f / _current_zoom, _zoom_area.TopLeft());
            clippedArea.y += _zoomable_window_y_margin;
            clippedArea.x -= _zoom_position.x;
            clippedArea.width += Mathf.Abs(clippedArea.width /_current_zoom);
            
            GUI.BeginGroup(clippedArea);

            _prev_transform_matrix = GUI.matrix;
            Matrix4x4 translation = Matrix4x4.TRS(clippedArea.TopLeft(), Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(_current_zoom, _current_zoom, 1.0f));
            GUI.matrix = translation * scale * translation.inverse * GUI.matrix;

            return clippedArea;
        }

        protected void EndZoomableLayout()
        {
            GUI.matrix = _prev_transform_matrix;
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(0.0f, _zoomable_window_y_margin, position.width, position.height));
        }

        //Get/Set Methods =============
        public Agent_GS selected_agent
        {
            get
            {
                return _selected_agent;
            }
            set
            {
                _selected_agent = value;
            }
        }
    }
}

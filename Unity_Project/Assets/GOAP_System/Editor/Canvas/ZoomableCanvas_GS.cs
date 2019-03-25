using UnityEngine;
using UnityEditor;
using GOAP_S.Tools;
using GOAP_S.AI;

//http://martinecker.com/martincodes/unity-editor-window-zooming/

namespace GOAP_S.UI
{
    public class ZoomableCanvas_GS : EditorWindow
    {
        //Content fields
        public delegate void WindowResizeCallbackFunction();
        public WindowResizeCallbackFunction on_window_resize_delegate;

        protected Vector2 _window_size = Vector2.zero;
        protected float _zoomable_window_y_margin = 25.0f;
        protected float _zoomable_window_x_margin = 0.0f;

        protected Matrix4x4 _prev_transform_matrix = Matrix4x4.identity;

        protected const float _min_zoom = 0.4f;
        protected const float _max_zoom = 1.2f;
        protected float _current_zoom = 1.0f;
        protected Rect _zoom_area = Rect.zero;
        protected Vector2 _zoom_position = Vector2.zero;

        protected Vector2 _mouse_position = Vector2.zero; //Used to track the mouse position in this window
        protected Texture2D _back_texture = null; //Texture in the background of the window
        protected static Agent_GS _selected_agent = null; //The agent selected by the user

        //Loop Methods ================
        private void OnInspectorUpdate()
        {
            //Check window size change
            if(_window_size.x != position.width || _window_size.y != position.height)
            {
                //Update size
                _window_size = new Vector2(position.width, position.height);
                _zoom_area = position;
                //Call on window resize delegate
                if (on_window_resize_delegate != null)
                {
                    on_window_resize_delegate();
                }
            }
        }

        protected void HandleZoomInput()
        {
            if(Event.current.type == EventType.ScrollWheel)
            {
                Vector2 delta = Event.current.delta;
                if ((delta.y > 0.0f && Mathf.Abs(_current_zoom - _min_zoom) > 0.001f) || (delta.y < 0.0f && Mathf.Abs(_current_zoom - _max_zoom) > 0.001f))
                {
                    _mouse_position = Event.current.mousePosition;
                    Vector2 mouse_position_in_zoom = ScreenCoordsToZoomCoords(_mouse_position);
                    float zoom_delta = -delta.y / 150.0f;
                    float old_zoom = _current_zoom;
                    _current_zoom += zoom_delta;
                    _current_zoom = Mathf.Clamp(_current_zoom, _min_zoom, _max_zoom);
                    _zoom_position += (mouse_position_in_zoom - _zoom_position) - (old_zoom / _current_zoom) * (mouse_position_in_zoom - _zoom_position);
                    _zoomable_window_y_margin += (zoom_delta * 85.0f);
                    _zoomable_window_x_margin += (zoom_delta * 270.0f);

                    Event.current.Use();
                }
            }

            if (Event.current.type == EventType.MouseDrag && (Event.current.button == 0 && Event.current.modifiers == EventModifiers.Alt) || Event.current.button == 2)
            {
                Vector2 delta = Event.current.delta;
                delta /= _current_zoom;
                _zoom_position += delta * 0.3f;
                Repaint();
            }
        }

        //Zoom Methods ================
        protected Vector2 ScreenCoordsToZoomCoords(Vector2 screen_coords)
        {
            Vector2 test = _zoom_area.TopLeft();
            test.x -= _zoomable_window_x_margin; //TODO
            test.y -= _zoomable_window_y_margin;
            return (screen_coords - test) / _current_zoom + _zoom_position;
        }

        protected Rect BeginZoomableLayout()
        {
            GUI.EndGroup();

            Rect clipped_area = _zoom_area.Scale(1.0f / _current_zoom, _zoom_area.TopLeft());

            clipped_area.x += _zoomable_window_x_margin;
            clipped_area.y += _zoomable_window_y_margin;

            GUI.BeginGroup(clipped_area);

            _prev_transform_matrix = GUI.matrix;
            Matrix4x4 translation = Matrix4x4.TRS(clipped_area.TopLeft(), Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(_current_zoom, _current_zoom, 1.0f));
            GUI.matrix = translation * scale * translation.inverse * GUI.matrix;

            return clipped_area;
        }

        protected void EndZoomableLayout()
        {
            GUI.matrix = _prev_transform_matrix;
            GUI.EndGroup();
            //Debug.Log(_current_zoom);
            GUI.BeginGroup(new Rect(_zoomable_window_x_margin, _zoomable_window_y_margin, Screen.width / _current_zoom, Screen.height / _current_zoom));
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

        public Vector2 mouse_position
        {
            get
            {
                return _mouse_position;
            }
        }
    }
}

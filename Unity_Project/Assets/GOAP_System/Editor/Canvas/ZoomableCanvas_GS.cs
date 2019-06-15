using UnityEngine;
using UnityEditor;
using GOAP_S.AI;
using GOAP_S.Tools;

//Thanks! http://martinecker.com/martincodes/unity-editor-window-zooming/

namespace GOAP_S.UI
{
    public class ZoomableCanvas_GS : EditorWindow
    {
        //Resize delegate
        public delegate void WindowResizeCallbackFunction();
        public WindowResizeCallbackFunction on_window_resize_delegate;
        //Content fields
        //Sizes/transform
        protected Matrix4x4 _prev_transform_matrix = Matrix4x4.identity;
        protected Vector2 _window_size = Vector2.zero;
        protected Vector2 _canvas_size = Vector2.zero;
        protected const float _zoomable_window_y_margin = 24.0f;
        //Zoom
        protected Rect _zoom_area = Rect.zero;
        protected const float _min_zoom = 0.4f;
        protected const float _max_zoom = 1.2f;
        protected float _current_zoom = 1.0f;
        protected Vector2 _zoom_position;
        //Editor commons
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

        protected virtual void HandleNoAgentInput()
        {

        }

        protected virtual void HandleInput()
        {
            //Zoom input
            HandleZoomInput();
        }

        protected void HandleZoomInput()
        {
            if (Event.current.type == EventType.ScrollWheel)
            {
                float zoom_delta = -Event.current.delta.y / 50.0f;
                if ((_current_zoom < _max_zoom && zoom_delta > 0.0f) || (_current_zoom > _min_zoom && zoom_delta < 0.0f))
                {
                    float prev_zoom = _current_zoom;
                    _current_zoom += zoom_delta;
                    _current_zoom = Mathf.Clamp(_current_zoom, _min_zoom, _max_zoom);

                    float delta_x = (position.width - (position.width / _current_zoom) - (position.width - (position.width / prev_zoom))) * 0.5f;
                    float delta_y = (position.height - (position.height / _current_zoom) - (position.height - (position.height / prev_zoom))) * 0.5f;

                    _zoom_position.x += delta_x;
                    _zoom_position.y += delta_y;

                    ClampZoomPosition(new Vector2(-delta_x, -delta_y));

                    Event.current.Use();
                }
            }

            else if (Event.current.type == EventType.MouseDrag &&
                (Event.current.button == 0 && Event.current.modifiers == EventModifiers.Alt) ||
                Event.current.button == 2)
            {
                Vector2 delta = Event.current.delta;
                delta /= _current_zoom;
                _zoom_position -= delta;

                ClampZoomPosition(-delta);

                Repaint();
            }
        }

        //Zoom Methods ================
        public Vector2 ScreenCoordsToZoomCoords(Vector2 screen_coords)
        {
            return new Vector2(_zoom_position.x + ((screen_coords.x - position.position.x) / _current_zoom), _zoom_position.y + ((screen_coords.y - position.position.y) / _current_zoom));
        }

        public Vector2 ZoomCoordsToScreenCoords(Vector2 zoom_coords)
        {
            Debug.Log(zoom_coords);
            Debug.Log(_current_zoom);
            return zoom_coords * _current_zoom;
        }

        protected void ClampZoomPosition(Vector2 delta)
        {
            if(_zoom_position.x < 0.0f)
            {
                _zoom_position.x = 0.0f;
            }
            if(_zoom_position.y < 0.0f)
            {
                _zoom_position.y = 0.0f;
            }
            if (_zoom_position.x + (position.width / _current_zoom) > _canvas_size.x)
            {
                _zoom_position.x -= delta.x;
            }
            if(_zoom_position.y + (position.height / _current_zoom) > _canvas_size.y)
            {
                _zoom_position.y -= delta.y;
            }
        }

        protected Rect BeginZoomableLayout()
        {
            GUI.EndGroup();        // End the group Unity begins automatically for an EditorWindow to clip out the window tab. This allows us to draw outside of the size of the EditorWindow.

            Rect clipped_area = _zoom_area.Scale(1.0f / _current_zoom, _zoom_area.TopLeft());
            clipped_area.y += _zoomable_window_y_margin;
            clipped_area.width += Mathf.Abs(clipped_area.width / _current_zoom);

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

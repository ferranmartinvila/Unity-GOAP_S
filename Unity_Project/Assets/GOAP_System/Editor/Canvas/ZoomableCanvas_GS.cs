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
        protected const float _zoomable_window_y_margin = 24.0f;

        protected Matrix4x4 _prev_transform_matrix = Matrix4x4.identity;

        protected const float _min_zoom = 0.4f;
        protected const float _max_zoom = 1.2f;
        protected float _current_zoom = 1.0f;
        protected Rect _zoom_area = Rect.zero;
        protected Vector2 _zoom_position = new Vector2(ProTools.CANVAS_SIZE * 0.5f, ProTools.CANVAS_SIZE * 0.5f); //Canvas center

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
                /*Vector2 screenCoordsMousePos = Event.current.mousePosition;
                Vector2 delta = Event.current.delta;
                Vector2 zoomCoordsMousePos = ScreenCoordsToZoomCoords(screenCoordsMousePos);*/

                float zoomDelta = -Event.current.delta.y / 50.0f;
                if ((_current_zoom < _max_zoom && zoomDelta > 0.0f) || (_current_zoom > _min_zoom && zoomDelta < 0.0f))
                {
                    float oldZoom = _current_zoom;
                    _current_zoom += zoomDelta;
                    _current_zoom = Mathf.Clamp(_current_zoom, _min_zoom, _max_zoom);

                    float delta_x = (position.width - (position.width / _current_zoom) - (position.width - (position.width / oldZoom))) * 0.5f;
                    float delta_y = (position.height - (position.height / _current_zoom) - (position.height - (position.height / oldZoom))) * 0.5f;

                    /*float new_delta_x = (position.width - (position.width / _current_zoom)) * 0.5f;

                    Debug.Log("Old " + old_delta_x);
                    Debug.Log("New " + new_delta_x);*/

                    _zoom_position.x += delta_x;
                    _zoom_position.y += delta_y;

                    ClampZoomPosition(new Vector2(-delta_x, -delta_y));

                    //_zoom_position.x += new_delta_x;

                    /*float zoom_delta = _current_zoom - oldZoom;
                    _zoom_position.x += (position.width * 0.5f * zoomDelta);
                    _zoom_position.y += (position.height * 0.5f * zoomDelta);

                    Debug.Log("Current Zoom: " + _current_zoom);

                    /*float delta_x = ((position.width * 0.5f) * oldZoom) - ((position.width * 0.5f) * _current_zoom);
                    _zoom_position.x -= delta_x;
                    float delta_y = ((position.height * 0.5f) * oldZoom) - ((position.height * 0.5f) * _current_zoom);
                    _zoom_position.y -= delta_y;

                    //_zoom_position = (zoomCoordsMousePos - _zoom_position) - (oldZoom / _current_zoom) * (zoomCoordsMousePos - _zoom_position);

                    */

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
/*            Vector2 trans_coords = screen_coords - position.position;

            Vector2 final_coords = _zoom_position;
            final_coords.x += (trans_coords.x / _current_zoom);
            final_coords.y += (trans_coords.y / _current_zoom);*/

            return new Vector2(_zoom_position.x + ((screen_coords.x - position.position.x) / _current_zoom), _zoom_position.y + ((screen_coords.y - position.position.y) / _current_zoom));
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

            /*float x_delta = (position.width - (position.width / _current_zoom)) *0.5f;
            float x_p = _zoom_position.x;
            float width_x = position.width / _current_zoom;
            Debug.Log("X_Delta " + x_delta);
            Debug.Log("XP: " + x_p);
            Debug.Log("WX: " + width_x);*/

            if (_zoom_position.x + (position.width / _current_zoom) > 3800.0f)
            {
                _zoom_position.x -= delta.x;
            }
            if(_zoom_position.y + (position.height / _current_zoom) > 3800.0f)
            {
                _zoom_position.y -= delta.y;
            }
        }

        protected Rect BeginZoomableLayout()
        {
            GUI.EndGroup();        // End the group Unity begins automatically for an EditorWindow to clip out the window tab. This allows us to draw outside of the size of the EditorWindow.

            Rect clippedArea = _zoom_area.ScaleSizeBy(1.0f / _current_zoom, _zoom_area.TopLeft());
            clippedArea.y += _zoomable_window_y_margin;
            clippedArea.width += Mathf.Abs(clippedArea.width /_current_zoom) + _zoom_position.x;

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

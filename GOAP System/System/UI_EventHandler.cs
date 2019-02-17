using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP_S.UI
{
    public class UI_EventHandler
    {
        public enum UI_EVENTS
        {
            NONE = 0,
            OPEN_NODE_EDITOR,
            OPEN_NODE_EDITOR_POP_MENU,
            OPEN_ACTION_SELECT_MENU,
            OPEN_MANAGER_MENU
        }

        //Stack where all ui events are send
        static private Stack<UI_EVENTS> events_stack = new Stack<UI_EVENTS>();

        //Recive event method(send from the point of view of the user)
        public void SendEvent(UI_EVENTS event_)
        {
            events_stack.Push(event_);
        }

        //Send event(recieve from the point of view of the user)
        public UI_EVENTS GetEvent(UI_EVENTS event_type)
        {
            if (events_stack.Peek() == event_type)
            {
                return events_stack.Pop();
            }
            else return UI_EVENTS.NONE;
        }

    }
}

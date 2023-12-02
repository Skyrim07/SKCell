using System;
namespace SKCell
{
    public class SKEvent
    {
        public Action action;
        public float param;

        public SKEvent() { }
        public SKEvent(Action action)
        {
            this.action = action;
        }
    }
}

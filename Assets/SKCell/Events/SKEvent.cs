using System;
using System.Collections.Generic;

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

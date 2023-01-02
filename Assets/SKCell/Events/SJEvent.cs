using System;
using System.Collections.Generic;

public class SJEvent
{
    public Action action;
    public float param;

    public SJEvent() { }
    public SJEvent(Action action)
    {
        this.action = action;
    }
}

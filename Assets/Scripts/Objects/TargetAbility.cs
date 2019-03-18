using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAbility {

    Point location;

    public TargetAbility(Point location)
    {
        this.location = location;
    }

    public bool confirmTarget(Point target)
    {
        if(target == location)
        {
            return true;
        }
        return false;
    }

    public void setLocation(Point location)
    {
        this.location = location;
    }

    public Point getLocation()
    {
        return location;
    }
}

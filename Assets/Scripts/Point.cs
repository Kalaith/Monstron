using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point  {

    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Point operator *(Point p1, int p2)
    {
        return new Point(p1.x * p2, p1.y * p2);
    }

    public static Point operator +(Point p1, Point p2)
    {
        return new Point(p1.x + p2.x, p1.y + p2.y);
    }

    public static bool operator ==(Point p1, Point p2)
    {
        if (p1.x == p2.x && p1.y == p2.y)
        {
            return true;
        }
        return false;
    }

    public static bool operator !=(Point p1, Point p2)
    {
        if (p1.x == p2.x && p1.y == p2.y)
        {
            return false;
        }
        return true;
    }

    public override string ToString()
    {
        return "P"+x+":"+y;
    }
}

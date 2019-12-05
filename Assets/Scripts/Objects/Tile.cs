using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TILE_TYPE {Empty, Ground, Wall, Corridor, Floor, Entry };

[Serializable]
public class Tile  {

    public TILE_TYPE type = TILE_TYPE.Empty;
    public int x;
    public int y;
    public int cost;
    public Point point;

    public Tile(TILE_TYPE type, int x, int y)
    {
        this.type = type;
        this.x = x;
        this.y = y;
        point = new Point(x, y);
    }

    public override string ToString()
    {
        return "Tile_Type_"+type+"_Pos_"+x+"-"+y;
    }

    public TILE_TYPE Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
    }
    public int X
    {
        get
        {
            return x;
        }
    }
    public int Y
    {
        get
        {
            return y;
        }
    }
    public int Cost
    {
        get
        {
            return cost;
        }
        set
        {
            cost = value;
        }
    }
    public static bool operator ==(Tile t1, Tile t2)
    {
        if (object.ReferenceEquals(t1, null) || object.ReferenceEquals(t2, null))
        {
            return false;
        }
        if (t1.x == t2.x && t1.y == t2.y)
        {
            return true;
        }
        return false;
    }

    public static bool operator !=(Tile t1, Tile t2)
    {
        if(object.ReferenceEquals(t1, null) || object.ReferenceEquals(t2, null))
        {
            return true;
        }
        if (t1.x == t2.x && t1.y == t2.y)
        {
            return false;
        }
        return true;
    }

    public int distanceToTile(Point start)
    {
        return (int)Vector2.Distance(new Vector2(x, y), new Vector2(start.x, start.y)); //System.Math.Sqrt((start.x - x) ^ 2 + (start.y - y)^2);
    }

    // want to get the distance but be able to not return when the distance is from an angled tile, so diag true should only return straight lines
    // dont like the name but it will do for now.. would also like to check if there are any walls inbetween the tiles
    public bool tileDiagonal(Point start)
    {
        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(start.x, start.y));
        if (distance % 1 == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

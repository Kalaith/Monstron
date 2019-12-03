using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The type of room can effect what sort of items and monsters spawn in it
public enum ROOM_TYPE { EMPTY, NEST, DEN }

public class Room {

    // Public or modifiers maybe public to keep it cheap
    public int startx;
    public int starty;
    public int width;
    public int height;
    ROOM_TYPE type;
    // This is the number of entrances into the room
    public int max_connections;
    public int connections;
    public int size;

    // Monsters that are assigned to this room
    public List<Monster> monsters;

    public Room(int startx, int starty, int width, int height, int connections, ROOM_TYPE type = ROOM_TYPE.EMPTY)
    {
        this.startx = startx;
        this.starty = starty;
        this.width = width;
        this.height = height;
        this.type = type;
        max_connections = connections;
        size = width * height;
        monsters = new List<Monster>();
    }

    internal Point getCenter()
    {
        return new Point((int)(width / 2) + startx, (int)(height / 2) + starty);
    }

    // Is the position inside the room
    public bool inRoom(int x, int y)
    {
        // Add and Remove one to account for walls
        if(x > startx && x < startx+width || y > starty && y < starty+height)
        {
            return true;
        }
        return false;
    }

    // does the room passed in overlap this room.
    public bool roomIntersects(Room room)
    {
       
        int ulx = System.Math.Max(startx, room.startx);
        int uly = System.Math.Max(starty, room.starty);
        int lrx = System.Math.Min(startx + width, room.startx + room.width);
        int lry = System.Math.Min(starty + height, room.starty + room.height);

        return ulx <= lrx && uly <= lry;
    }

    public override string ToString()
    {
        return "Room_SX"+startx+"SY"+starty+"W"+width+"H"+height;
    }

    public Point getDirPoint(int dir, RandomGen rand)
    {
        int x = startx;
        int y = starty;

        // we increment 1 on start and decrement 1 on end so we dont get a corner 

        if (dir == 0) // North
        {
            x = rand.range(startx+1, startx + width-1);
            y = starty + height-1;
        }
        if (dir == 1) // East
        {
            x = startx + width-1;
            y = rand.range(starty+1, starty + height-1);
        }
        if (dir == 2) // South
        {
            x = rand.range(startx+1, startx + width-1);
            y = starty;
        }
        if (dir == 3) // West
        {
            x = startx;
            y = rand.range(starty+1, starty + height-1);
        }

        return new Point(x, y);
    }

    internal void placeMonster(Monster m)
    {
        monsters.Add(m);
    }

    public static bool operator ==(Room r1, Room r2)
    {
        if (r1.startx == r2.startx && r1.starty == r2.starty)
        {
            return true;
        }
        return false;
    }

    public static bool operator !=(Room r1, Room r2)
    {
        if (r1.startx == r2.startx && r1.starty == r2.starty)
        {
            return false;
        }
        return true;
    }

    internal Point getRandomLocation(RandomGen rand)
    {
        int ranX = rand.range(startx+1, startx+width-1);
        int ranY = rand.range(starty + 1, starty + height - 1);

        return new Point(ranX, ranY);
    }
}

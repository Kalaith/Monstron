using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map {

    int width;
    int height;

    Tile[,] map;
    public List<Room> rooms;
    public Point startPosition;
    Tile teleport; // teleport changes maps
    Tile exit; // exit leaves the dungeon

    public List<Item> items; // items on the map

    public Path_TileGraph TileGraph;

    public Map(int width, int height)
    {
        this.width = width;
        this.height = height;
        items = new List<Item>();
        map = new Tile[width,height];
        Debug.Log("W"+width+" H"+height);

    }

    public void initPathfinding()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y].Type != TILE_TYPE.Wall && map[x, y].Type != TILE_TYPE.Empty)
                {
                    map[x, y].Cost = 1;
                }
            }
        }

        TileGraph = new Path_TileGraph(this);

    }
    // Init map with the type based in
    public void fillMap(TILE_TYPE type)
    {
        //Debug.Log("Filling a map with W" + width+"H"+height);
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = new Tile(type, x, y);
                if(type == TILE_TYPE.Empty || type == TILE_TYPE.Wall)
                {
                    map[x, y].Cost = 0;
                } else
                {
                    map[x, y].Cost = 1;
                }
                //Debug.Log(map[x, y].ToString());
            }
        }
    }

    // Init map with the type based in
    public void addOutsideWall()
    {
        //Debug.Log("Filling a map with W" + width+"H"+height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(x == 0 || y == 0 || x == width-1 || y == height-1)
                {
                    map[x, y].Type = TILE_TYPE.Wall;
                    map[x, y].Cost = 0;
                }
                
                //Debug.Log(map[x, y].ToString());
            }
        }
    }

    public bool isPassable(int x, int y)
    {
        if (map != null)
        {
            if (x > 0 && x < width && y > 0 && y < height)
            {
                if (getTileTypeAt(x, y) == TILE_TYPE.Empty || getTileTypeAt(x, y) == TILE_TYPE.Wall)
                    return false;

                return true;
            }
        }
        // outside the bounds of the map
        return false;
    }

    public TILE_TYPE getTileTypeAt(int x, int y)
    {
        if (x > 0 && x < width && y > 0 && y < height)
        {
            return map[x, y].Type;
        }
        return TILE_TYPE.Empty;
    }

    // Should this be deciding if to add a room or not.
    public bool addRoom(int startx, int starty, int roomWidth, int roomHeight, int connections)
    {
        if (rooms == null)
            rooms = new List<Room>();

        Room newRoom = new Room(startx, starty, roomWidth, roomHeight, connections);

        bool intersects = false;
        foreach(Room room in rooms)
        {
            if(newRoom.roomIntersects(room))
            {
                intersects = true;
            }
        }

        if (!intersects)
        {
            rooms.Add(newRoom);
            placeRoom(startx, starty, roomWidth, roomHeight);
            //Debug.Log("Room "+rooms.Count+"#"+newRoom.ToString());
            return true;
        }
        return false;
    }

    internal ITEM_TYPE getItemTypeAt(int x, int y)
    {
        foreach(Item item in items)
        {
            if(item.x == x && item.y == y)
            {
                return item.itemType;
            }
        }

        return ITEM_TYPE.NOTHING;
    }

    internal Item getItemAt(int x, int y)
    {
        foreach (Item item in items)
        {
            if (item.x == x && item.y == y)
            {
                return item;
            }
        }

        return null;
    }

    public List<Tile> getNeighbours(Tile tile, bool diagonal = false)
    {
        List<Tile> neighbours = new List<Tile>();
        if(tile.X != 0) {
            neighbours.Add(map[tile.X - 1, tile.Y]);
        }
        if (tile.Y != 0)
        {
            neighbours.Add(map[tile.X, tile.Y - 1]);
        }
        if (tile.X != width - 1)
        {
            neighbours.Add(map[tile.X + 1, tile.Y]);
        }
        if (tile.Y != height - 1)
        {
            neighbours.Add(map[tile.X, tile.Y + 1]);
        }
        if(diagonal)
        {
            if(tile.X != width - 1 && tile.Y != height - 1)
                neighbours.Add(map[tile.X + 1, tile.Y + 1]);

            if (tile.X != 0 && tile.Y != 0)
                neighbours.Add(map[tile.X - 1, tile.Y - 1]);

            if(tile.X != 0 && tile.Y != height - 1)
                neighbours.Add(map[tile.X - 1, tile.Y + 1]);

            if(tile.X != width-1 && tile.Y != 0)
                neighbours.Add(map[tile.X + 1, tile.Y - 1]);
        }

        return neighbours;
    }

    public void placeRoom(int startx, int starty, int roomWidth, int roomHeight)
    {

        for(int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                if (x == 0 || y == 0 || x == roomWidth-1 || y == roomHeight-1)
                {
                    map[x + startx, y + starty].Type = TILE_TYPE.Wall;
                    map[x + startx, y + starty].Cost = 0;
                } else
                {
                    map[x + startx, y + starty].Type = TILE_TYPE.Floor;
                    map[x + startx, y + starty].Cost = 1;
                }

            }
        }
    }

    public int Width
    {
        get
        {
            return width;
        }

        set
        {
            width = value;
        }
    }

    public int Height
    {
        get
        {
            return height;
        }

        set
        {
            height = value;
        }
    }

    public Tile Teleport
    {
        get
        {
            return teleport;
        }

        set
        {
            teleport = value;
        }
    }

    public Tile Exit
    {
        get
        {
            return exit;
        }

        set
        {
            exit = value;
        }
    }
    public Tile[,] GameMap
    {
        get
        {
            return map;
        }

        set
        {
            map = value;
        }
    }

    public Tile getTileAt(Point point)
    {
        // getting a tile outside of the bounds
        if(point.x < 0 || point.x > width && point.y < 0 || point.y > height)
        {
            return null;
        }
        return map[point.x, point.y];
    }

    public Tile getTileAt(int x, int y)
    {
        // getting a tile outside of the bounds
        if (x < 0 || x > width && y < 0 || y > height)
        {
            return null;
        }
        return map[x, y];
    }

    // remove a rooms walls if its an external wall touching the boundry it will be added in outsidewalls
    internal void removeRoomWalls(Room room)
    {
        for (int x = 0; x < room.width; x++)
        {
            for (int y = 0; y < room.height; y++)
            {
                if (x == 0 || y == 0 || x == room.width - 1 || y == room.height - 1)
                {
                    map[x + room.startx, y + room.starty].Type = TILE_TYPE.Floor;
                    map[x + room.startx, y + room.starty].Cost = 1;
                }
            }
        }
    }

    public bool isTilePartOfRoom(Tile t)
    {
        foreach(Room r in rooms)
        {
            if (r.inRoom(t.X, t.Y))
                return true;
        }
        return false;
    }
    
}

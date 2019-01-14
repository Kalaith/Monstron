using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// use to generate a map
public class MapGenerator {

    Map genMap;
    RandomGen rand;
    int seed;
    int width;
    int height;
    int rooms;

    public MapGenerator(RandomGen rand, int width, int height, int rooms)
    {
        this.rand = rand;

        this.width = width;
        this.height = height;
        this.rooms = rooms;
    }

    public Map createTownMap()
    {
        Debug.Log("Creating dungeon a Map");
        // create a new map, assign the seed
        genMap = new Map(width, height);
        genMap.fillMap(TILE_TYPE.Corridor);

        // in the case of the town map then we are generating houses instead of rooms but it works for nowpopl.
        createRooms(10, 10, 15, 15);
        addRoomDoors(2);

        genMap.addOutsideWall();

        return genMap;
    }

    public Map createDungeonMap()
    { 
        Debug.Log("Creating dungeon a Map");
        // create a new map, assign the seed
        genMap = new Map(width, height);
        genMap.fillMap(TILE_TYPE.Wall);

        createRooms();
        createMaze();
        addRoomDoors();
        addTeleport();
        addExit();
        //createCorridors();
        genMap.addOutsideWall();

        return genMap;
    }


    // Generates a maze from wall tiles in a map.
    public void createMaze()
    {
        // find available options for the next tile look at N, S, E, W remove any of these directions that are Ground
        List<Tile> potentials = new List<Tile>();
        List<Tile> placedTiles = new List<Tile>();

        // start from point x, y that is not against the edge or any square around it is not a ground tile.. change it to ground
        int mx = 0;
        int my = 0;
        do
        {
            mx = rand.range(1, genMap.Width-1);
            my = rand.range(1, genMap.Height-1);
        } while (genMap.getTileTypeAt(mx, my) != TILE_TYPE.Wall);

        Tile nextTile = genMap.GameMap[mx, my];
        // we get a point anyways, lets use this for the player

        nextTile.Type = TILE_TYPE.Corridor;
        placedTiles.Add(nextTile);
        Debug.Log("Placed " + placedTiles.Count + " Tile " + nextTile.point.ToString());
        //Debug.Log("Starting Position: "+nextTile.ToString());
        foreach (Tile tile in genMap.getNeighbours(nextTile))
        {
            if (mazeCheckValidTile(tile))
            {
                potentials.Add(tile);
            }
        }
        //Debug.Log("Potentials from starting position"+potentials.Count);
        // when we are looking at a tile we want to make sure it has 3 walls around it and no wall from the direction we came in only these rooms get added to potential
        while (potentials.Count != 0)
        {
        //for (int i = 0; i < 3; i++)
        //{
            // choose one at random update the x, y position record the rest as potentials
            nextTile = potentials[rand.range(0, potentials.Count)];
            if (mazeCheckValidTile(nextTile))
            {
                //Debug.Log(nextTile.ToString());
                // are we going to run into the issue that every tile is filled out again..
                nextTile.Type = TILE_TYPE.Corridor;

                // this tile goes from potential to placed.
                potentials.Remove(nextTile);
                placedTiles.Add(nextTile);

                List<Tile> potNeighbours = genMap.getNeighbours(nextTile);
                //Debug.Log("Potential Neighbours Count " + potNeighbours.Count);
                foreach (Tile t in potNeighbours)
                {
                    // dont check an already placed tile
                    if (!placedTiles.Contains(t))
                    {
                        if (mazeCheckValidTile(t))
                        {
                            //Debug.Log("Tile " + t.ToString() + " is a potential path");
                            potentials.Add(t);
                        }
                    }
                }
            } else
            {
                potentials.Remove(nextTile);
            }
        }
        Debug.Log("Placed At: " + placedTiles[0].point.ToString());
        genMap.startPosition = placedTiles[0].point;
        //Debug.Log("We have this many potentials "+potentials.Count);

        // repeat
        // when there is no available options go back to previous potentials, recheck make sure we havn't ruled this out by other changes
        // repeat until no futher potentials

    }

    // Checks the tiles neighbours to ensure that the tile passed in is a valid potential
    public bool mazeCheckValidTile(Tile tile)
    {
        List<Tile> wallCheck = genMap.getNeighbours(tile, true);
        // if there isn't 8 entries in the list then at least one entry was invalid so this can't be a potential target
        //Debug.Log("Tile "+tile.ToString()+" has "+wallCheck.Count+" neighbours");
        if (wallCheck.Count == 8)
        {
            // only allow one ground neighbour tile, this allows us to not care what the lead in tile is
            int groundCount = 0;
            foreach (Tile neighbour in wallCheck)
            {
                if(neighbour.Type!=TILE_TYPE.Wall)
                {
                    groundCount++;
                }
                //Debug.Log("Checking Tile "+neighbour.ToString() + " as a Wall and not the same tile as the one we came from"+ (neighbour != fromTile));
                if (groundCount >= 3)
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    // Try and avoid placing on the same place as eachother, its still random atm, so it could happen, so just trying to fix it once.
    public void addTeleport()
    {
        Room room = genMap.rooms[rand.range(0, genMap.rooms.Count)];
        Tile t = genMap.getTileAt(room.getRandomLocation(rand));
        if(genMap.Teleport == t)
        {
            genMap.Teleport = genMap.getTileAt(room.getRandomLocation(rand));
        }
        genMap.Teleport = t;
    }

    public void addExit()
    {
        Room room = genMap.rooms[rand.range(0, genMap.rooms.Count)];
        Tile t = genMap.getTileAt(room.getRandomLocation(rand));
        if (genMap.Exit == t)
        {
            genMap.Exit = genMap.getTileAt(room.getRandomLocation(rand));
        }
        genMap.Exit = t;
    }

    public void createRooms(int minWidth = 4, int minHeight = 4, int maxWidth = 10, int maxHeight = 10)
    {
        for (int r = 0; r < rooms; r++)
        {
            int roomWidth = rand.range(minWidth, maxWidth);
            int roomHeight = rand.range(minHeight, maxHeight);

            int startx = rand.range(0, width - roomWidth);
            int starty = rand.range(0, height - roomHeight);

            // Add room returns true but for now we will ignore it and just aim for adding more rooms to account for some conflicts.
            genMap.addRoom(startx, starty, roomWidth, roomHeight, rand.range(1, 6));

        }

    }



    public void addRoomDoors(int max_connections = 0)
    {

        foreach (Room room in genMap.rooms)
        {
            if (max_connections == 0)
            {
                max_connections = room.max_connections;
            }
            int attempts = 0;
            for (int i = room.connections; i < max_connections; i++)
            {
                // find the direction to put the door
                int dirRoom = rand.range(0, 3);
                
                // we need to stop the direction being one of the game borders
                if(room.startx == 0)
                {
                    dirRoom = rand.range(0, 1);
                }

                if(room.starty == 0)
                    dirRoom = rand.range(0, 1);

                if(room.startx+room.width > genMap.Width-1)
                    dirRoom = rand.range(2, 3);

                if(room.starty+room.height > genMap.Height-1)
                    dirRoom = rand.range(0, 1);

                // find the point on that side to add the door
                Point room1P = room.getDirPoint(dirRoom, rand);
                // Do a check to see if the point we chose is touching a corridor
                List<Tile> neighbours = genMap.getNeighbours(genMap.getTileAt(room1P));
                bool valid = false;
                foreach (Tile t in neighbours) { 
                    // Look for a corridor if we dont find one then the point isn't good.
                    if(t.Type == TILE_TYPE.Corridor)
                    {
                        valid = true;
                        break;
                    }
                }
                if (valid)
                {
                    genMap.GameMap[room1P.x, room1P.y].Type = TILE_TYPE.Corridor;
                    room.connections++;
                } else
                {
                    // repeat the loop so we try the connection again.. I can't see where it could enter a infinite loop because a room has 2 deep walls
                    i--;
                    attempts++;
                    if(attempts > 10)
                    {
                        
                        Debug.Log("Reached max attempts");
                        break;
                        
                        
                    }
                }

                // todo add a way to check its reached the maze..
            }

            // failed to make a connection and aborted
            if(room.connections == 0)
            {
                // For now lets just remove all the walls
                
                genMap.removeRoomWalls(room);
            }
        }
    }

    public void createCorridors()
    {
        List<Room> rooms = genMap.rooms;
        for (int r = 0; r < rooms.Count; r++) {
            // For each room go from connections to max connections
            for (int i = rooms[r].connections; i < rooms[r].max_connections; i++)
            {
                
                // if the room already has its max number of connections stop.
                if (rooms[r].max_connections == rooms[r].connections)
                {
                    break;
                }
                // j is the room is want to connect to, reverse the order so first room will look at last room (no particular reason)
                for (int j = rooms.Count-1; j > 0; j--)
                {
                    // we dont want to connect to ourselves
                    if (r != j)
                    {
                        // if the room already has its max number of connections stop.
                        if (rooms[j].max_connections == rooms[j].connections || rooms[r].max_connections == rooms[r].connections)
                        {
                            break;
                        }

                        createConnection(rooms[r], rooms[j]);
                        // We only want to connect room a to room b once
                        // Once we have connected both of the rooms increase there connections by 1
                        rooms[r].connections++;
                        rooms[j].connections++;
                    }

                }
            }
        }
    }

    private bool createConnection(Room room1, Room room2)
    {
        Debug.Log("Creating a connection between "+room1.ToString() + " and " + room2.ToString());
        // which side of the room is the connection coming from
        int dirRoom1 = 0;// rand.Next(0, 3);
        int dirRoom2 = 0;// rand.Next(0, 3);

        // Points from each of the rooms (only issue I can see is that what if one room is against the edge we can't go in that direction.)
        Point room1P = room1.getDirPoint(dirRoom1, rand);
        Point room2P = room2.getDirPoint(dirRoom2, rand);

        Debug.Log(room1P.ToString());
        Debug.Log(room2P.ToString());

        // North we want to go up first
        if (dirRoom1 == 0)
        {
            // y
            if (room1P.y > room2P.y)
            {
                // Since room1.y is greater then room2.y then we are decrementing until i reaches the y of room2
                for (int i = room1P.y; i < room2P.y; i--)
                {
                    foreach (Room room in genMap.rooms)
                    {
                        if (room1 != room)
                        {
                            if (room.inRoom(room1P.x, i))
                            {
                                // have we reached our room
                                if (room == room2)
                                {
                                    // We have reached our goal.
                                    return true;
                                }
                                else
                                {
                                    // we have reached a diffrent room, is it allowing connections
                                    if (room.connections + 2 < room.max_connections)
                                    {
                                        // We can use this room, 2 connections because we will going thru
                                        Debug.Log("We are passing through room: " + room.ToString());
                                    }
                                    else
                                    {
                                        // we can't make a connection to this room, we need to switch axis
                                        // for now fail the connection
                                        Debug.Log("We reached a room we can't form a connection with");
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    
                    genMap.GameMap[room1P.x, i].Type = TILE_TYPE.Ground;
                }
            }
            else
            {
                Debug.Log("r1y " + room1P.y + " less then r2y " + room2P.y);
                // Since room1.x is less then room2.x then we are incrementing until i reaches the x of room2 or we are already on the same x axis and this ends
                for (int i = room1P.y; i < room2P.y; i++)
                {
                    Debug.Log("Tile" + genMap.GameMap[room1P.x, i].ToString());

                    // reached the destination
                    if (room2.inRoom(room1P.x, i))
                    {
                        return true;
                    }
                    Debug.Log("We have hit ground"+ room2.inRoom(room1P.x, i));
                    foreach (Room room in genMap.rooms)
                    {
                        if (room1 != room)
                        {
                            if (room.inRoom(room1P.x, i))
                            {
                                // have we reached our room
                                if (room == room2)
                                {
                                    // We have reached our goal.
                                    return true;
                                }
                                else
                                {
                                    // we have reached a diffrent room, is it allowing connections
                                    if (room.connections + 2 < room.max_connections)
                                    {
                                        // We can use this room, 2 connections because we will going thru
                                        Debug.Log("We are passing through room: " + room.ToString());
                                    }
                                    else
                                    {
                                        // we can't make a connection to this room, we need to switch axis
                                        // for now fail the connection
                                        Debug.Log("We reached a room we can't form a connection with");
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    genMap.GameMap[room1P.x, i].Type = TILE_TYPE.Corridor;
                }
            }

            if (room1P.x > room2P.x)
            {
                Debug.Log("r1x " + room1P.x + " greater then r2x " + room2P.x);
                // Since room1.x is greater then room2.x then we are decrementing until i reaches the x of room2
                for (int i = room1P.x; i > room2P.x; i--)
                {

                    Debug.Log("We have found a ground tile at " + genMap.GameMap[i, room2P.y].ToString());
                    foreach (Room room in genMap.rooms)
                    {
                        if (room1 != room)
                        {
                            if (room.inRoom(i, room2P.y))
                            {
                                Debug.Log("The tile belongs to room: " + room.ToString());
                                // have we reached our room
                                if (room == room2)
                                {
                                    // We have reached our goal.
                                    return true;
                                }
                                else
                                {
                                    // we have reached a diffrent room, is it allowing connections
                                    if (room.connections + 2 < room.max_connections)
                                    {
                                        // We can use this room, 2 connections because we will going thru
                                        Debug.Log("We are passing through room: " + room.ToString());
                                    }
                                    else
                                    {
                                        // we can't make a connection to this room, we need to switch axis
                                        // for now fail the connection
                                        Debug.Log("We reached a room we can't form a connection with");
                                        return false;
                                    }
                                }
                            }
                        }
                    }

                    genMap.GameMap[i, room2P.y].Type = TILE_TYPE.Corridor;
                }
            }
            else
            {
                // Since room1.x is less then room2.x then we are incrementing until i reaches the x of room2 or we are already on the same x axis and this ends
                for (int i = room1P.x; i < room2P.x; i++)
                {
                    foreach (Room room in genMap.rooms)
                    {
                        if (room1 != room)
                        {
                            if (room.inRoom(i, room2P.y))
                            {
                                // have we reached our room
                                if (room == room2)
                                {
                                    // We have reached our goal.
                                    return true;
                                }
                                else
                                {
                                    // we have reached a diffrent room, is it allowing connections
                                    if (room.connections + 2 < room.max_connections)
                                    {
                                        // We can use this room, 2 connections because we will going thru
                                        Debug.Log("We are passing through room: " + room.ToString());
                                    }
                                    else
                                    {
                                        // we can't make a connection to this room, we need to switch axis
                                        // for now fail the connection
                                        Debug.Log("We reached a room we can't form a connection with");
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    genMap.GameMap[i, room2P.y].Type = TILE_TYPE.Ground;
                }
            }

        }

        return false;       
        
    }


}



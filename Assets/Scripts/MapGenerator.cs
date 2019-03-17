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
    List<Tile> placedTiles;
    List<Tile> potentialTiles;

    public MapGenerator(RandomGen rand, int width, int height, int rooms)
    {
        this.rand = rand;

        this.width = width;
        this.height = height;
        this.rooms = rooms;
        placedTiles = new List<Tile>();
        potentialTiles = new List<Tile>();
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

        // create the rooms and add doors
        // A better way of doing this would be to split it into segments and then create a room per segment.

        createRooms();

        genMap.addOutsideWall();
        placeOutsideWalls();

        // we have rooms, need a path to go between them
        createPath();
        addRoomDoors();

        // fill dead space.
        fillInTiles(20);
        //
        // create a maze wth left over space, lets try and make sure the maze is connected to all rooms.
        //createMaze();

        addTeleport();
        addExit();

        //createCorridors();

        return genMap;
    }

    private void createPath()
    {
        int mx;
        int my;
        Tile tile;

        // choose a random starting position, make sure its not also inside a room or room wall
        do {
            mx = rand.range(1, genMap.Width - 1);
            my = rand.range(1, genMap.Height - 1);

            tile = genMap.getTileAt(mx, my);
        } while (placedTiles.Contains(tile));

        genMap.startPosition = tile.point;

        potentialTiles.Add(tile);
        int inf = 0;
        do
        {
            tile = potentialTiles[rand.range(0, potentialTiles.Count)];
            //Debug.Log("Looking at tile: " + tile.ToString());

            if (tile.Type != TILE_TYPE.Floor)
            {
                tile.Type = TILE_TYPE.Corridor;
            }

            placedTiles.Add(tile);
            potentialTiles.Remove(tile);

            List<Tile> neigh = genMap.getNeighbours(tile, false);
            foreach (Tile n in neigh)
            {
                //Debug.Log("Looking at Potential:" + n.ToString());
                if (!placedTiles.Contains(n))
                {

                    // if its a floor then we are inside the room, add the tile as a potential
                    // because if we dont then we can't select any of the cells around it, some of which
                    // may be a way to get to the rest of the dungeon.

                    if (tilesTouchingTile(n) > 1)
                    {
                        placedTiles.Add(n);
                        // it may have been a potential option from another tile
                        // because we have placed near it, it is now cosidered locked in.
                        potentialTiles.Remove(n);
                        //Debug.Log("Removed potential tile was conflicts: "+n.ToString());
                    } else
                    {
                        // we want to add a potential if there is even one option available..
                        // we do not want to remove a  
                        potentialTiles.Add(n);
                        //Debug.Log("Potential Added:" + n.ToString());  
                    }
                } else
                {
                    //Debug.Log("Tile was already placed:" + n.ToString());
                }
            }
            inf++;
            // run enough times for each cell
            if (inf > (width * height))
            {
                Debug.Log("Stop at over 10");
                potentialTiles.Clear();
                break;
            }
            //Debug.Log("Potential Count: "+ potentialTiles.Count);
        } while (potentialTiles.Count != 0);
    }

    public int tilesTouchingTile(Tile t, TILE_TYPE type = TILE_TYPE.Corridor)
    {
        List<Tile> neighbours = genMap.getNeighbours(t);
        int touches = 0;
        foreach (Tile touch in neighbours)
        {
            if (touch.Type == type)
            {
                touches++;
            }
        }

        //Debug.Log(t.ToString()+" matches "+ touches);

        return touches;
    }


    // Out of all of the placed cells how many percs of tiles should it fill in,
    // this will handle maps with non fixed dimentions 
    public void fillInTiles(Double fillInPerc)
    {
        List<Tile> checkTiles = placedTiles;

        int tilesToFillIn = (int)Math.Floor(((fillInPerc / 100) * checkTiles.Count));
        Debug.Log(checkTiles.Count+"::"+fillInPerc+ "Tiles to fill in:" +tilesToFillIn+" - "+ ((fillInPerc / 100)+1)+ "-" + ((fillInPerc / 100) * checkTiles.Count));

        // two checks one for the amount we want to fill in the other for check cells
        while (tilesToFillIn >= 0 && checkTiles.Count != 0)
        {
            Tile t = checkTiles[rand.range(0, checkTiles.Count)];
            // if only care about checking corridors, but avoid the players starting position
            if (t.Type == TILE_TYPE.Corridor && t.point != genMap.startPosition)
            {
                // check all the neighbours, a fill only qualiies if 3 sides are a wall because it then can't be blocking a path.
                List<Tile> neigh = genMap.getNeighbours(t);
                int walls = 0;
                foreach(Tile n in neigh)
                {
                    if(n.Type == TILE_TYPE.Wall)
                    {
                        walls++;
                    }
                }
                if(walls == 3)
                {
                    t.Type = TILE_TYPE.Wall;
                    // this function doesn't handle if it checked one cell and it didn't qualify and then after a fill it does qualify.
                    // we could if this tile was filled add the non wall tile back into checkTiles so it can be rechecked.
                    tilesToFillIn--;
                    // this is no longer a potential starting position.
                    placedTiles.Remove(t);
                }
                checkTiles.Remove(t);
                
            } else
            {
                checkTiles.Remove(t);
            }
        }
    }




    // Checks the tiles neighbours to ensure that the tile passed in is a valid potential
    public bool mazeCheckValidTile(Tile tile)
    {
        // if its already a placed tile then it is a potential tile.
        if(tile.Type == TILE_TYPE.Floor)
        {
            return true;
        }
        List<Tile> wallCheck = genMap.getNeighbours(tile, true);
        // if there isn't 8 entries in the list then at least one entry was invalid so this can't be a potential target
        //Debug.Log("Tile "+tile.ToString()+" has "+wallCheck.Count+" neighbours");
        int borders = 0;

        if (wallCheck.Count == 8)
        {
            // only allow one ground neighbour tile, this allows us to not care what the lead in tile is
            int groundCount = 0;
            foreach (Tile neighbour in wallCheck)
            {
                if(neighbour.Type!=TILE_TYPE.Wall && neighbour.Type != TILE_TYPE.Floor)
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
            bool roomAdded = genMap.addRoom(startx, starty, roomWidth, roomHeight, rand.range(1, 6));

            // Assign all tiles into the placed list, entry will later be removed, this is to stop the program from editing every second row.
            if (roomAdded)
            {
                for (int x = startx; x < startx + roomWidth; x++)
                {
                    for (int y = starty; y < starty + roomHeight; y++)
                    {
                        placedTiles.Add(genMap.getTileAt(new Point(x, y)));
                        //Debug.Log("Placing tile for room " + startx + ":" + starty + "|" + roomWidth + ":" + roomHeight + " Tile-" + x + ":" + y);
                    }
                }
            }

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

            Debug.Log("Room Doors: "+room.ToString());
            List<int> direction = new List<int>();
            direction.Add(0);
            direction.Add(1);
            direction.Add(2);
            direction.Add(3);

            //setup which directions we can choose from.
            if (room.starty + room.height >= genMap.Height - 1)
            {
                direction.Remove(0);
            }
            if(room.startx+room.width >= genMap.Width-1)
            {
                direction.Remove(1);
            }
            if (room.starty <= 1)
            {
                direction.Remove(2);
            }
            if (room.startx <= 1)
            {
                direction.Remove(3);
            }

            for (int i = room.connections; i < max_connections; i++)
            {
                bool valid;
                Tile t;
                int inf = 0;
                // find an entry out of the room that will reach a corridor
                do {
                    valid = false;

                    int pointDirection = direction[rand.range(0, direction.Count)];
                    Point room1P = room.getDirPoint(pointDirection, rand);

                    t = genMap.getTileAt(room1P);
                    List<Tile> neighbours = genMap.getNeighbours(t);

                    foreach (Tile neigh in neighbours)
                    {
                        // Look for a corridor if we dont find one then the point isn't good.
                        if (neigh.Type == TILE_TYPE.Corridor)
                        {
                            valid = true;
                            break;
                        }
                    }
                    inf++;
                    if (inf > 100)
                    {
                        Debug.Log(inf+" attempts and failed to find an valid entry.. will need something else.");
                        break;
                    } 
                } while (!valid ) ;

                t.Type = TILE_TYPE.Corridor;
                placedTiles.Remove(t);
                //Debug.Log(room.ToString()+ " Connections "+ max_connections + " Added entry: " + t.ToString());

                // we dont add to potential because then we dont start multiple paths that wont connect to eachother
                //potentialTiles.Add(t);

                room.connections++;
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

    // does this really need to be here?..
    private void placeOutsideWalls()
    {
        //Debug.Log("Filling a map with W" + width+"H"+height);
        for (int x = 0; x < genMap.Width; x++)
        {
            for (int y = 0; y < genMap.Height; y++)
            {
                if (x == 0 || y == 0 || x == genMap.Width - 1 || y == genMap.Height - 1)
                {
                    placedTiles.Add(genMap.getTileAt(x, y));
                }
            }
        }
    }


    // Generates a maze from wall tiles in a map.
    // old generator, can't handle starts inside rooms
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
            mx = rand.range(1, genMap.Width - 1);
            my = rand.range(1, genMap.Height - 1);
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
                if (nextTile.Type == TILE_TYPE.Wall)
                {
                    nextTile.Type = TILE_TYPE.Corridor;
                }
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
            }
            else
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
}



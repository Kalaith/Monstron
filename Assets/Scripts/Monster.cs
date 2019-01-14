using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MONSTERS { GOBLIN, HOBGOBLIN, ORC, ORGE, GREMLIN};
public enum MONSTER_EGGS { GOBLIN_EGG = 40, HOBGOBLIN_EGG = 10, ORC_EGG = 20, ORGE_EGG = 5, GREMLIN_EGG = 30 };

public class Monster : Character {

    Path_AStar pathAStar;
    Tile currTile;
    Tile nextTile;
    Tile destTile;

    public double eggSpawnChance; // chance for an egg to spawn of this monster

    public Monster(int x, int y, string name, int health, CHARACTER_TYPE type, Vector3Int stats, double egg) : base(x, y, name, health, type)
    {
        this.stats = stats;
        eggSpawnChance = egg;
    }
    public Monster(Point p, string name, int health, CHARACTER_TYPE type, Vector3Int stats, double egg) : base(p.x, p.y, name, health, type)
    {
        this.stats = stats;
        eggSpawnChance = egg;
    }

    // For now just move randomly, later we will want to add features such as target player and movemeent types like patrol
    public void moveMonster(Map map, Player player)
    {
        Tile mTile = map.getTileAt(x, y);
        Tile pTile = map.getTileAt(player.x, player.y);
        int distanceToPlayer = mTile.distanceToTile(new Point(pTile.X, pTile.Y));
        // did we attack, if we did we dont also get to move.
        bool attacked = false;
        foreach (Ability a in abilities)
        {
            if(!a.passive && a.range >= distanceToPlayer && mTile.tileDiagonal(new Point(pTile.X, pTile.Y)))
            {
                player.current_health = player.current_health - a.damage;
                attacked = true;
            }
        }
        
        //Debug.Log("Monster " + this.ToString() + " distance to player "+ mTile.distanceToTile(new Point(pTile.X, pTile.Y)));
        if (!attacked)
        {
            if (distanceToPlayer < 10)
            {

                //Debug.Log("Moving towards player");
                //Debug.Log("Is player the same as destination" + (pTile != destTile));
                //Debug.Log("pTile" + (pTile.ToString()));
                //Debug.Log("destTile" + (pTile.ToString()));
                // Player is close lets move in their direction
                // we actually want to move next to the player not ontop off so will need to update this later.
                if (pTile != destTile)
                {
                    pathAStar = null;

                    destTile = pTile;
                    pathAStar = new Path_AStar(map, mTile, destTile);
                    //Debug.Log(pathAStar.ToString());
                    //Debug.Log("Found Player, moving towards distance" + pathAStar.Length());
                }

                // Do we have a path to travel to
                if (pathAStar != null && pathAStar.Length() != 0)
                {

                    nextTile = pathAStar.Dequeue();
                    //Debug.Log("Moving to " + nextTile.ToString());
                    x = nextTile.X;
                    y = nextTile.Y;
                }


            }
            else
            {
                // Do the below where its just a rando direction
                // allow each monster to move as many times as it can for each time its move function is called
                // this doesn't really allow good merging of activities but it will do for now.
                for (int i = 0; i < speed; i++)
                {
                    int move = 0;
                    int moveX = 0;
                    int moveY = 0;
                    // move to a random position, if its a wall we can't move.
                    move = UnityEngine.Random.Range(0, 3) - 1;

                    int direction = UnityEngine.Random.Range(0, 2);

                    if (direction == 0)
                    {
                        moveX = move;
                    }
                    if (direction == 1)
                    {
                        moveY = move;
                    }
                    // we dont want to move diagonal unless we let the player do it as well.
                    //Debug.Log("Monster Movement moveX"+moveX+" moveY"+moveY);
                    if (map.getTileTypeAt(moveX + x, moveY + y) != TILE_TYPE.Wall && map.getTileTypeAt(moveX + x, moveY + y) != TILE_TYPE.Empty)
                    {
                        x += moveX;
                        y += moveY;
                    }

                    if (moveX == -1)
                    {
                        facing = CHARACTER_FACING.LEFT;
                    }
                    if (moveX == 1)
                    {
                        facing = CHARACTER_FACING.RIGHT;
                    }
                    if (moveY == 1)
                    {
                        facing = CHARACTER_FACING.UP;
                    }
                    if (moveY == -1)
                    {
                        facing = CHARACTER_FACING.DOWN;
                    }
                }
            }
        }
    }
}

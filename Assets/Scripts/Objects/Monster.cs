using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MONSTERS { GOBLIN, HOBGOBLIN, ORC, ORGE, GREMLIN };
public enum MONSTER_EGGS { GOBLIN_EGG = 40, HOBGOBLIN_EGG = 10, ORC_EGG = 20, ORGE_EGG = 5, GREMLIN_EGG = 30 };

[Serializable]
public class Monster : Character
{

    static int nrOfInstances = 1; // start at 1 so we can use 0 for no monster

    public int monsterID;

    Path_AStar pathAStar;
    public Tile currTile;
    public Tile nextTile;
    public Tile destTile;

    public double eggSpawnChance; // chance for an egg to spawn of this monster
    internal bool ready;

    public Monster(int x, int y, string name, int health, CHARACTER_TYPE type, Vector3Int stats, double egg) : base(x, y, name, health, type)
    {
        this.stats = stats;
        eggSpawnChance = egg;
        monsterID = Monster.nrOfInstances;
        Monster.nrOfInstances++;

    }
    public Monster(Point p, string name, int health, CHARACTER_TYPE type, Vector3Int stats, double egg) : base(p.x, p.y, name, health, type)
    {
        this.stats = stats;
        eggSpawnChance = egg;
        monsterID = Monster.nrOfInstances;
        Monster.nrOfInstances++;

    }

    internal void Tick(float deltaTime)
    {
        // increment counter since last action.. if time is up set ready to true.
        ready = false;
    }

    public override string ToString()
    {
        return name + ": Health: " + current_health + " Stats:" + stats.ToString() + " Type:" + type.ToString();
    }

    // For now just move randomly, later we will want to add features such as target player and movemeent types like patrol
    public void moveMonster(Map map, Player player)
    {
        Debug.Log("Moving Monster -");
        Tile currentTile = map.getTileAt(x, y);
        Tile playerTile = map.getTileAt(player.x, player.y);
        int distanceToPlayer = currentTile.distanceToTile(new Point(playerTile.X, playerTile.Y));

        if (TryAttackPlayer(player, currentTile, playerTile, distanceToPlayer))
            return;

        MoveTowardsTarget(map, currentTile, playerTile, distanceToPlayer);
    }

    private bool TryAttackPlayer(Player player, Tile currentTile, Tile playerTile, int distance)
    {
        foreach (Ability ability in abilities)
        {
            if (!ability.passive && ability.range >= distance && currentTile.tileDiagonal(new Point(playerTile.X, playerTile.Y)))
            {
                player.current_health -= ability.damage;
                return true;
            }
        }
        return false;
    }

    private void MoveTowardsTarget(Map map, Tile currentTile, Tile targetTile, int distance)
    {
        if (distance < 10)
        {
            HandlePathfinding(map, currentTile, targetTile);
        }
        else
        {
            MoveRandomly(map);
        }
    }

    private void HandlePathfinding(Map map, Tile currentTile, Tile targetTile)
    {
        if (targetTile != destTile)
        {
            InitializeNewPath(map, currentTile, targetTile);
        }

        if (pathAStar != null && pathAStar.Length() != 0)
        {
            MoveAlongPath();
        }
    }

    private void InitializeNewPath(Map map, Tile currentTile, Tile targetTile)
    {
        pathAStar = null;
        destTile = targetTile;
        pathAStar = new Path_AStar(map, currentTile, destTile);
    }

    private void MoveAlongPath()
    {
        nextTile = pathAStar.Dequeue();
        x = nextTile.X;
        y = nextTile.Y;
    }

    private void MoveRandomly(Map map)
    {
        for (int i = 0; i < speed; i++)
        {
            Vector2Int movement = GetRandomMovement();
            if (IsValidMove(map, movement))
            {
                ApplyMovement(movement);
                UpdateFacingDirection(movement);
            }
        }
    }

    private Vector2Int GetRandomMovement()
    {
        int move = UnityEngine.Random.Range(0, 3) - 1;
        int direction = UnityEngine.Random.Range(0, 2);
        return direction == 0 ? new Vector2Int(move, 0) : new Vector2Int(0, move);
    }

    private bool IsValidMove(Map map, Vector2Int movement)
    {
        TILE_TYPE tileType = map.getTileTypeAt(movement.x + x, movement.y + y);
        return tileType != TILE_TYPE.Wall && tileType != TILE_TYPE.Empty;
    }

    private void ApplyMovement(Vector2Int movement)
    {
        x += movement.x;
        y += movement.y;
    }

    private void UpdateFacingDirection(Vector2Int movement)
    {
        if (movement.x < 0) facing = CHARACTER_FACING.LEFT;
        else if (movement.x > 0) facing = CHARACTER_FACING.RIGHT;
        else if (movement.y > 0) facing = CHARACTER_FACING.UP;
        else if (movement.y < 0) facing = CHARACTER_FACING.DOWN;
    }

}

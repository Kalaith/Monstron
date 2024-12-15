using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// Should this be more like a turn manager because a turn starts when a player successfully moves.
public class InputController : MonoBehaviour
{
    public int turns = 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.game == null || !GameController.game.gameReady || MapController.mapC.map == null)
            return;

        if (PlayerController.playerC.player.current_health <= 0 && GameController.game.level > 0)
        {
            GameController.game.leaveDungeon();
            SceneManager.LoadScene("Town");
            return;
        }

        // Wait, let enemy move
        if (Input.GetKeyUp("q"))
        {
            EnemyController.enemyC.updateMonsters();

        }

        if (Input.GetKeyUp("t"))
        {
            if (!PlayerController.playerC.player.usingAbility)
            {
                Ability t = PlayerController.playerC.player.getAbility("Teleport");
                if (t.current_uses > 0)
                {
                    PlayerController.playerC.player.useAbility(t);
                    Debug.Log("Using teleport");
                }
                else
                {
                    Debug.Log("Unable to use ability, no uses left.");
                }
            }
        }

        if (Input.GetKeyUp("e"))
        {
            if (!PlayerController.playerC.player.usingAbility)
            {
                PlayerController.playerC.player.useAbility(PlayerController.playerC.player.getAbility("Attack"));
            }
        }

        // these are functions to help test the game and should be removed later
        if (Input.GetKeyUp("x"))
        {
            // We have reached an teleport.
            Debug.Log("Choosing to exit");
            GameController.game.leaveDungeon();
        }
        // Goto the next level
        if (Input.GetKeyUp("z"))
        {
            GameController.game.unloadMap();
            GameController.game.loadMap();
        }

        HandleMovementInput();

    }
    private void HandleMovementInput()
    {
        if (Input.GetKeyUp("w")) TryMovePlayer(0, 1);
        if (Input.GetKeyUp("s")) TryMovePlayer(0, -1);
        if (Input.GetKeyUp("d")) TryMovePlayer(1, 0);
        if (Input.GetKeyUp("a")) TryMovePlayer(-1, 0);
    }


    private void TryMovePlayer(int x, int y)
    {
        if (MapController.mapC.map.isPassable(PlayerController.playerC.player.x + x, PlayerController.playerC.player.y + y))
        {
            movePlayer(x, y);
        }
    }

    public void movePlayer(int x = 0, int y = 0)
    {
        Debug.Log("Moving Player");
        PlayerController.playerC.player.x += x;
        PlayerController.playerC.player.y += y;

        CheckForItem();
        CheckForSpecialTiles();

        if (x != 0 || y != 0)
        {
            Debug.Log("Turns:" + turns + " Player Speed:" + PlayerController.playerC.player.speed);
            turns++;
            if (turns == PlayerController.playerC.player.speed)
            {
                Debug.Log("Monster Updates");
                EnemyController.enemyC.updateMonsters();
                turns = 0;
            }
        }
    }
    
    private void CheckForItem()
    {
        Item item = MapController.mapC.map.getItemAt(PlayerController.playerC.player.x, PlayerController.playerC.player.y);
        if (item != null && item.itemType == ITEM_TYPE.EGG)
        {
            Debug.Log("Standing on an egg, picked it up");
            PlayerController.playerC.player.pickupItem(item);
            MapController.mapC.despawnItems();
        }
    }

    private void CheckForSpecialTiles()
    {
        var playerPos = new Vector2Int(PlayerController.playerC.player.x, PlayerController.playerC.player.y);
        var map = MapController.mapC.map;

        if (map.getTileAt(playerPos.x, playerPos.y) == map.Teleport)
        {
            Debug.Log("Standing on the teleport");
            GameController.game.unloadMap();
            GameController.game.loadMap();
        }
        else if (map.getTileAt(playerPos.x, playerPos.y) == map.Exit)
        {
            Debug.Log("Standing on the exit");
            GameController.game.leaveDungeon();
        }
    }
}



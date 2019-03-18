using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// Should this be more like a turn manager because a turn starts when a player successfully moves.
public class InputController : MonoBehaviour {


	// Use this for initialization
	void Start () {
    }

    // Update is called once per frame
    void Update () {
        if (GameController.game != null && GameController.game.gameReady && MapController.mapC.map != null)
        {

            if (PlayerController.playerC.player.current_health <= 0 && GameController.game.level > 0)
            {
                GameController.game.leaveDungeon();
                SceneManager.LoadScene("Town");

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
                    } else
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
            
            // leaves the dungeon
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


            if (Input.GetKeyUp("w"))
            {
                // Is it terrain we can enter, maybe rename from Ground to more like Enterable
                if (MapController.mapC.map.isPassable(PlayerController.playerC.player.x, PlayerController.playerC.player.y + 1))
                {
                    movePlayer(0, 1);
                }

            }
            if (Input.GetKeyUp("s"))
            {
                if (MapController.mapC.map.isPassable(PlayerController.playerC.player.x, PlayerController.playerC.player.y - 1))
                {
                    //Debug.Log("Moving Down " + MapController.mapC.map.getTileTypeAt(PlayerController.playerC.player.x, PlayerController.playerC.player.y - 1).ToString());
                    movePlayer(0, -1);
                }
            }
            if (Input.GetKeyUp("d"))
            {
                if (MapController.mapC.map.isPassable(PlayerController.playerC.player.x + 1, PlayerController.playerC.player.y))
                {
                    movePlayer(1, 0);
                }
            }
            if (Input.GetKeyUp("a"))
            {
                if (MapController.mapC.map.isPassable(PlayerController.playerC.player.x - 1, PlayerController.playerC.player.y))
                {
                    movePlayer(-1, 0);
                }
            }
            
        }
    }

    int turns = 0;

    public void movePlayer(int x = 0, int y = 0)
    {
        PlayerController.playerC.player.x += x;
        PlayerController.playerC.player.y += y;

        Item item = MapController.mapC.map.getItemAt(PlayerController.playerC.player.x, PlayerController.playerC.player.y);
        if (item != null && item.itemType == ITEM_TYPE.EGG)
        {
            // We have reached an teleport.
            Debug.Log("Standing on an egg, picked it up");
            PlayerController.playerC.player.pickupItem(item);
            MapController.mapC.despawnItems(); // hacky need both bits of data togheter to remove just one..
        }
        if (MapController.mapC.map.getTileAt(PlayerController.playerC.player.x, PlayerController.playerC.player.y) == MapController.mapC.map.Teleport)
        {
            // We have reached an teleport.
            Debug.Log("Standing on the teleport");
            GameController.game.unloadMap();
            GameController.game.loadMap();
        }
        if (MapController.mapC.map.getTileAt(PlayerController.playerC.player.x, PlayerController.playerC.player.y) == MapController.mapC.map.Exit)
        {
            // We have reached an teleport.
            Debug.Log("Standing on the exit");
            GameController.game.leaveDungeon();
        }
        // Debug.Log("X" + x + "Y" + y);
        // if its not an empty move then attempt to move each monster
        if (x != 0 || y != 0)
        {
            turns++;
            if (turns == PlayerController.playerC.player.speed)
            {
                EnemyController.enemyC.updateMonsters();
                turns = 0;
            }
        }
    }
}



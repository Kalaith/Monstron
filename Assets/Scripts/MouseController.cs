using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    GameObject selectGO;
    public Sprite select;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            if (PlayerController.playerC.player.usingAbility)
            {
                Vector3 mouse = Input.mousePosition;

                if (PlayerController.playerC.player.activeAbility.name == "Teleport")
                {

                    mouse = Camera.main.ScreenToWorldPoint(mouse);
                    mouse.x += 0.5f;
                    mouse.y += 0.5f;

                    //Debug.Log("Mouse Position " + mouse.ToString());
                    if (MapController.mapC.map != null)
                    {
                        if (MapController.mapC.map.isPassable((int)mouse.x, (int)mouse.y))
                        {
                            // get the tile and see if the distance to the player is less then the range of the active ability, if it is then we can show the select box.
                            Tile t = MapController.mapC.map.getTileAt(new Point((int)mouse.x, (int)mouse.y));
                            //Debug.Log("Range Check"+ t.distanceToTile(new Point(PlayerController.playerC.player.x, PlayerController.playerC.player.y)));
                            if (PlayerController.playerC.player.activeAbility.range >= t.distanceToTile(new Point(PlayerController.playerC.player.x, PlayerController.playerC.player.y)))
                            {
                                if (selectGO == null)
                                {
                                    selectGO = new GameObject();
                                    selectGO.transform.SetParent(this.transform, true);
                                    selectGO.AddComponent<SpriteRenderer>();
                                    selectGO.GetComponent<SpriteRenderer>().sortingOrder = 5;
                                    selectGO.GetComponent<SpriteRenderer>().sprite = select;
                                }
                                if (selectGO.transform.position == new Vector3((int)mouse.x, (int)mouse.y, 0))
                                {
                                    // we have clicked twice so we want to teleport.
                                    Destroy(selectGO);
                                    PlayerController.playerC.player.x = (int)mouse.x;
                                    PlayerController.playerC.player.y = (int)mouse.y;
                                    PlayerController.playerC.player.usingAbility = false;

                                    //Debug.Log("Monsters "+EnemyController.enemyC.monsters.Count);
                                    foreach (Monster monster in EnemyController.enemyC.monsters.Keys)
                                    {
                                        monster.moveMonster(MapController.mapC.map, PlayerController.playerC.player);
                                    }

                                }
                                else
                                {
                                    selectGO.name = "select_X" + (int)mouse.x + "Y" + (int)mouse.y;
                                    selectGO.transform.position = new Vector3((int)mouse.x, (int)mouse.y, 0);

                                }

                            }
                        }
                    }
                }
                if (PlayerController.playerC.player.activeAbility.name == "Attack")
                {


                    mouse = Camera.main.ScreenToWorldPoint(mouse);
                    mouse.x += 0.5f;
                    mouse.y += 0.5f;

                    //Debug.Log("Mouse Position " + mouse.ToString());
                    Monster m = EnemyController.enemyC.getMonsterAt((int)mouse.x, (int)mouse.y);
                    if (m != null)
                    {
                        // get the tile and see if the distance to the player is less then the range of the active ability, if it is then we can show the select box.
                        Tile t = MapController.mapC.map.getTileAt(new Point((int)mouse.x, (int)mouse.y));
                        //Debug.Log("Range Check"+ t.distanceToTile(new Point(PlayerController.playerC.player.x, PlayerController.playerC.player.y)));
                        if (PlayerController.playerC.player.activeAbility.range >= t.distanceToTile(new Point(PlayerController.playerC.player.x, PlayerController.playerC.player.y)))
                        {
                            if (selectGO == null)
                            {
                                selectGO = new GameObject();
                                selectGO.transform.SetParent(this.transform, true);
                                selectGO.AddComponent<SpriteRenderer>();
                                selectGO.GetComponent<SpriteRenderer>().sortingOrder = 5;
                                selectGO.GetComponent<SpriteRenderer>().sprite = select;
                            }
                            if (selectGO.transform.position == new Vector3((int)mouse.x, (int)mouse.y, 0))
                            {
                                // we have clicked twice so we want to teleport.
                                Destroy(selectGO);
                                m.takeDamage(PlayerController.playerC.player.activeAbility.damage);
                                PlayerController.playerC.player.usingAbility = false;

                                EnemyController.enemyC.updateMonsters();
                            }
                            else
                            {
                                selectGO.name = "select_X" + (int)mouse.x + "Y" + (int)mouse.y;
                                selectGO.transform.position = new Vector3((int)mouse.x, (int)mouse.y, 0);

                            }

                        }
                    }
                }
            }
        }
    }
}

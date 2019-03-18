using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public static GameController game;
    public int level;
    public GameObject GameUI;
    public int gameSeed = 0;

    private void Awake()
    {
        if (game == null)
        {
            DontDestroyOnLoad(gameObject);
            random = new RandomGen(gameSeed);
            game = this;
        }
        else if (game != this)
        {
            Destroy(gameObject);
        }
    }

    public RandomGen random;

    public bool gameReady;

    public List<Ability> availableAbilities;

    // Use this for initialization
    void Start () {
        availableAbilities = new List<Ability>();

        PlayerController.playerC.createPlayer();

        assignPlayer();

        if (SceneManager.GetActiveScene().name == "Dungeon")
        {
            loadMap();
        }
    }

    public void assignPlayer()
    {
        // when we first start given the player a monster and assign all the defaults or when the previous one was killed.
        if (PlayerController.playerC.corral.monsters.Count == 0)
        {
            Debug.Log("Creating Initial Monster or Player has no monsters");

            // Create our inital monster
            Monster mon = new Monster(new Point(0, 0), "SLIME", 10, CHARACTER_TYPE.MONSTER, new Vector3Int(255, 255, 255), 0);

            mon.setNextLevel();

            // Create inital ability and assign to both player (selected monster and the first monster)
            Ability teleport = new Ability("Teleport", false, false, 0, 2, true, 0, 0, 0, 0, CHARACTER_TYPE.ALL, 5);
            mon.addAbility(teleport);
            
            SpriteGenerator sg = ScriptableObject.CreateInstance<SpriteGenerator>();
            mon.texture = sg.tex2dtobytes(PlayerController.playerC.playerSprite.texture);
            PlayerController.playerC.player.texture = mon.texture;

            // Assign it to the players Corral
            PlayerController.playerC.corral.addMonster(mon);
            PlayerController.playerC.player.assignMonsterToPlayer(mon);
        } else
        {
            // reassign the monster to the player.. this looks like it could be shorter.
             PlayerController.playerC.player.assignMonsterToPlayer(PlayerController.playerC.corral.getMonsterByID(PlayerController.playerC.player.controllingMonster));
        }
    }

    public void leaveDungeon()
    {
        unloadMap();
        Player player = PlayerController.playerC.player;

        // if the monster manages to exit the dungeon give them 5 exp per floor.
        if (player.current_health > 0)
        {
            Debug.Log("Left the dungeon, exp: "+(5*level)); // TODO remove constant values
            player.current_exp += (5*level);
            player.current_health = player.max_health;
            level = 0;
            PlayerController.playerC.corral.updateMonster(player);

            player.usingAbility = false;

            player.resetAbilities();

            // Reset ability uses, for now just do teleport.
            Ability a = player.getAbility("Teleport");
            if (a != null)
            {
                a.current_uses = a.max_uses;
            }

        }
        else
        {
            // if the monster died remove it from the corral.
            Debug.Log("Player died, killed by monster");
            level = 0;
            PlayerController.playerC.removeMonster();
            // dont get to keep items when you die.
            PlayerController.playerC.player.emptyItems();
        }

        Debug.Log("Moving to Town scene");

        SceneManager.LoadScene("Town");
    }

    public void loadMap()
    {

        level++;

        if (MapController.mapC != null)
        {
            Debug.Log("Map Available");
            MapController.mapC.spawnMap(random);
            if (MapController.mapC.spawnMonsters)
            {
                Debug.Log("Spawning Monsters");
                EnemyController.enemyC.spawnMonsters(level);

                Ability monsterAttack = new Ability("Attack", false, false, 1, 1, false, 0, 0, 0, 0, CHARACTER_TYPE.MONSTER, 0);

                availableAbilities.Add(monsterAttack);

                foreach (Monster m in EnemyController.enemyC.monsters.Keys)
                {
                    EnemyController.enemyC.assignAbility(m, monsterAttack);
                }

                MapController.mapC.spawnItems();
            }

            Debug.Log("Spawning Player");
            PlayerController.playerC.spawnPlayer(MapController.mapC.map.startPosition);

            gameReady = true;

            if (SceneManager.GetActiveScene().name == "Dungeon")
            {
                Text t = GameObject.Find("CurrentLevel").GetComponent<Text>();
                t.text = "Level: " + level;
            } else
            {
                Debug.Log("Loading Map, Not a Dungeon, Scene " + SceneManager.GetActiveScene().name);
            }

        }
    }

    public void unloadMap()
    {
        Debug.Log("Unload map was called and player health was " + PlayerController.playerC.player.current_health);
        PlayerController.playerC.despawnPlayer();
        if (MapController.mapC.spawnMonsters)
            EnemyController.enemyC.despawnMonsters();
        MapController.mapC.despawnMap();
        // if the player has died we need to leave the dungeon.

    }

    // Update is called once per frame
    void Update () {
        
    }
}

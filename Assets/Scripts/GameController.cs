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

    private void Awake()
    {
        if (game == null)
        {
            DontDestroyOnLoad(gameObject);
            random = new RandomGen(0);
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

        // when we first start given the player a monster and assign all the defaults.
        if (PlayerController.playerC.corral.corralMonsters() == 0)
        {
            Debug.Log("Creating Initial Monster");
            // Create inital ability and assign to both player (selected monster and the first monster)
            Ability teleport = new Ability("Teleport", false, false, 0, 2, true, 0, 0, 0, 0, CHARACTER_TYPE.ALL);

            // Create our inital monster
            Monster m = new Monster(new Point(0, 0), "SLIME", 10, CHARACTER_TYPE.MONSTER, new Vector3Int(255, 255, 255), 0);

            m.setNextLevel();

            m.addAbility(teleport);
            SpriteGenerator sg = ScriptableObject.CreateInstance<SpriteGenerator>();
            m.texture = sg.tex2dtobytes(PlayerController.playerC.playerSprite.texture);

            PlayerController.playerC.player.texture = m.texture;

            // Assign it to the players Corral
            PlayerController.playerC.corral.addMonster(m);

            // Add the ability to the player since the monster is the only one that can be selected.
            PlayerController.playerC.player.addAbility(teleport);
            availableAbilities.Add(teleport);
        }

        if(SceneManager.GetActiveScene().name == "Dungeon")
        {
            loadMap();
        }

    }

    public void leaveDungeon()
    {
        unloadMap();

        // if the monster manages to exit the dungeon give them 5 exp per floor.
        if (PlayerController.playerC.player.current_health > 0)
        {
            Debug.Log("Left the dungeon, exp: "+(5*level));
            PlayerController.playerC.player.current_exp += (5*level);
            PlayerController.playerC.corral.updateMonster(PlayerController.playerC.player);
            PlayerController.playerC.player.current_health = PlayerController.playerC.player.max_health;
        }
        else
        {
            // if the monster died remove it from the corral.
            PlayerController.playerC.corral.removeMonster(PlayerController.playerC.corral.getMonsterByID(PlayerController.playerC.player.controllingMonster));
            PlayerController.playerC.player.controllingMonster = 0;
        }

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

                Ability monsterAttack = new Ability("Attack", false, false, 1, 1, false, 0, 0, 0, 0, CHARACTER_TYPE.MONSTER);

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
        
        
        PlayerController.playerC.despawnPlayer();
        if (MapController.mapC.spawnMonsters)
            EnemyController.enemyC.despawnMonsters();
        MapController.mapC.despawnMap();
    }

    // Update is called once per frame
    void Update () {
        
    }
}

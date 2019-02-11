﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// Should this be encounters instead?
public class EnemyController : MonoBehaviour {

    public static EnemyController enemyC;

    private void Awake()
    {
        if (enemyC == null)
        {
            DontDestroyOnLoad(gameObject);
            enemyC = this;
        }
        else if (enemyC != this)
        {
            Destroy(gameObject);
        }
    }

    public Dictionary<Monster, GameObject> monsters;
    List<GameObject> goMonsters;
    public List<Sprite> monsterSprites;
    public Dictionary<string, int> monsterDetails; 
    GameController gc;
    SpriteGenerator sg;

    // Use this for initialization
    void Start () {
        // get a copy of the map controller so we know where we are placing enemies on the map
        gc = (GameController)FindObjectOfType(typeof(GameController));

        monsters = new Dictionary<Monster, GameObject>();
        sg = ScriptableObject.CreateInstance<SpriteGenerator>();
        
    }

    public void spawnMonsters(int current_level)
    {
        foreach (Room room in MapController.mapC.map.rooms)
        {
            Point p = room.getCenter();
            //Debug.Log("Spawn a monster at " + p.ToString());
            addMonster(p, current_level);
        }

        
    }

    public Monster getMonsterAt(int x, int y)
    {
        foreach(Monster monster in monsters.Keys)
        {
            if(monster.x ==x && monster.y==y)
            {
                return monster;
            }
        }
        return null;
    }

    public void updateMonsters()
    {
        foreach (Monster monster in monsters.Keys)
        {
            monster.moveMonster(MapController.mapC.map, PlayerController.playerC.player);
        }

    }

    public void addMonster(Monster monster)
    {
        int monsterID = GameController.game.random.range(1, monsterSprites.Count + 1);

        GameObject monsterGO = new GameObject();

        monsterGO.name = monster.name;
        monsterGO.AddComponent<SpriteRenderer>();
        monsterGO.transform.position = new Vector3(monster.x, monster.y, 0);
        monsterGO.transform.SetParent(this.transform, true);


        monsterGO.GetComponent<SpriteRenderer>().sprite = sg.addXMirror(monsterSprites[monsterID]);
        monsterGO.GetComponent<SpriteRenderer>().sortingOrder = 5;
        monsterGO.GetComponent<SpriteRenderer>().color = new Color32((byte)monster.stats.x, (byte)monster.stats.y, (byte)monster.stats.z, 255); // monster.stats.x/255, monster.stats.y/255, monster.stats.z/255);

        monster.texture = sg.tex2dtobytes(monsterSprites[monsterID].texture);

        monsters.Add(monster, monsterGO);
    }

    internal void assignAbility(Monster m, Ability monsterAttack)
    {
        m.abilities.Add(monsterAttack);
    }

    public void addMonster(Point p, int level)
    {
        int monsterID = GameController.game.random.range(1, monsterSprites.Count + 1);
        string name = "";
        int eggSpawnChance = 0;
        int health = 3;
        Vector3Int stats = new Vector3Int(255, 0, 0);
        Debug.Log("Monster ID:" + monsterID);
        switch (monsterID)
        {
            case 1:
                name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(MONSTERS.GOBLIN.ToString());
                eggSpawnChance = (int)MONSTER_EGGS.GOBLIN_EGG;
                stats = new Vector3Int(255, 0, 0);
                health = 3;
                break;
            case 2:
                name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(MONSTERS.GREMLIN.ToString());
                eggSpawnChance = (int)MONSTER_EGGS.GREMLIN_EGG;
                stats = new Vector3Int(0, 255, 0);
                health = 2;
                break;
            case 3:
                name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(MONSTERS.HOBGOBLIN.ToString());
                eggSpawnChance = (int)MONSTER_EGGS.HOBGOBLIN_EGG;
                stats = new Vector3Int(0, 0, 255);
                health = 4;
                break;
            case 4:
                name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(MONSTERS.ORC.ToString());
                eggSpawnChance = (int)MONSTER_EGGS.ORC_EGG;
                stats = new Vector3Int(255, 0, 255);
                health = 7;
                break;
            case 5:
                name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(MONSTERS.ORGE.ToString());
                eggSpawnChance = (int)MONSTER_EGGS.ORGE_EGG;
                stats = new Vector3Int(0, 255, 255);
                health = 10;
                break;
            default:
                name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(MONSTERS.ORGE.ToString());
                eggSpawnChance = (int)MONSTER_EGGS.ORGE_EGG;
                stats = new Vector3Int(0, 255, 255);
                health = 10;
                break;
        }


        Monster monster = new Monster(p, name, health, CHARACTER_TYPE.MONSTER, stats, eggSpawnChance);

        // Setup monster details, should these be done inside the monster.
        monster.current_exp = 0;
        monster.level = GameController.game.random.range(level-1, level+1);
        monster.next_exp = monster.level * health;
        monster.max_health = GameController.game.random.range(health, health+level);
        monster.current_health = monster.max_health;

        GameObject monsterGO = new GameObject();

        monsterGO.name = monster.name;
        monsterGO.AddComponent<SpriteRenderer>();
        monsterGO.transform.position = new Vector3(monster.x, monster.y, 0);
        monsterGO.transform.SetParent(this.transform, true);

        int sprite = GameController.game.random.range(0, monsterSprites.Count);
        monsterGO.GetComponent<SpriteRenderer>().sprite = sg.addXMirror(monsterSprites[sprite]);
        monsterGO.GetComponent<SpriteRenderer>().sortingOrder = 5;
        monsterGO.GetComponent<SpriteRenderer>().color = new Color32((byte)monster.stats.x, (byte)monster.stats.y, (byte)monster.stats.z, 255); // monster.stats.x/255, monster.stats.y/255, monster.stats.z/255);

        monster.texture = sg.tex2dtobytes(monsterSprites[sprite].texture);

        // Has a chance of spawning 1 egg per level
        if (MapController.mapC.map.items.Count == 0)
        {
            int range = GameController.game.random.range(1, 100);
            if(range < monster.eggSpawnChance)
            MapController.mapC.map.items.Add(new Egg(monster.name+" Egg", 1.0, monster.x, monster.y, monster, 1, stats, ITEM_TYPE.EGG));
            Debug.Log("Item Added");
        }

        monsters.Add(monster, monsterGO);
    }
    List<Monster> removals = new List<Monster>();

    // Update is called once per frame
    void Update () {
        if (monsters != null)
        {
            
            foreach (KeyValuePair<Monster, GameObject> mon in monsters)
            {
                if (mon.Key.current_health <= 0)
                {
                    removals.Add(mon.Key);
                }
                else
                {
                    float rotation = 0;
                    if (mon.Key.facing == CHARACTER_FACING.UP)
                    {
                        rotation = 180;
                    }
                    if (mon.Key.facing == CHARACTER_FACING.LEFT)
                    {
                        rotation = -90;
                    }
                    if (mon.Key.facing == CHARACTER_FACING.DOWN)
                    {
                        rotation = 0;
                    }
                    if (mon.Key.facing == CHARACTER_FACING.RIGHT)
                    {
                        rotation = 90;
                    }
                    //mon.Value.transform.rotation = Quaternion.Euler(0, 0, rotation);
                    mon.Value.transform.position = new Vector3(mon.Key.x, mon.Key.y, 0);
                }
            }

            foreach(Monster mon in removals)
            {
                despawnMonster(mon);
            }
            removals.Clear();
        }
	}

    void despawnMonster(Monster m)
    {
        // destroy the game object and then remove the monster
        Destroy(monsters[m]);
        monsters.Remove(m);

        Debug.Log("Monster died.");
    }
    public void despawnMonsters()
    {
        foreach(KeyValuePair<Monster, GameObject> monster in monsters)
        {
            // Destroy all the game objects
            Destroy(monster.Value);
        }

        monsters.Clear();
    }


    public void assignAbilities(Monster mon)
    {
        foreach(Ability a in gc.availableAbilities)
        {
            if(a.type == CHARACTER_TYPE.ALL || a.type == CHARACTER_TYPE.MONSTER)
            {
                mon.addAbility(a);
            } 
        }
    }
}

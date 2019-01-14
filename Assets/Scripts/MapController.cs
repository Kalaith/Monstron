using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

    public static MapController mapC;
    private void Awake()
    {
        if (mapC == null)
        {
            DontDestroyOnLoad(gameObject);
            mapC = this;
        }
        else if (mapC != this)
        {
            Destroy(gameObject);
        }
    }

    public Map map;
    GameObject[,] goMap;
    public Sprite ground;
    public Sprite wall;
    public Sprite corridor;
    public Sprite floor;
    public Sprite teleport;
    public Sprite exit;
    public Sprite egg;

    public int mapWidth;
    public int mapHeight;
    public int mapRooms;
    public int mapSeed;

    public bool mapReady = false;
    GameObject exitGO;
    GameObject teleportGO;

    List<GameObject> itemGOs;

    // Do we spawn monsters on the map
    public bool spawnMonsters = false;

    // Use this for initialization
    void Start () {

    }

    public void spawnMap(RandomGen random)
    {
        Debug.Log("Spawning Map");
        // Use the map generator to spawn a map
        MapGenerator mg = new MapGenerator(random, mapWidth, mapHeight, mapRooms);
        map = mg.createDungeonMap();
        //map = mg.createTownMap();
        spawnMonsters = true;

        // Spawn the gameobjects for the map
        goMap = new GameObject[map.Width, map.Height];
        instantiateMap();

        // add in a random value from 0 to max rooms placed by the map generator
        if (!object.ReferenceEquals(map.Exit, null))
        {
            if (GameController.game.level % 5 == 0)
            {
                spawnExit();
            } else
            {
                map.Exit = null;
            }
        }
        if (!object.ReferenceEquals(map.Teleport, null))
        {
            spawnTeleport();
        }

        map.initPathfinding();

        mapReady = true;
    }

    void instantiateMap()
    {
        GameObject go;
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                Tile t = map.GameMap[x, y];
                go = new GameObject();

                go.name = t.ToString();
                go.transform.position = new Vector3(x, y, 0);
                go.transform.SetParent(this.transform, true);
                go.AddComponent<SpriteRenderer>();

                if(t.Type == TILE_TYPE.Ground)
                {
                    go.GetComponent<SpriteRenderer>().sprite = ground;
                }
                if (t.Type == TILE_TYPE.Wall)
                {
                    go.GetComponent<SpriteRenderer>().sprite = wall;
                }
                if (t.Type == TILE_TYPE.Corridor)
                {
                    go.GetComponent<SpriteRenderer>().sprite = corridor;
                }
                if (t.Type == TILE_TYPE.Floor)
                {
                    go.GetComponent<SpriteRenderer>().sprite = floor;
                }

                goMap[x, y] = go;
            }
        }
    }

    public void despawnMap()
    {
        mapReady = false;
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                Destroy(goMap[x, y]);
            }
        }
        despawnExit();
        despawnTeleport();
        despawnItems();

        map = null;
    }

    void drawMap()
    {

    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public void spawnItems()
    {

        Debug.Log("Item Count"+map.items.Count);

        foreach(Item item in map.items)
        {
            Debug.Log("Spawning Item");
            GameObject itemGO = new GameObject
            {
                name = "item_"+item.name
            };

            itemGO.AddComponent<SpriteRenderer>();
            itemGO.transform.position = new Vector3(item.x, item.y, 0);
            itemGO.transform.SetParent(this.transform, true);

            itemGO.GetComponent<SpriteRenderer>().sprite = egg;
            itemGO.GetComponent<SpriteRenderer>().sortingOrder = 5;
            itemGO.GetComponent<SpriteRenderer>().color = new Color32((byte)item.stats.x, (byte)item.stats.y, (byte)item.stats.z, 255); // monster.stats.x/255, monster.stats.y/255, monster.stats.z/255);

            if (itemGOs == null)
                itemGOs = new List<GameObject>();

            itemGOs.Add(itemGO);
        }
    }

    public void despawnItems()
    {
        Debug.Log("Clearing Game Objects");

        // clear the base objects
        map.items.Clear();
        if (itemGOs != null && itemGOs.Count > 0)
        {
            // destroy then clear the game objects
            foreach (GameObject go in itemGOs)
            {
                Destroy(go);
            }
            itemGOs.Clear();
        }
    }

    // place an exit inside one of the rooms this takes you to the next level
    public void spawnExit()
    {
        //Debug.Log("Placing Exit");
        exitGO = new GameObject
        {
            name = "To Town"
        };
        exitGO.AddComponent<SpriteRenderer>();
        exitGO.transform.position = new Vector3(map.Exit.X, map.Exit.Y, 0);
        exitGO.transform.SetParent(this.transform, true);

        exitGO.GetComponent<SpriteRenderer>().sprite = exit;
        exitGO.GetComponent<SpriteRenderer>().sortingOrder = 5;

    }

    // place an exit inside one of rooms this takes you to the next level
    public void spawnTeleport()
    {
        //Debug.Log("Placing Exit");
        teleportGO = new GameObject
        {
            name = "Next Level"
        };
        teleportGO.AddComponent<SpriteRenderer>();
        teleportGO.transform.position = new Vector3(map.Teleport.X, map.Teleport.Y, 0);
        teleportGO.transform.SetParent(this.transform, true);

        teleportGO.GetComponent<SpriteRenderer>().sprite = teleport;
        teleportGO.GetComponent<SpriteRenderer>().sortingOrder = 5;

    }
    void despawnExit()
    {
        Destroy(exitGO);
        map.Exit = null;
    }
    void despawnTeleport()
    {
        Destroy(teleportGO);
        map.Teleport = null;
    }
}

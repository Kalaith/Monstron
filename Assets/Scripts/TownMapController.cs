using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownMapController : MonoBehaviour {

    public static TownMapController townC;
    private void Awake()
    {
        if (townC == null)
        {
            DontDestroyOnLoad(gameObject);
            townC = this;
        }
        else if (townC != this)
        {
            Destroy(gameObject);
        }
    }

    public Map map;
    GameObject[,] goMap;
    public Sprite ground;
    public Sprite wall;
    public Sprite floor;

    public int mapWidth;
    public int mapHeight;
    public int mapRooms;

    public bool mapReady = false;

    List<GameObject> itemGOs;

    // Use this for initialization
    void Start () {
        spawnMap();
    }

    public void spawnMap()
    {
        Debug.Log("Spawning Town Map");
        map = new Map(mapWidth, mapHeight);

        map.fillMap(TILE_TYPE.Ground);
        // Spawn the gameobjects for the map
        goMap = new GameObject[map.Width, map.Height];
        instantiateMap();

        map.initPathfinding();

        PlayerController.playerC.createPlayer();
        PlayerController.playerC.spawnPlayer(new Point(25, 25));

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

        Debug.Log("Depspawning Map");

        mapReady = false;
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                Destroy(goMap[x, y]);
            }
        }
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

            //itemGO.GetComponent<SpriteRenderer>().sprite;
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

}

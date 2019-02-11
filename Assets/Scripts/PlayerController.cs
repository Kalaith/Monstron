using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public static PlayerController playerC;

    private void Awake()
    {
        if (playerC == null)
        {
            DontDestroyOnLoad(gameObject);
            playerC = this;
            createPlayer();
        } else if(playerC != this)
        {
            Destroy(gameObject);
        }
    }
    SpriteGenerator sg;

    public Player player;
    GameObject playerGO;
    public Sprite playerSprite;
    public Text playerHealth;
    public Text AbilityUses;

    public Corral corral;

    public void Start()
    {
        corral = new Corral();
    }

    public void spawnPlayer(Point p)
    {
        if (player == null)
        {
            player = new Player(p.x, p.y, "SLIME", 10, CHARACTER_TYPE.PLAYER);
        }
        player.x = p.x;
        player.y = p.y;

        playerGO = new GameObject();
        playerGO.name = player.name;
        playerGO.AddComponent<SpriteRenderer>();
        playerGO.transform.position = new Vector3(player.x, player.y, 0);
        playerGO.transform.SetParent(this.transform, true);

        SpriteGenerator sg = ScriptableObject.CreateInstance<SpriteGenerator>();
        Sprite ps = Sprite.Create(sg.bytestotex2d(player.texture), new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);

        playerGO.GetComponent<SpriteRenderer>().sprite = sg.addXMirror(ps);

        sg = null;

        playerGO.GetComponent<SpriteRenderer>().sortingOrder = 10;
        Debug.Log("Spawning player with stats:"+player.stats.ToString());
        playerGO.GetComponent<SpriteRenderer>().color = new Color32((byte)player.stats.x, (byte)player.stats.y, (byte)player.stats.z, 255);

    }

    internal void createPlayer()
    {
        if (player == null)
        {
            player = new Player(0, 0, "SLIME", 10, CHARACTER_TYPE.PLAYER);
        }
    }

    // Update is called once per frame
    void Update () {
        if (AbilityUses != null && player != null)
        {
            Ability a = player.getAbility("Teleport");
            AbilityUses.text = a.name+": " + a.current_uses + "/" + a.max_uses;
        }
        else
        {
            GameObject go = GameObject.Find("AbilityUses");
            if (go != null)
                AbilityUses = go.GetComponent<Text>();
        }

        if (playerHealth != null && player != null)
        {
            playerHealth.text = "Health: " + player.current_health + "/" + player.max_health;

            if (player.current_health <= 0)
            {
                Debug.Log("YOU ARE DEAD.");
            }
        } else
        {
            GameObject go = GameObject.Find("PlayerHealth");
            if (go != null)
                playerHealth = go.GetComponent<Text>();
        }
        if (player != null && playerGO != null)
        {
            float rotation = 0;
            if (player.facing == CHARACTER_FACING.UP)
            {
                rotation = 180;
            }
            if (player.facing == CHARACTER_FACING.LEFT)
            {
                rotation = -90;
            }
            if(player.facing == CHARACTER_FACING.DOWN)
            {
                rotation = 0;
            }
            if (player.facing == CHARACTER_FACING.RIGHT)
            {
                rotation = 90;
            }
            //playerGO.transform.rotation = Quaternion.Euler(0, 0, rotation);

            playerGO.transform.position = new Vector3(player.x, player.y, 0);
            Camera.main.transform.position = new Vector3(player.x, player.y, Camera.main.transform.position.z);
        }
    }

    // do we need to depspawn the player?
    public void despawnPlayer()
    {
        Destroy(playerGO);
        //player = null;

    }
}

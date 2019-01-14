using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {

    public bool usingAbility;
    public Ability activeAbility;
    public List<Item> items;

    public void pickupItem(Item item)
    {
        items.Add(item);
    }

    // Coverts the monster into the current active player.
    public static explicit operator Player(Monster v)
    {
        Player p = new Player(v.x, v.y, v.name, v.max_health, CHARACTER_TYPE.PLAYER);
        p.texture = v.texture;
        p.abilities = v.abilities;
        p.stats = v.stats;
        Debug.Log("Monster Changed to active Player");
        return p;
    }

    public void dropItem(Item item)
    {
        items.Remove(item);
    }

    public void emptyItems()
    {
        items.Clear();
    }

    public void useAbility(Ability a)
    {
        if(abilities.Contains(a))
        {
            activeAbility = a;
            usingAbility = true;
        }
    }

    public Player(int x, int y, string name, int health, CHARACTER_TYPE type) : base(x, y, name, health, type)
    {
        stats = new Vector3Int(255, 255, 255);
        speed = 1;
        items = new List<Item>();


        Debug.Log("Player was created!!!!!!!!!!!!!!!!");
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {

    public bool usingAbility;
    public Ability activeAbility;
    public List<Item> items;
    public int controllingMonster;

    public void pickupItem(Item item)
    {
        items.Add(item);
    }

    // Coverts the monster into the current active player.
    public void assignMonsterToPlayer(Monster mon)
    {
        x = mon.x;
        y = mon.y;
        name = mon.name;
        max_health = mon.max_health;
        current_health = mon.max_health;

        texture = mon.texture;
        abilities = mon.abilities;
        stats = mon.stats;

        controllingMonster = mon.monsterID;

        level = mon.level;
        current_exp = mon.current_exp;
        next_exp = mon.next_exp;

        Debug.Log("Monster Changed to active Player");
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
            // allow the ability to be used if current_uses is above 0 or max_users is 0 because it means unlimited uses.
            if (a.current_uses > 0 || a.max_uses == 0)
            {
                activeAbility = a;
                usingAbility = true;
            } else
            {
                Debug.Log("No uses of ability left");
            }
        }
    }

    public Player(int x, int y, string name, int health, CHARACTER_TYPE type) : base(x, y, name, health, type)
    {
        stats = new Vector3Int(255, 255, 255);
        speed = 1;
        items = new List<Item>();
        Debug.Log("Player was created!!!!!!!!!!!!!!!!");
    }

    // we can call this and pass in the ability, or assume we are using the current active ability
    internal void usedAbility(Ability a = null)
    {
        if (a == null)
        {
            Debug.Log(activeAbility.current_uses);
            activeAbility.current_uses--;
            Debug.Log(activeAbility.current_uses);
            usingAbility = false;
        }
    }
}

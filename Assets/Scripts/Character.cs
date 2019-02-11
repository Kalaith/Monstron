using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CHARACTER_TYPE {ALL, NPC, MONSTER, PLAYER };
public enum CHARACTER_FACING {UP, DOWN, LEFT, RIGHT };

public class Character {

    // Naive Hybrid Stats
    public Vector3Int stats;
    public int x { get; set; }
    public int y { get; set; }
    public int speed { get; set; } // how fast can the character move
    public string name { get; set; }
    public CHARACTER_TYPE type { get; set; }


    public int max_health { get; set; } // Characters max health
    public int current_health { get; set; } // Characters current health

    public int current_exp { get; set; }
    public int next_exp { get; set; }
    public int level { get; set; }

    public CHARACTER_FACING facing { get; set; }
    
    // the bytes that make up the texture, so it can be saved to disk and not have to be recreated from templates
    public byte[] texture { get; set; }

    public List<Ability> abilities;

    public void levelUp()
    {
        if (current_health > 0)
        {
            while (current_exp >= next_exp)
            {
                level++;
                current_exp -= next_exp;
                max_health += (max_health / level);
                current_health = max_health;
                setNextLevel();
                Debug.Log(name+" leveled up!");
            }
        } else
        {
            Debug.Log("Can't level up if dead.");
        }
    }

    public void addAbility(Ability a)
    {
        abilities.Add(a);
    }

    public Ability getAbility(string abilityName)
    {
        foreach(Ability a in abilities)
        {
            if(a.name == abilityName)
            {
                return a;
            }
        }
        return null;
    }

    public void removeAbility(Ability a)
    {
        abilities.Remove(a);
    }

    public bool hasAbility(Ability a)
    {
        return abilities.Contains(a);
    }

    public Character(int x, int y, string name, int health, CHARACTER_TYPE type)
    {
        this.x = x;
        this.y = y;
        this.name = name;
        this.type = type;

        max_health = health;
        current_health = health;
        speed = 1;
        facing = CHARACTER_FACING.DOWN;
        abilities = new List<Ability>();

        setNextLevel();

    }

    public void takeDamage(int damage) {
        current_health -= damage;
    }

    public override string ToString()
    {
        return "Monster " + stats.ToString();
    }

    // Set the experience for the next level, currently based of level * health so stronger monsters will take longer to level up.
    internal void setNextLevel()
    {
        // if its a new monster level might not have been set.
        if (level == 0)
        {
            current_exp = 0;
            level = 1;
        }

        next_exp = level * max_health;

        if(next_exp == 0)
        {
            Debug.Log("0 next_exp: "+level+" - "+max_health);
        }
    }
}

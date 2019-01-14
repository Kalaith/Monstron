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

    public int attack { get; set; } // Attack/damage stat how much is removed on each hit.
    public CHARACTER_FACING facing { get; set; }
    
    // the bytes that make up the texture, so it can be saved to disk and not have to be recreated from templates
    public byte[] texture { get; set; }

    public List<Ability> abilities;

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

    }

    public void takeDamage(int damage) {
        current_health -= damage;
    }

    public override string ToString()
    {
        return "Monster " + stats.ToString();
    }
}

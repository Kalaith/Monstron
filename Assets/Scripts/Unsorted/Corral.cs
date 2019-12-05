using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// List of monsters that the player has, will need to keep sprite information
[Serializable]
public class Corral {

    public List<Monster> monsters;

    public Corral() {
        monsters = new List<Monster>();
    }

    public void addMonster(Monster m)
    {
        monsters.Add(m);
    }

    public void removeMonster(Monster m)
    {
        monsters.Remove(m);
    }

    public int corralMonsters()
    {
        return monsters.Count;
    }


    public Monster breedMonsters(Monster m1, Monster m2)
    {
        int r = 0;
        int g = 0;
        int b = 0;

        r = halfValue(m1.stats.x, m2.stats.x);
        g = halfValue(m1.stats.y, m2.stats.y);
        b = halfValue(m1.stats.z, m2.stats.z);

        Monster m3 = new Monster(m1.x, m2.y, m1.name.Substring(0, 3) + m2.name.Substring(0, 3), m1.max_health + m2.max_health, CHARACTER_TYPE.MONSTER, new Vector3Int(r, g, b), 0);

        // Assign abilities, one parent can be a direct 1to1 copy the other has to be added one by one.
        // hopefully these are now seperate, might have to double check dont want m1 to get m2 abilities.
        m3.abilities = m1.abilities;
        foreach (Ability a in m2.abilities)
        {
            if(!m3.abilities.Contains(a))
                m3.addAbility(a);
        }
        SpriteGenerator sg = ScriptableObject.CreateInstance<SpriteGenerator>();

        m3.texture = sg.tex2dtobytes(sg.mergeTextures(sg.bytestotex2d(m1.texture), sg.bytestotex2d(m2.texture)));

        monsters.Add(m3);
        return m3;
    }

    public int halfValue(int v1, int v2)
    {
        int v3 = v1 + v2;

        if (v1 == 0 || v2 == 0)
        {
            if (v3 != 0)
            {
                v3 = v3 / 2;
            }
        }
        else
        {
            v3 = v3 / 2;
        }

        return v3;
    }

    // May return null if monster does't exist.
    public Monster getMonsterByID(int monsterID)
    {
        Monster m = monsters.Find(x => x.monsterID == monsterID);

        return m;
    }

    internal void updateMonster(Player player)
    {
        Monster m = getMonsterByID(player.controllingMonster);

        if (m != null)
        {
            Debug.Log("Monster: " + m.ToString());

            m.level = player.level;
            m.current_exp = player.current_exp;
            m.next_exp = player.next_exp;

            // See if the monster needs to level up.
            m.levelUp();

            Debug.Log("Monster exp after player: " + m.current_exp);
        }
        else
        {
            Debug.Log("Monster not found.");
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ITEM_TYPE { NOTHING, EGG };

// A generic item, currently contains classes for other item types.
public class Item {

    public string name { get; set; }
    public double weight { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public ITEM_TYPE itemType { get; set; }
    public Vector3Int stats { get; set; }

    public Item(string name, double weight, int x, int y, Vector3Int stats, ITEM_TYPE type)
    {
        this.name = name;
        this.weight = weight;
        this.x = x;
        this.y = y;
        this.stats = stats;
        itemType = type;
    }

}

public class Egg : Item
{
    public Monster parent; // which monster did this come from
    public int level; // what level did we did this egg

    public Egg(string name, double weight, int x, int y, Monster parent, int level, Vector3Int stats, ITEM_TYPE type) : base(name, weight, x, y, stats, type)
    {
        this.parent = parent;
        this.level = level;
        
    }
}
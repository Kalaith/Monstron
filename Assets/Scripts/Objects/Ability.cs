using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for abilities
[Serializable]
public class Ability {
    public string name;
    public bool passive; // is it an active or passive skill
    public bool collide; // Can they target a cell that contains something
    public int damage; // how much damage does it do
    public int range; // how far from the current player position can it be cast
    public bool diagonal;
    // how far they extend from the initial target position
    public int upRange; // From the select location how far does it travel in the 4 directions
    public int downRange;
    public int leftRange;
    public int rightRange;
    public CHARACTER_TYPE type;
    public int max_uses;
    public int current_uses; // uses available to be used.

    public Ability(string name, bool passive, bool collide, int damage, int range, bool diag, int up, int down, int left, int right, CHARACTER_TYPE type, int uses)
    {
        this.name = name;
        this.passive = passive;
        this.collide = collide;
        this.damage = damage;
        this.range = range;
        this.diagonal = diag;
        this.upRange = up;
        this.downRange = down;
        this.leftRange = left;
        this.rightRange = right;
        this.type = type;
        max_uses = uses;
        current_uses = uses;
    }
}

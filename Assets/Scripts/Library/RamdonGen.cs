using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGen {
    public System.Random rand;
    int seed;

    public RandomGen(int seed)
    {
        if (seed != 0)
        {
            this.seed = seed;
        }
        else
        {
            this.seed = System.DateTime.Now.Millisecond;
            Debug.Log("Using Seed:" + this.seed);
        }

        rand = new System.Random(this.seed);
    }

    public void setSeed(int seed)
    {
        this.seed = seed;
    }

    public int getSeed()
    {
        return seed;
    }

    public int range(int num1, int num2)
    {
        return rand.Next(num1, num2);
    }
}

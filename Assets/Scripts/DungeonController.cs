using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour {

	// Use this for initialization
	void Start () {

        GameController.game.level = 0;
        GameController.game.loadMap();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedMonster : MonoBehaviour {

    public Monster monster;

	public void selectedMon()
    {
        Debug.Log("You have selected "+monster.name);
        PlayerController.playerC.player = (Player)monster;
        Debug.Log(PlayerController.playerC.player.stats.ToString());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedMonster : MonoBehaviour {

    public Monster monster;
    public GameObject thisMonster;

    public void selectedMon()
    {

        Debug.Log("You have selected "+monster.monsterID+":"+monster.name);

        CorralController.corral.setSelectedMonster(thisMonster);

        PlayerController.playerC.player.assignMonsterToPlayer(monster);
        Debug.Log(PlayerController.playerC.player.stats.ToString());

        thisMonster.GetComponent<Image>().color = new Color32(44, 58, 226, 100);
        

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownInputController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Cheat, create a monster
        if (Input.GetKeyUp("c"))
        {
            // create a random monster
            Monster mon = new Monster(new Point(0, 0), "CHEATMON", 10, CHARACTER_TYPE.MONSTER, new Vector3Int(1, 1, 1), 1);

            // Create inital ability and assign to both player (selected monster and the first monster)
            Ability teleport = new Ability("Teleport", false, false, 0, 2, true, 0, 0, 0, 0, CHARACTER_TYPE.ALL, 5);
            mon.addAbility(teleport);

            SpriteGenerator sg = ScriptableObject.CreateInstance<SpriteGenerator>();
            mon.texture = sg.tex2dtobytes(PlayerController.playerC.playerSprite.texture);

            PlayerController.playerC.corral.addMonster(mon);
            Debug.Log("Creating a cheat monster");
        }

        // Cheat, level up all player monsters
        if (Input.GetKeyUp("l"))
        {
            // create a random monster
            foreach (Monster mon in PlayerController.playerC.corral.monsters)
            {
                mon.current_exp = mon.next_exp;
                mon.levelUp();

            }
            Debug.Log("Monsters have all leveled up.");
        }
    }
}

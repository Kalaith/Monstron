using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Town : MonoBehaviour {

    public GameObject text;
    public GameObject dialog;
    public GameObject corral;
    public GameObject breed;
    public GameObject corralContent;
    public GameObject monPrefab;

    public void enterDungeon()
    {
        SceneManager.LoadScene("Dungeon");
        GameController.game.level = 0;

        GameController.game.loadMap();
    }

    public Dropdown dropdown1;
    public Dropdown dropdown2;

    SpriteGenerator sg;

    Monster[] monster;

    public GameObject monster1;
    public GameObject monster2;

    public void updateBreedSprites()
    {
        Debug.Log("Updating Sprites");

        Monster mon1 = monster[dropdown1.value];
        Monster mon2 = monster[dropdown2.value];

        Sprite sprite1 = sg.addXMirror(sg.bytesToSprite(mon1.texture));
        Sprite sprite2 = sg.addXMirror(sg.bytesToSprite(mon2.texture));

        monster1.GetComponent<Image>().sprite = sprite1;
        monster1.GetComponent<Image>().color = new Color32((byte)mon1.stats.x, (byte)mon1.stats.y, (byte)mon1.stats.z, 255); // monster.stats.x/255, monster.stats.y/255, monster.stats.z/255);

        monster2.GetComponent<Image>().sprite = sprite2;
        monster2.GetComponent<Image>().color = new Color32((byte)mon2.stats.x, (byte)mon2.stats.y, (byte)mon2.stats.z, 255); // monster.stats.x/255, monster.stats.y/255, monster.stats.z/255);

        Debug.Log("Monster 1"+monster[dropdown1.value].ToString());
        Debug.Log("Monster 1"+monster[dropdown2.value].ToString());

    }

    public void breedMonsters()
    {
        textObject.text = "";
        corral.SetActive(false);
        breed.SetActive(true);
        monster = PlayerController.playerC.corral.monsters.ToArray();

        sg = ScriptableObject.CreateInstance<SpriteGenerator>();

        List<string> m_DropOptions = new List<string>();
        Sprite sprite = null;
        foreach (Monster mon in monster) {
            m_DropOptions.Add(mon.name);
            if(sprite == null)
            {
                sprite = sg.addXMirror(sg.bytesToSprite(mon.texture));
                
            }
        }

        //Clear the old options of the Dropdown menu
        dropdown1.ClearOptions();
        //Add the options created in the List above
        dropdown1.AddOptions(m_DropOptions);

        //Clear the old options of the Dropdown menu
        dropdown2.ClearOptions();
        //Add the options created in the List above
        dropdown2.AddOptions(m_DropOptions);

        updateBreedSprites();

    }

    public void startBreeding()
    {
        if(dropdown1.value == dropdown2.value)
        {
            Debug.Log("You can't breed the monster with itself!");
        } else
        {
            Debug.Log("Breeding");
            PlayerController.playerC.corral.breedMonsters(monster[dropdown1.value], monster[dropdown2.value]);          
        }
    }

    public void viewCorral()
    {
        textObject.text = "";

        corral.SetActive(true);
        breed.SetActive(false);

        Debug.Log("View Corral");
        // if we have viewed the corral before, wipe it first.
        foreach(Transform child in corralContent.transform)
        {
            
            Destroy(child.gameObject);
        }

        SpriteGenerator sg = ScriptableObject.CreateInstance<SpriteGenerator>();
        foreach(Monster mon in PlayerController.playerC.corral.monsters)
        {
            GameObject monSprite = Instantiate(monPrefab);
            Sprite s = sg.addXMirror(sg.bytesToSprite(mon.texture));

            GameObject childSprite = monSprite.transform.Find("monSprite").gameObject;
            GameObject childName = monSprite.transform.Find("monName").gameObject;
            GameObject childRed = monSprite.transform.Find("txtRedVal").gameObject;
            GameObject childGreen = monSprite.transform.Find("txtGreenVal").gameObject;
            GameObject childBlue = monSprite.transform.Find("txtBlueVal").gameObject;
            GameObject childHealth = monSprite.transform.Find("txtHealthVal").gameObject;
            
            childName.GetComponent<Text>().text = mon.name;
            childRed.GetComponent<Text>().text = mon.stats.x.ToString();
            childGreen.GetComponent<Text>().text = mon.stats.y.ToString();
            childBlue.GetComponent<Text>().text = mon.stats.z.ToString();
            childHealth.GetComponent<Text>().text = mon.max_health.ToString();

            childSprite.GetComponent<Image>().sprite = s;
            childSprite.GetComponent<Image>().color = new Color32((byte)mon.stats.x, (byte)mon.stats.y, (byte)mon.stats.z, 255); // monster.stats.x/255, monster.stats.y/255, monster.stats.z/255);

            monSprite.GetComponent<SelectedMonster>().monster = mon;
            monSprite.transform.SetParent(corralContent.transform, false);
        }
    }

    Text textObject;

    public void Start()
    {
        textObject = dialog.GetComponent<Text>();

        // if they only have 1 monster and no items show the welcome screen
        if (PlayerController.playerC.corral.corralMonsters() == 1 && PlayerController.playerC.player.items.Count == 0)
        {
            // Going to assume if player isn't set first time loading the game.
            textObject.text = "Welcome to Monstron!\n\n You have been given a monster but its weak\n use it to find a monster egg inside the dungeon \n\n Goodluck!\n You can use T to teleport short distances\n WASD to move\n";
        }

        if (PlayerController.playerC.player.items.Count > 0)
        {
            foreach (Item item in PlayerController.playerC.player.items)
            {
                if(PlayerController.playerC.corral.corralMonsters() == 1)
                {
                    textObject.text = "You have found an egg\n\n it will hatch soon you can also breed it with your current monster to create a hyrbid\n raise monsters and conqueror the dungeon!\n";
                }
                Egg egg = (Egg)item;

                textObject.text = textObject.text + "Found an egg, looks like its hatching you now have a " + egg.parent.name + "\n You can select it from the Corral in the dungeon it can use E to attack\n";
                
                PlayerController.playerC.corral.addMonster(egg.parent);
            }
            PlayerController.playerC.player.items.Clear();

        }

        if(textObject.text == "")
        {
            textObject.text = "You can find monster eggs in the dungeon\n T to teleport, E to Attack (if your monster can attack) WASD to move";
        }

    }


}

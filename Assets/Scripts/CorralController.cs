using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorralController : MonoBehaviour
{
    public GameObject corralContent;
    public GameObject monPrefab;

    private GameObject selectedMonster;

    public static CorralController corral;

    private void Awake()
    {
        if (corral == null)
        {
            DontDestroyOnLoad(gameObject);
            corral = this;
        }
        else if (corral != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        displayCorral();
    }

    public void displayCorral()
    {
        // if we have viewed the corral before, wipe it first.
        foreach (Transform child in corralContent.transform)
        {

            Destroy(child.gameObject);
        }

        SpriteGenerator sg = ScriptableObject.CreateInstance<SpriteGenerator>();
        Monster m = PlayerController.playerC.corral.getMonsterByID(PlayerController.playerC.player.controllingMonster);

        foreach (Monster mon in PlayerController.playerC.corral.monsters)
        {
            // shouldn't need to do this here, but lets catch it before we display it to the player.
            mon.levelUp();
            mon.setNextLevel();

            GameObject monSprite = Instantiate(monPrefab);
            Sprite s = sg.addXMirror(sg.bytesToSprite(mon.texture));

            GameObject childSprite = monSprite.transform.Find("monSprite").gameObject;
            GameObject childName = monSprite.transform.Find("monName").gameObject;
            GameObject childRed = monSprite.transform.Find("txtRedVal").gameObject;
            GameObject childGreen = monSprite.transform.Find("txtGreenVal").gameObject;
            GameObject childBlue = monSprite.transform.Find("txtBlueVal").gameObject;
            GameObject childHealth = monSprite.transform.Find("txtHealthVal").gameObject;
            GameObject childExp = monSprite.transform.Find("txtExpVal").gameObject;

            childName.GetComponent<Text>().text = mon.name;
            childRed.GetComponent<Text>().text = mon.stats.x.ToString();
            childGreen.GetComponent<Text>().text = mon.stats.y.ToString();
            childBlue.GetComponent<Text>().text = mon.stats.z.ToString();
            childHealth.GetComponent<Text>().text = mon.max_health.ToString();

            Debug.Log("Mon Exp: " + mon.current_exp);
            childExp.GetComponent<Text>().text = (mon.current_exp + "/" + mon.next_exp);

            childSprite.GetComponent<Image>().sprite = s;
            childSprite.GetComponent<Image>().color = new Color32((byte)mon.stats.x, (byte)mon.stats.y, (byte)mon.stats.z, 255); // monster.stats.x/255, monster.stats.y/255, monster.stats.z/255);

            if (mon == m)
            {
                // highlight this as the selected monster
                monSprite.GetComponent<Image>().color = new Color32(44, 58, 226, 100);
                selectedMonster = monSprite;
            }


            monSprite.GetComponent<SelectedMonster>().monster = mon;
            monSprite.GetComponent<SelectedMonster>().thisMonster = monSprite;
            monSprite.transform.SetParent(corralContent.transform, false);
        }
    }

    public void setSelectedMonster(GameObject newSelMonster)
    {
        // reset the previous selected monster back to normal
        selectedMonster.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        selectedMonster = newSelMonster;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

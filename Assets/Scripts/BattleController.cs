using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Manages the flow of battle
// Enemy Team, Created based on oppoent and level
// Player Team, Selected by the player

// Turn order, based on speed and last moved or player turn/monster turn..
// or active battle system, 
// each character would have a time until they can make the next action inital would be done by speed.


public class BattleController : MonoBehaviour
{
    List<Monster> playerTeam;
    List<Monster> enemyTeam;
    private Monster awaitingCommand;

    // Start is called before the first frame update
    void Start()
    {
        // Game/Map get current level
        setupPlayerTeam();
        generateEnemyTeam(new Monster(new Point(0,0), "test", 1, CHARACTER_TYPE.MONSTER, new Vector3Int(1, 1, 1), 10), GameController.game.level);
    }

    // create the ui elements for the player, make sure all the variables are set correctly
    private void setupPlayerTeam()
    {
        foreach(Monster mon in playerTeam)
        {
            // Add mon to UI.
            // also do life bars
        }
    }

    // builds a team of enemies based on the monster passwed in and current level
    private void generateEnemyTeam(Monster monster, int level)
    {
        enemyTeam = new List<Monster>();
        enemyTeam.Add(monster);

        // layout the monsters in the gui.
        foreach (Monster mon in enemyTeam)
        {
            // Add mon to UI.
        }
    }

    // Update is called once per frame
    void Update()
    {
    
        if(awaitingCommand.ready)
        {
            foreach(Monster player in playerTeam)
            {
                player.Tick(Time.deltaTime);

                if(player.ready)
                {
                    awaitingCommand = player;
                }
            }

            foreach(Monster enemy in enemyTeam)
            {
                enemy.Tick(Time.deltaTime);

                if(enemy.ready)
                {
                    awaitingCommand = enemy;
                    preformAction(enemy);
                }
            }
        }
    }

    // enemy units dont need to wait, this will choose an action at random and kickoff the animation.
    private void preformAction(Monster enemy)
    {
        // prob just pick an action out of the available actions at random.

        // the monster has acted, we are not waiting for a command now.
        awaitingCommand = null;
    }
}

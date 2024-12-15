using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    GameObject selectGO;
    public Sprite select;

    // Update is called once per frame
    void Update()
    {
        if (!ShouldProcessInput())
            return;

        Vector3 mousePos = GetAdjustedMousePosition();
        ProcessAbility(mousePos);
    }

    private bool ShouldProcessInput()
    {
        return Input.GetMouseButtonDown(0) && PlayerController.playerC.player.usingAbility;
    }

    private Vector3 GetAdjustedMousePosition()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.x += 0.5f;
        mouse.y += 0.5f;
        return mouse;
    }

    private void ProcessAbility(Vector3 mousePos)
    {
        string abilityName = PlayerController.playerC.player.activeAbility.name;
        switch (abilityName)
        {
            case "Teleport":
                HandleTeleport(mousePos);
                break;
            case "Attack":
                HandleAttack(mousePos);
                break;
        }
    }
    
    private void HandleTeleport(Vector3 mouse)
    {
        if (!IsValidPosition((int)mouse.x, (int)mouse.y))
            return;

        Tile tile = MapController.mapC.map.getTileAt(new Point((int)mouse.x, (int)mouse.y));
        if (PlayerController.playerC.player.activeAbility.range < tile.distanceToTile(new Point(PlayerController.playerC.player.x, PlayerController.playerC.player.y)))
            return;

        if (selectGO == null)
            CreateSelectGO();

        if (selectGO.transform.position == new Vector3((int)mouse.x, (int)mouse.y, 0))
        {
            // Teleport action
            Destroy(selectGO);
            PlayerController.playerC.player.x = (int)mouse.x;
            PlayerController.playerC.player.y = (int)mouse.y;
            PlayerController.playerC.player.usedAbility();
            MoveMonsters();
        }
        else
        {
            UpdateSelectGO(mouse);
        }
    }

    private bool IsValidPosition(int x, int y)
    {
        return MapController.mapC.map != null && MapController.mapC.map.isPassable(x, y);
    }

    void HandleAttack(Vector3 mouse)
    {
        Monster monster = EnemyController.enemyC.getMonsterAt((int)mouse.x, (int)mouse.y);
        if (monster == null)
            return;

        Tile tile = MapController.mapC.map.getTileAt(new Point((int)mouse.x, (int)mouse.y));
        if (PlayerController.playerC.player.activeAbility.range < tile.distanceToTile(new Point(PlayerController.playerC.player.x, PlayerController.playerC.player.y)))
            return;

        if (selectGO == null)
            CreateSelectGO();

        if (selectGO.transform.position == new Vector3((int)mouse.x, (int)mouse.y, 0))
        {
            // Attack action
            Destroy(selectGO);
            monster.takeDamage(PlayerController.playerC.player.activeAbility.damage);
            PlayerController.playerC.player.usingAbility = false;
            EnemyController.enemyC.updateMonsters();
        }
        else
        {
            UpdateSelectGO(mouse);
        }
    }

    void CreateSelectGO()
    {
        selectGO = new GameObject();
        selectGO.transform.SetParent(this.transform, true);
        SpriteRenderer renderer = selectGO.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 5;
        renderer.sprite = select;
    }

    void UpdateSelectGO(Vector3 mouse)
    {
        selectGO.name = $"select_X{(int)mouse.x}Y{(int)mouse.y}";
        selectGO.transform.position = new Vector3((int)mouse.x, (int)mouse.y, 0);
    }

    void MoveMonsters()
    {
        foreach (Monster monster in EnemyController.enemyC.monsters.Keys)
        {
            monster.moveMonster(MapController.mapC.map, PlayerController.playerC.player);
        }
    }
}

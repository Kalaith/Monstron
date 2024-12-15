public class BattleController : MonoBehaviour
{
    [SerializeField] private Transform playerTeamPositions;
    [SerializeField] private Transform enemyTeamPositions;
    
    private List<Monster> playerTeam = new List<Monster>();
    private List<Monster> enemyTeam = new List<Monster>();
    private Queue<Monster> turnOrder = new Queue<Monster>();
    private BattleState currentState;
    private Monster activeMonster;

    public enum BattleState
    {
        Start,
        PlayerTurn,
        EnemyTurn,
        Victory,
        Defeat
    }

    public void InitializeBattle(List<Monster> playerMonsters, Monster enemyLeader, int dungeonLevel)
    {
        playerTeam = playerMonsters;
        enemyTeam = GenerateEnemyTeam(enemyLeader, dungeonLevel);
        SpawnTeams();
        SetupTurnOrder();
        currentState = BattleState.Start;
        StartCoroutine(BattleLoop());
    }

    private List<Monster> GenerateEnemyTeam(Monster leader, int level)
    {
        List<Monster> team = new List<Monster> { leader };
        int teamSize = Mathf.Min(3 + level / 2, 6);
        
        for (int i = 1; i < teamSize; i++)
        {
            team.Add(EnemyController.enemyC.CreateRandomMonster(level));
        }
        return team;
    }

    private void SpawnTeams()
    {
        for (int i = 0; i < playerTeam.Count; i++)
        {
            SpawnMonster(playerTeam[i], playerTeamPositions.GetChild(i).position);
        }
        
        for (int i = 0; i < enemyTeam.Count; i++)
        {
            SpawnMonster(enemyTeam[i], enemyTeamPositions.GetChild(i).position);
        }
    }

    private void SetupTurnOrder()
    {
        List<Monster> allMonsters = new List<Monster>();
        allMonsters.AddRange(playerTeam);
        allMonsters.AddRange(enemyTeam);
        
        allMonsters.Sort((a, b) => b.speed.CompareTo(a.speed));
        
        foreach (Monster monster in allMonsters)
        {
            turnOrder.Enqueue(monster);
        }
    }

    private IEnumerator BattleLoop()
    {
        while (currentState != BattleState.Victory && currentState != BattleState.Defeat)
        {
            activeMonster = turnOrder.Dequeue();
            
            if (playerTeam.Contains(activeMonster))
            {
                currentState = BattleState.PlayerTurn;
                yield return StartCoroutine(PlayerTurn());
            }
            else
            {
                currentState = BattleState.EnemyTurn;
                yield return StartCoroutine(EnemyTurn());
            }
            
            turnOrder.Enqueue(activeMonster);
            CheckBattleEnd();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void CheckBattleEnd()
    {
        if (enemyTeam.TrueForAll(m => m.current_health <= 0))
            currentState = BattleState.Victory;
        else if (playerTeam.TrueForAll(m => m.current_health <= 0))
            currentState = BattleState.Defeat;
    }

    private IEnumerator PlayerTurn()
    {
        // Wait for player input
        awaitingCommand = activeMonster;
        yield return new WaitUntil(() => awaitingCommand == null);
    }

    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);
        // AI decides action
        Monster target = playerTeam[UnityEngine.Random.Range(0, playerTeam.Count)];
        UseAbility(activeMonster.abilities[0], target);
    }

    public void UseAbility(Ability ability, Monster target)
    {
        ability.Execute(activeMonster, target);
        awaitingCommand = null;
    }
}
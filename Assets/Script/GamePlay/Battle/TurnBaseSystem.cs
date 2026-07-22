using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnBaseSystem : MonoBehaviour
{
    BattleMaster battleMaster;
    DeckBuilderMaster deckBuilderMaster;

    PlayerBody playerBody;
    List<EnemyBody> enemyBodyList = new List<EnemyBody>();

    int round = 1;
    int maxTurn;
    int currentTurn;


    public void SetupTurnBaseSystem(PlayerBody _playerBody, EnemyBody[] _enemyBodys)
    {
        playerBody = _playerBody;
        enemyBodyList = _enemyBodys.ToList();

        maxTurn = enemyBodyList.Count + 1;

        battleMaster = FindAnyObjectByType<BattleMaster>();
        deckBuilderMaster = FindAnyObjectByType<DeckBuilderMaster>();

        Invoke("PlayTurn", .1f);
    }

    void PlayTurn()
    {
        if(currentTurn >= maxTurn)
        {
            NewRound();
        }

        //player turn
        if(currentTurn == 0)
        {
            deckBuilderMaster.DrawCardOnHand();
        }
        //enemy turn
        else
        {
            int n = currentTurn - 1;
            enemyBodyList[n].GetEnemyBrain().PlayThisTurn();
        }
    }

    public void PlayNextTurn()
    {
        currentTurn++;
        Invoke("PlayTurn", 1f);
    }

    void NewRound()
    {
        if(battleMaster.gamePlayCondition != GamePlayCondition.Playing) return;
        
        currentTurn = 0;
        round++;

        playerBody.buffDebuffHandler.CheckAllEffect();
        foreach(EnemyBody enemyBody in enemyBodyList)
        {
            enemyBody.buffDebuffHandler.CheckAllEffect();
        }
    }

    public void RemoveEnemy(EnemyBody enemyBody)
    {
        maxTurn--;
        enemyBodyList.Remove(enemyBody);
    }

    public PlayerBody GetPlayerBody()=> playerBody;
    public EnemyBody[] GetEnemyBody()=> enemyBodyList.ToArray();

}

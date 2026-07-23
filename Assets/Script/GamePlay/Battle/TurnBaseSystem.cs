using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnBaseSystem : MonoBehaviour
{
    BattleMaster battleMaster;
    DeckBuilderMaster deckBuilderMaster;

    PlayerBody playerBody;
    List<EnemyBody> enemyBodyList = new List<EnemyBody>();
    List<EnemyBody> enemyBodyDeadTempList = new List<EnemyBody>();

    int round = 1;
    int maxTurn;
    int currentTurn;
    bool isCheckingEffect = false;

    public void SetupTurnBaseSystem(PlayerBody _playerBody, EnemyBody[] _enemyBodys)
    {
        playerBody = _playerBody;
        enemyBodyList = _enemyBodys.ToList();

        maxTurn = enemyBodyList.Count + 1;

        battleMaster = FindAnyObjectByType<BattleMaster>();
        deckBuilderMaster = FindAnyObjectByType<DeckBuilderMaster>();

        StartCoroutine(PlayTurn());
    }

    IEnumerator PlayTurn()
    {
        if(currentTurn >= maxTurn)
        {
            NewRound();
        }

        //player turn
        if(currentTurn == 0)
        {
            StartCoroutine(battleMaster.ShowTurnPanel("Your Turn"));
            yield return new WaitForSeconds(1);
            deckBuilderMaster.DrawCardOnHand();
        }
        //enemy turn
        else
        {
            int n = currentTurn - 1;
            if(n==0) StartCoroutine(battleMaster.ShowTurnPanel("Enemy Turn"));
            yield return new WaitForSeconds(1);

            enemyBodyList[n].GetEnemyBrain().PlayThisTurn();
        }
    }

    public void PlayNextTurn()
    {
        currentTurn++;
        StartCoroutine(PlayTurn());
    }

    void NewRound()
    {
        if(battleMaster.gamePlayCondition != GamePlayCondition.Playing) return;
        
        currentTurn = 0;
        round++;

        //check all buff and debuff
        isCheckingEffect = true;
        playerBody.buffDebuffHandler.CheckAllEffect();
        foreach(EnemyBody enemyBody in enemyBodyList)
        {
            enemyBody.buffDebuffHandler.CheckAllEffect();
        }
        isCheckingEffect = false;

        //check apakah ada enemy yg sudah mati
        foreach(EnemyBody enemyBody in enemyBodyDeadTempList)
        {
            enemyBodyList.Remove(enemyBody);
        }
        enemyBodyDeadTempList.Clear();
    }

    public void RemoveEnemy(EnemyBody enemyBody)
    {
        maxTurn--;

        //check apakah mati ketika sedang proses check effect, jika iya maka akan di simpan di list sementara
        if(isCheckingEffect)
            enemyBodyDeadTempList.Add(enemyBody);
        else
            enemyBodyList.Remove(enemyBody);
    }

    public PlayerBody GetPlayerBody()=> playerBody;
    public EnemyBody[] GetEnemyBody()=> enemyBodyList.ToArray();

}

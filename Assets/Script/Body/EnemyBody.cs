using UnityEngine;

public class EnemyBody : MainBody
{
    [Header("Enemy Brain")]
    [SerializeField] EnemyBrain enemyBrain;

    protected override void Setup()
    {
        base.Setup();
        

        healtHandler.Setup(this);
        buffDebuffHandler.Setup(this);

        enemyBrain.SetupEnemyBrain(this);
    }

    public override void Isdead()
    {
        base.Isdead();
        battleMaster.RemoveEnemy(this);
        this.gameObject.SetActive(false);
    }

    public EnemyBrain GetEnemyBrain() => enemyBrain;
}

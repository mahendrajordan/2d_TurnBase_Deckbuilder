using UnityEngine;

public class EnemyBody : MainBody
{
    protected override void Setup()
    {
        base.Setup();
        

        healtHandler.Setup(this);
    }

    public override void Isdead()
    {
        base.Isdead();
        this.gameObject.SetActive(false);
    }
}

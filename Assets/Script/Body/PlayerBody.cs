using UnityEngine;

public class PlayerBody : MainBody
{
    protected override void Setup()
    {
        base.Setup();
        

        healtHandler.Setup(this);
    }

    public override void Isdead()
    {
        base.Isdead();
        //gameover;
    }
}

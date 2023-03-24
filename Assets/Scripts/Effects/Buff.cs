using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : Effect //Effect apply instant on use 
{
    protected Chosen caster;
    protected Chosen target;
    public int nbTurnRemaining;

    public bool instant;
    public bool begin;
    

    public Buff(int nbTurn, bool instant){
        this.nbTurnRemaining = nbTurn;
        this.instant = instant;
    }

    public override void apply(Chosen caster, Chosen target)
    {
        this.caster = caster;
        this.target = target;
        //TODO Don't pass copy of the object instead make list buffs couple (counter, Buff) so each chosen hadle their own buffs cd (counter)
        target.buffs.Add((Buff) this.MemberwiseClone());
        Debug.Log("buff added");
        if(instant){
            applyBuff();
        }
    }

    public override void apply(Chosen caster, Obstacle target)
    {
        throw new System.NotImplementedException();
    }

    public virtual void applyBuff(){
        nbTurnRemaining--;
    }
}
public class MPbuff : Buff
{
    public int amount;
    public MPbuff( int nbTurn, bool instant, int amount) : base(nbTurn, instant)
    {
        this.amount = amount;
    }

    public override void applyBuff(){
        base.applyBuff();
        target.MP.Value += amount;
    }
}

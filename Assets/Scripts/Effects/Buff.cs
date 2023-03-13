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
        target.buffs.Add(this);
        if(instant){
            applyBuff();
        }
    }

    public override void apply(Chosen caster, Obstacles target)
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
        target.MP += amount;
    }
}

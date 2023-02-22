using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
  public abstract void apply(Chosen caster, Chosen target);
  public abstract void apply(Chosen caster, Obstacles target);
  /*public void apply(Chosen caster, EmptyTile target);*/

}

public class PhysicalDamage : Effect
{
    public int damage;
    public PhysicalDamage(int damage){
        this.damage = damage;
    }

    public override void apply(Chosen caster, Chosen target){
        float result = (float) damage * (float) caster.strength * (100-(float) target.armor)/100; 
        target.HP -= (int) result;
    }

    public override void apply(Chosen caster, Obstacles target){
        Debug.Log("You applied the spell on an obstacle!");
    }

    /*public void apply(Chosen caster, EmptyTile target){
    }*/
}
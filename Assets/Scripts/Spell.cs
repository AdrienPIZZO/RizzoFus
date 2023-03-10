using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Effect;

public struct CastingCondition {
    public (int,int) range;
    bool line;
    bool LOS;

    public CastingCondition((int,int) range, bool line, bool LOS){
        this.range = range;
        this.line = line;
        this.LOS = LOS;
    }
}

public class Spell
{
    public List<Effect> effects = new List<Effect>();

    private string name;
    public int pwrCost;
    public CastingCondition castingCondition;

    public Spell(string name, int pwrCost, CastingCondition cc){
        this.name = name;
        this.pwrCost = pwrCost;
        this.castingCondition = cc;
    }

    public string getName(){
        return name;
    }

    public void applyAllEffects(Chosen caster, Chosen target){
        foreach (Effect e in effects){
            e.apply(caster, target);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Effect;

public class Spell
{
    public List<Effect> effects = new List<Effect>();

    private string name;
    public int pwrCost;

    public Spell(string name, int pwrCost){
        this.name = name;
        this.pwrCost = pwrCost;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell //: MonoBehaviour
{

private string name;
private int phisycalDamage;
private int magicalDamage;
private int aoe;

public Spell(string name, int phisycalDamage, int magicalDamage, int aoe){
    this.name = name;
    this.phisycalDamage = phisycalDamage;
    this.magicalDamage = magicalDamage;
    this.aoe = aoe;
}

public string getName(){
    return name;
}

public void effectOnChosen(Chosen caster, Chosen target){
    target.HP -= phisycalDamage + magicalDamage;
}
}

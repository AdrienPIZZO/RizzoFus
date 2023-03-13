using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Chosen : Entity
{
    //STATS
    private const int MPMAX = 6;
    private const int HPMAX = 1000;
    private const int PWRGAUGEMAX = 200;
    private const int ARMORMAX = 50;// % based
    private const int STRENGTHMAX = 200;

    public int MP = MPMAX;
    public int HP = HPMAX;
    public int powerGauge = PWRGAUGEMAX/2;
    public int powerRegen = 50;
    public int armor = 10;// % based
    public int strength = 5;
    public Dictionary<int, Spell> spells = new Dictionary<int, Spell>();

    public List<Buff> buffs = new List<Buff>();
    public void addSpell(KeyValuePair<int, Spell> s){
        if(spells == null){
            Debug.Log("s");
        }
        spells.Add(s.Key, s.Value);
    }

    public void beginTurn(){
        foreach(Buff b in buffs.ToList()){
            if (b.nbTurnRemaining <= 0){
                Debug.Log("NOPE");
                buffs.Remove(b);
            } else {
                b.applyBuff();
            }
        }
    }
 
    public void passTurn()
    {
        MP = MPMAX;
        powerGauge = powerGauge + powerRegen <= PWRGAUGEMAX ? powerGauge + powerRegen : PWRGAUGEMAX;
    }

    private bool isDead()
    {
        return HP<=0;
    }

    public override bool targeted(Chosen caster, Spell s)
    {
        s.applyAllEffects(caster, this);
        Debug.Log("HP remaining: " + HP);
        return isDead();
    }

    public void useSpell(Square square, Spell spell)
    {
        if(powerGauge - spell.pwrCost >= 0){//if the chosen has enough power to cast the spell
            powerGauge -= spell.pwrCost;
            if (square.occupant != null){ //if the tile is not empty
                square.occupant.targeted(this, spell);
            }
             Debug.Log("Power Gauge: " + powerGauge);
        } else {
            Debug.Log("You don't have enough power to use this spell!");
        }

    }
}

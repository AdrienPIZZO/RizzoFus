using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chosen : Entity
{
    private const int MPMAX = 6;
    private const int HPMAX = 100;

    public int MP = MPMAX;
    public int HP = HPMAX;

    //private List<Spell> spells = new List<Spell>();
    public Dictionary<int, Spell> spells = new Dictionary<int, Spell>();

    public void addSpell(KeyValuePair<int, Spell> s){
        spells.Add(s.Key, s.Value);
    }
 
    public void MPReset()
    {
        MP = MPMAX;
    }

    private bool isDead()
    {
        return HP<=0;
    }

    public override bool targeted(Chosen caster, Spell s)
    {
        s.effectOnChosen(caster, this);
        Debug.Log(HP);
        return isDead();
    }

    public void useSpell(Entity target, Spell s)
    {
        if (target != null){ //if the tile is not empty
            target.targeted(this, s);
        } 

    }
}

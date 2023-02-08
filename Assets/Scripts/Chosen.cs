using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chosen : Entity
{
    private const int MPMAX = 6;
    private const int HPMAX = 100;

    public int MP = MPMAX;
    public int HP = HPMAX;


    public void MPReset()
    {
        MP = MPMAX;
    }

    private bool isDead()
    {
        return HP<=0;
    }

    public override bool receiveAttack(int amount)
    {
        HP -= amount;
        Debug.Log(HP);
        return isDead();
    }

    public void attack(Entity e)//Spell s
    {
        bool death = e.receiveAttack(10);

    }
}

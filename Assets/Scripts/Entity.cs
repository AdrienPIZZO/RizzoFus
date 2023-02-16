using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int currentX { set; get; }
    public int currentZ { set; get; }

    public void SetPosition(int x, int z)
    {
        currentX = x;
        currentZ = z;
    }

    public virtual bool targeted(Chosen caster, Spell s){
        Debug.Log("WTF");
        return false;
    }

}

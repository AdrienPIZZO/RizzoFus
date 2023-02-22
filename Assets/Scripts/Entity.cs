using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Board board;
    public int currentX { set; get; }
    public int currentZ { set; get; }

    public void SetPosition(int x, int z)
    {
        board.setElement(currentX, currentZ, null);
        currentX = x;
        currentZ = z;
        board.setElement(currentX, currentZ, this);
    }

    public void setBoard(Board board){
        this.board = board;
    }

    public virtual bool targeted(Chosen caster, Spell s){
        Debug.Log("WTF");
        return false;
    }
}

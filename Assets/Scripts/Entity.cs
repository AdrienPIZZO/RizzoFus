using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Entity : NetworkBehaviour
{
    public Board board;
    public NetworkVariable<int> x = new NetworkVariable<int>(-1);
    public NetworkVariable<int> z = new NetworkVariable<int>(-1);

    public void initPos(int x, int z){
        this.x.Value = x;
        this.z.Value = z;
    }
    public void ModifyExistingPosition(int newX, int newZ)
    {
        board.setEntityAtPos(this.x.Value, z.Value, null);
        x.Value = newX;
        z.Value = newZ;
        board.setEntityAtPos(x.Value, z.Value, this);
    }

    public void setBoard(Board board){
        Debug.Log("board: " + board==null);
        Debug.Log("obstacle: " + this==null);
        this.board = board;
    }

    public virtual bool targeted(Chosen caster, Spell s){
        Debug.Log("WTF");
        return false;
    }
}

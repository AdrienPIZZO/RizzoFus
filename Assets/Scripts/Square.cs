using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square
{

    public Board board;
    public int x;
    public int z;
    public Entity occupant;

    public Square(Board board, int x, int z){
        this.board = board;
        this.x = x;
        this.z = z;
        occupant = null;
    }
}

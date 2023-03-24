using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Effect
{
    protected Board board;
    public abstract void apply(Chosen caster, Chosen target);
    public abstract void apply(Chosen caster, Obstacle target);
    /*public void apply(Chosen caster, EmptySquare target);*/
}

public class PhysicalDamage : Effect
{
    public int damage;
    public PhysicalDamage(Board board, int damage){
        this.board = board;
        this.damage = damage;
    }

    public override void apply(Chosen caster, Chosen target){
        float result = (float) damage * (float) caster.strength.Value * (100-(float) target.armor.Value)/100; 
        target.HP.Value -= (int) result;
    }

    public override void apply(Chosen caster, Obstacle target){
        Debug.Log("You applied PhysicalDamage effect on an obstacle!");
    }
    /*public void apply(Chosen caster, EmptySquare target){
    }*/
}

public class MoveTarget : Effect
{
    public int nbSquare;
    
    public MoveTarget(Board board, int nbSquare){
        this.board = board;
        this.nbSquare = nbSquare;
    }

    public override void apply(Chosen caster, Chosen target){
        (int, int) vect = Utils.getVector(caster.x.Value, caster.z.Value, target.x.Value, target.z.Value); // Save vect between caster and target somewhere nice
        (int, int) orientation;
        if(Math.Abs(vect.Item1) > Math.Abs(vect.Item2)){
            orientation = (Math.Abs(vect.Item1)/vect.Item1, 0);
        } else if(Math.Abs(vect.Item1) < Math.Abs(vect.Item2)){
            orientation = (0, Math.Abs(vect.Item2)/vect.Item2);
        } else if(Math.Abs(vect.Item1) == Math.Abs(vect.Item2)){// Diagonal
            orientation = (Math.Abs(vect.Item1)/vect.Item1, Math.Abs(vect.Item2)/vect.Item2); // TODO: better math function
        }
        else{
            orientation = (0, 0);
            Debug.Log("Error orientation!");
        }
        //Debug.Log("vect: (" + vect.Item1 + "," + vect.Item2 +")");
        //Debug.Log("Orientation: (" + orientation.Item1 + "," + orientation.Item2 +")");
        int currentDistance = nbSquare;
        (int, int) currentPos = (target.x.Value, target.z.Value);
        while (currentDistance > 0 && board.IsSquareAvailable(currentPos.Item1 + orientation.Item1, currentPos.Item2 + orientation.Item2)){
            currentPos.Item1 += orientation.Item1;
            currentPos.Item2 += orientation.Item2;
            currentDistance--;
        }
        target.ModifyExistingPosition(currentPos.Item1, currentPos.Item2);
    }


    public override void apply(Chosen caster, Obstacle target){
        Debug.Log("You applied MoveTarget effect on an obstacle!");
    }

    /*public void apply(Chosen caster, EmptySquare target){
    }*/
}




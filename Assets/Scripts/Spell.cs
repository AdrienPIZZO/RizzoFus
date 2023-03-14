using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Effect;

public struct CastingCondition {
    public (int,int) range;
    public bool line;
    public bool LOS;

    public CastingCondition((int,int) range, bool line, bool LOS){
        this.range = range;
        this.line = line;
        this.LOS = LOS;
    }
}

public class Spell
{
    public List<Effect> effects = new List<Effect>();

    private string name;
    public int pwrCost;
    public CastingCondition castingCondition;

    public Spell(string name, int pwrCost, CastingCondition cc){
        this.name = name;
        this.pwrCost = pwrCost;
        this.castingCondition = cc;
    }

    public string getName(){
        return name;
    }

    public void applyAllEffects(Chosen caster, Chosen target){
        foreach (Effect e in effects){
            e.apply(caster, target);
        }
    }

    public void computeReachableSquares(Board board, (int, int) position){
        for(int i=castingCondition.range.Item1; i<=castingCondition.range.Item2; i++){
            List<(int, int)> squarePos = Utils.getSquaresAtRange(i, position, board);
            for(int j=0; j<squarePos.Count; j++){
                Debug.Log(squarePos[j]);
                board.reachableSquares[squarePos[j].Item1, squarePos[j].Item2]=2;
                board.squaresGO[squarePos[j].Item1, squarePos[j].Item2].GetComponentInParent<MeshRenderer>().material = board.materials[board.reachableSquares[squarePos[j].Item1, squarePos[j].Item2]];
            }
        }
    }
}

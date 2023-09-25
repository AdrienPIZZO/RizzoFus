using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Effect;
using System;

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
                board.reachableSquares[squarePos[j].Item1, squarePos[j].Item2]=2;
                board.squares[squarePos[j].Item1, squarePos[j].Item2].GetComponentInParent<MeshRenderer>().material = board.materials[2];
                if(castingCondition.LOS){
                    if(!lineOfSight(board, position, squarePos[j])){
                        board.reachableSquares[squarePos[j].Item1, squarePos[j].Item2]=1;
                        board.squares[squarePos[j].Item1, squarePos[j].Item2].GetComponentInParent<MeshRenderer>().material = board.materials[1];
                    }
                }
            }
        }
    }

    public bool lineOfSight(Board board, (int, int) a, (int, int) b){
        bool los = true;
        (int, int) vect = Utils.getVector(a.Item1, a.Item2, b.Item1, b.Item2);
        if( vect.Item1 == 0 || vect.Item2 == 0){
            if (vect.Item1 == 0 && vect.Item2 == 0){
                return true;
            }
            else if(vect.Item1 == 0){
                int way = vect.Item2 / Math.Abs(vect.Item2);
                for(int z = a.Item2 + way; z != b.Item2; z += way){
                    if(!board.squares[a.Item1, z].isEmpty()) los = false;
                }
            }else if(vect.Item2 == 0){
                int way = vect.Item1 / Math.Abs(vect.Item1);
                for(int x = a.Item1 + way; x != b.Item1; x += way){
                    if(!board.squares[x, a.Item2].isEmpty()) los = false;
                }
            }else{
                Debug.Log("Error LOS fc");
            }
        } else {
            Fraction coef = new Fraction(vect.Item2, vect.Item1);
            Fraction intecept = a.Item2 - a.Item1*coef;
            //Debug.Log(vect.Item1 + " : " + vect.Item2);
            //Debug.Log("Pos a: (" + a.Item1 + "," + a.Item2 + ") Pos b: (" + b.Item1 + "," + b.Item2 + ") => Coef: " + coef.ToString() + " Intercept: " + intecept.ToString());
            (int, int) orientation = (0,0);
            (int, int) over = (0,0);
            (int, int) under = (0,0);

            if(vect.Item1 > 0 && vect.Item2 > 0){
                orientation = (1, 1);
                over = (0, 1);
                under = (1, 0);
            }else if(vect.Item1 > 0 && vect.Item2 < 0){
                orientation = (1, -1);
                over = (1, 0);
                under = (0, -1);
            }else if(vect.Item1 < 0 && vect.Item2 > 0){
                orientation = (-1, 1);
                over = (0, 1);
                under = (-1, 0);
            }else if(vect.Item1 < 0 && vect.Item2 < 0){
                orientation = (-1, -1);
                over = (-1, 0);
                under = (0, -1);
            }
            else{
                Debug.Log("Error call LOS algorithm for diagonal trajectory on a line trajectory!");
            }
            Fraction offset = new Fraction( ((int) Math.Round(board.getSquareSize())), 2 ); // TODO: calculate this once and store atribute in class
            (Fraction, Fraction) corner = (a.Item1 + (orientation.Item1 * offset), a.Item2 + (orientation.Item2 * offset));
            (int, int) currentPos = (a.Item1, a.Item2);
            Fraction z;
            //Debug.Log("Pos A: (" + a.Item1 + "," + a.Item2 + ")");
            //Debug.Log("Pos B: (" + b.Item1 + "," + b.Item2 + ")");
            //Debug.Log("coef = " + coef.ToString());

            while(Utils.range(currentPos.Item1, currentPos.Item2, b.Item1, b.Item2) > 1 && los){
                z = (coef * corner.Item1) + intecept;
                //Debug.Log("corner = " + corner.ToString());
                //Debug.Log("Z = " + z.ToString());
                //Debug.Log("Zdouble = " + z.toDouble() + " et cornerZdouble = " + corner.Item2.toDouble());
                if(z.toDouble() > corner.Item2.toDouble()){//Curve pass over the corner
                    corner = (over.Item1 + corner.Item1, over.Item2 + corner.Item2);
                    currentPos = (over.Item1 + currentPos.Item1, over.Item2 + currentPos.Item2);
                }else if(z.toDouble() < corner.Item2.toDouble()){//Curve pass under the corner
                    //Debug.Log("ALO");
                    corner = (under.Item1 + corner.Item1, under.Item2 + corner.Item2);
                    currentPos = (under.Item1 + currentPos.Item1, under.Item2 + currentPos.Item2);
                }else{ // corner = z
                    //Debug.Log("ALUILE");
                    corner = (orientation.Item1 + corner.Item1, orientation.Item2 + corner.Item2);
                    currentPos = (orientation.Item1 + currentPos.Item1, orientation.Item2 + currentPos.Item2);
                }
                //Debug.Log("currentPos: (" + currentPos.Item1 + "," + currentPos.Item2 + ")");
                if(!board.squares[currentPos.Item1, currentPos.Item2].isEmpty() && Utils.range(currentPos.Item1, currentPos.Item2, b.Item1, b.Item2) !=0) los = false;
            }
        }
        return los;
    }
}
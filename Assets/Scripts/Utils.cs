using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Utils
{
    public static int range(int x, int z, int x2, int z2){
        return Math.Abs(x2 - x) + Math.Abs(z2 - z);
    }

    public static List<(int, int)> getSquaresAtRange(int range, (int, int) position, Board board){
        List<(int, int)> result = new List<(int, int)>();
        if(range !=0){
            for(int i=0; i<range; i++){
                if(board.doesSquareExist(position.Item1 + i, position.Item2 + (range - i))) result.Add( (position.Item1 + i, position.Item2 + (range - i)) );
            }
            for(int i=0; i<range; i++){
                if(board.doesSquareExist(position.Item2 + (range - i), position.Item2 - i)) result.Add( (position.Item1 + (range - i), position.Item2 - i) );
            }
            for(int i=0; i<range; i++){
                if(board.doesSquareExist(position.Item1 - i, position.Item2 - (range - i))) result.Add( (position.Item1 - i, position.Item2 - (range - i)) );
            }
            for(int i=0; i<range; i++){
                if(board.doesSquareExist(position.Item1 - (range - i), position.Item2 + i)) result.Add( (position.Item1 - (range - i), position.Item2 + i) );
            }
        } else{
            result.Add(position);
        }
        return result;
    }
}

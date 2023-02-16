using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Utils
{
    public static int range(int x, int z, int x2, int z2){
        return Math.Abs(x2 - x) + Math.Abs(z2 - z);
    }
}

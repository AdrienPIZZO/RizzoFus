using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chosen : MonoBehaviour
{
    public int currentX{set;get;}
    public int currentZ{set;get;}

    public void SetPosition(int x, int z)
    {
        currentX = x;
        currentZ = z;
    }
}

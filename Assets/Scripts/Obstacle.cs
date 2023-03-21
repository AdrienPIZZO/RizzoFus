using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : Entity
{
    public static List<Obstacle> Instances = new List<Obstacle>();
    private void Awake()
    {
        Instances.Add(this);
        Debug.Log("Awake of Obstacles");
    }
}

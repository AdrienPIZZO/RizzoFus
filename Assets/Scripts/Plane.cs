using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    public static Plane Instance = null;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {

    }
}
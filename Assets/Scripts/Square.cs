using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Square : NetworkBehaviour
{

    public Board board;
    public NetworkVariable<int> x = new NetworkVariable<int>(-1);
    public NetworkVariable<int> z = new NetworkVariable<int>(-1);
    public Entity entity;
    public static List<Square> Instances = new List<Square>();

    private void Start()
    {
        Instances.Add(this);
    }
    private void Update()
    {

    }
    public void init(Board board, int x, int z){
        this.board = board;
        this.x.Value = x;
        this.z.Value = z;
        entity = null;
    }
    public bool isEmpty(){
        //Debug.Log(elements[x,z]);
        return entity == null;   //Check if there is no object on the Square we are trying to move on
    }
    public Entity getEntity(){
        //Debug.Log(elements[x,z]);
        return entity;
    }
}
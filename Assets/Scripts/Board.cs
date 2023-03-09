using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private float tileSize /*= 1.0f*/;
    private float tileOffset /*= 0.5f*/;
    private int nbTiles /* = 8*/;
    //private Entity[,] elements;/*{set;get;}*/
    public Square[,] squares;/*{set;get;}*/
    public Board(float tileSize, float tileOffset, int nbTiles){
        this.tileSize = tileSize;
        this.tileOffset = tileOffset;
        this.nbTiles = nbTiles;
        squares =  new Square[nbTiles, nbTiles];
        for(int x=0; x<nbTiles; x++){
            for (int z=0; z<nbTiles; z++){
                squares[x,z]=new Square(this, x, z);
            }
        }
    }
    public int getNbTiles(){
        return nbTiles;
    }
    public float getTileSize(){
        return tileSize;
    }
    public float getTileOffset(){
        return tileOffset;
    }
    /*
    public void initElements(){
        squares =  new Square[nbTiles, nbTiles];
    }
    */
    public void setElement(int x, int z, Entity e){
        this.squares[x, z].occupant = e;
    }
    public Square[,] getElements(){
        return squares;
    }
    public Vector3 GetTileCenter(int x, int z){
        Vector3 origin = Vector3.zero;
        origin.x += (tileSize * x) + tileOffset;
        origin.z += (tileSize * z) + tileOffset;
        return origin;
    }
    public bool IsTileAvailable(int x, int z){
        //Debug.Log(elements[x,z]);
        return x >= 0 && z >= 0 && x < nbTiles && z < nbTiles && squares[x, z].occupant == null;   //Check if there is no object on the tile we are trying to move on
    }
    public Entity TileContent(int x, int z){
        //Debug.Log(elements[x,z]);
        if ( x >= 0 && z >= 0 && x < nbTiles && z < nbTiles )
        {
             return squares[x, z].occupant;
        }
           
        return null;
    }
    public Node PathFinding(Node current, int x, int z, int nbMP, List<Node> leaves){
        //Debug.Log(current.x + " : " + current.z);
        if (current.x == x && current.z == z)
        {
            //Debug.Log("found.");
            return current;
        }
        if(nbMP <= 0)
        {
            leaves.Add(current);
            return null;
        }
        else
        {
            Node node;
            Node res1 = null;
            Node res2 = null;
            Node res3 = null;
            Node res4 = null;
            current.childrens = new List<Node>();

            if (current.x + 1 < nbTiles && IsTileAvailable(current.x+1, current.z))
            {
                node = new Node(current.x + 1, current.z);
                node.parent = current;
                current.childrens.Add(node);
                res1 = PathFinding(node, x, z, nbMP - 1, leaves);
                if (res1 != null) return res1;
            }
            if (current.z + 1 < nbTiles && IsTileAvailable(current.x, current.z+1))
            {
                node = new Node(current.x, current.z + 1);
                node.parent = current;
                current.childrens.Add(node);
                res2 = PathFinding(node, x, z, nbMP - 1, leaves);
                if (res2 != null) return res2;
            }
            if (current.x - 1 >= 0 && IsTileAvailable(current.x-1, current.z))
            {
                node = new Node(current.x - 1, current.z);
                node.parent = current;
                current.childrens.Add(node);
                res3 = PathFinding(node, x, z, nbMP - 1, leaves);
                if (res3 != null) return res3;
            } 
            if (current.z - 1 >= 0 && IsTileAvailable(current.x, current.z-1))
            {
                node = new Node(current.x, current.z - 1);
                node.parent = current;
                current.childrens.Add(node);
                res4 = PathFinding(node, x, z, nbMP - 1, leaves);
                if (res4 != null) return res4;
            }
            return null;
        }
    }
}
public class Node
{
    public int x;
    public int z;
    public Node parent = null;
    public List<Node> childrens = null;
    public Node(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}
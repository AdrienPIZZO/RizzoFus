using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private float SquareSize /*= 1.0f*/;
    private float SquareOffset /*= 0.5f*/;
    private int nbSquares /* = 8*/;
    //private Entity[,] elements;/*{set;get;}*/
    public Square[,] squares;/*{set;get;}*/
    public GameObject[,] squaresGO;/*{set;get;}*/ //Duplicate used for swaping meshRender material
    public int[,] reachableSquares;
    public bool isASpellSelected = false;
    public List<GameObject> prefabs;
    public List<Material> materials;
    Vector3 offset;

    private void Start()
    {

    }
    private void Update()
    {

    }

    public void init(float SquareSize, float SquareOffset, int nbSquares, Vector3 offset){
        this.SquareSize = SquareSize;
        this.SquareOffset = SquareOffset;
        this.nbSquares = nbSquares;
        this.offset=offset;
        squares =  new Square[nbSquares, nbSquares];
        squaresGO =  new GameObject[nbSquares, nbSquares];
        reachableSquares =  new int[nbSquares, nbSquares];
        for(int x=0; x<nbSquares; x++){
            for (int z=0; z<nbSquares; z++){
                GameObject go = Instantiate(prefabs[0],
                GetSquareCenter(x, z) + Vector3.up * prefabs[0].GetComponent<Renderer>().bounds.size.y / 2 + offset, Quaternion.identity) as GameObject;
                go.transform.SetParent(transform);
                Square square = go.GetComponent<Square>();
                squaresGO[x,z]= go;
                squares[x,z]= square;
                square.init(this, x, z);
                //go.transform.position = GetSquareCenter(squares[x, z]) + Vector3.up * prefabs[0].GetComponent<Renderer>().bounds.size.y / 2 + offset;
            }
        }
    }

//Verification of all castingCondition of the spell to diplay the targetables squares
    public void updateReachableSquare(Spell s, (int, int) position){
        resetReachableSquares();
        isASpellSelected = true;
        //int squareSatus = 0;// 0 unreachable, 1 in range but no LOS, 2 everything is good -> cast spell ok 
        s.computeReachableSquares(this, position);
    }
    public void resetReachableSquares(){
        isASpellSelected = false;
        for(int x = 0; x < reachableSquares.GetLength(0); x++){
            for(int z = 0; z < reachableSquares.GetLength(1); z++){
                reachableSquares[x,z]=0;
                squaresGO[x,z].GetComponentInParent<MeshRenderer>().material = materials[0];
            }
        }
    }
    public int getNbSquares(){
        return nbSquares;
    }
    public float getSquareSize(){
        return SquareSize;
    }
    public bool doesSquareExist(int x, int z){
        return x >= 0 && z >= 0 && x < nbSquares && z < nbSquares;
    }
    public float getSquareOffset(){
        return SquareOffset;
    }
    public Vector3 GetSquareCenter(int x, int z){
        Vector3 origin = Vector3.zero;
        origin.x += (SquareSize * x) + SquareOffset;
        origin.z += (SquareSize * z) + SquareOffset;
        return origin;
    }
    public Vector3 GetSquareCenter(Square square){
        Vector3 origin = Vector3.zero;
        origin.x += (SquareSize * square.x) + SquareOffset;
        origin.z += (SquareSize * square.z) + SquareOffset;
        return origin;
    }
    public bool IsSquareAvailable(int x, int z){
        bool flag = doesSquareExist(x,z);
        if(flag) {
            flag = squares[x, z].isEmpty();
        }
        return flag;
    }
    public void setEntityAtPos(int x, int z, Entity e){
        this.squares[x, z].entity = e;
    }
    public Square[,] GetSquares(){
        return squares;
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

            if (current.x + 1 < nbSquares && IsSquareAvailable(current.x+1, current.z))
            {
                node = new Node(current.x + 1, current.z);
                node.parent = current;
                current.childrens.Add(node);
                res1 = PathFinding(node, x, z, nbMP - 1, leaves);
                if (res1 != null) return res1;
            }
            if (current.z + 1 < nbSquares && IsSquareAvailable(current.x, current.z+1))
            {
                node = new Node(current.x, current.z + 1);
                node.parent = current;
                current.childrens.Add(node);
                res2 = PathFinding(node, x, z, nbMP - 1, leaves);
                if (res2 != null) return res2;
            }
            if (current.x - 1 >= 0 && IsSquareAvailable(current.x-1, current.z))
            {
                node = new Node(current.x - 1, current.z);
                node.parent = current;
                current.childrens.Add(node);
                res3 = PathFinding(node, x, z, nbMP - 1, leaves);
                if (res3 != null) return res3;
            } 
            if (current.z - 1 >= 0 && IsSquareAvailable(current.x, current.z-1))
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
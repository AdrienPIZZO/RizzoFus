using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Board : NetworkBehaviour
{
    private float SquareSize /*= 1.0f*/;
    private float SquareOffset /*= 0.5f*/;
    //private int nbSquaresNetwork.Value /* = 8*/;
    //private Entity[,] elements;/*{set;get;}*/
    public Square[,] squares;/*{set;get;}*/
    //public GameObject[,] squaresGO;/*{set;get;}*/ //Duplicate used for swaping meshRender material
    public int[,] reachableSquares;
    public bool isASpellSelected = false;
    public List<GameObject> prefabs;
    public List<Material> materials;
    //Vector3 offset;
    public NetworkVariable<int> nbPlayers = new NetworkVariable<int>(0);
    public NetworkVariable<int> nbObstacles = new NetworkVariable<int>(0);
    public NetworkVariable<int> nbSquaresNetwork = new NetworkVariable<int>(0);

    public static Board Instance;

    private void Awake()
    {
        Instance = this;
        //Debug.Log("Awake of Board");
    }

    private void Start()
    {

    }
    private void Update()
    {

    }

    public override void OnNetworkSpawn()
    {
        //squaresGO =  new GameObject[nbSquaresNetwork.Value, nbSquaresNetwork.Value];
        reachableSquares =  new int[nbSquaresNetwork.Value, nbSquaresNetwork.Value];
    }

    public void DrawChosens(){
        //TODO: Do better for redraw board model
        for(int x = 0; x < GetSquares().GetLength(0); x++){
            for(int z = 0; z < GetSquares().GetLength(1); z++){
                if(!squares[x, z].isEmpty()){
                    //TODO Polymorphism with method from entity such that each subclass of entity modify transform position Y of their prefab
                    squares[x, z].getEntity().transform.position = GetSquareCenter(squares[x, z]) + Vector3.up * prefabs[2].GetComponent<Renderer>().bounds.size.y / 2 + transform.position;
                }
            }
        }
    }

    public void init(float SquareSize, float SquareOffset, int nbSquares){
        this.SquareSize = SquareSize;
        this.SquareOffset = SquareOffset;
        this.nbSquaresNetwork.Value = nbSquares;
        GameObject planeGO = Instantiate(prefabs[1], transform.position + new Vector3(getNbSquares()*getSquareSize()/2, 0, getNbSquares()*getSquareSize()/2),
        Quaternion.identity) as GameObject;//Plane GO
        planeGO.GetComponent<NetworkObject>().Spawn();
        planeGO.transform.SetParent(transform);
        planeGO.transform.localScale = new Vector3(nbSquaresNetwork.Value * prefabs[0].transform.localScale.x, 1, nbSquaresNetwork.Value * prefabs[0].transform.localScale.z);
        squares =  new Square[nbSquaresNetwork.Value, nbSquaresNetwork.Value];
        //squaresGO =  new GameObject[nbSquaresNetwork.Value, nbSquaresNetwork.Value];
        reachableSquares =  new int[nbSquaresNetwork.Value, nbSquaresNetwork.Value];
        for(int x=0; x<nbSquaresNetwork.Value; x++){
            for (int z=0; z<nbSquaresNetwork.Value; z++){
                GameObject go = Instantiate(prefabs[0], transform.position + GetSquareCenter(x, z), Quaternion.identity) as GameObject;
                go.GetComponent<NetworkObject>().Spawn();
                go.transform.SetParent(transform);
                Square square = go.GetComponent<Square>();
                //squaresGO[x,z]= go;
                squares[x,z]= square;
                squares[x,z].GetComponent<MeshRenderer>().material = materials[getIndexPrefabSquare(x,z)];
                square.init(this, x, z);
                //go.GetComponent<NetworkObject>().Spawn();
            }
        }
        SpawnAllObstacles();
    }

    public int getIndexPrefabSquare(int x, int z){
        return ((x + z) % 2) == 0 ? 3 : 4; 
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
                squares[x,z].GetComponent<MeshRenderer>().material = materials[getIndexPrefabSquare(x, z)];
            }
        }
    }
    public int getNbSquares(){
        return nbSquaresNetwork.Value;
    }
    public float getSquareSize(){
        return SquareSize;
    }
    public bool doesSquareExist(int x, int z){
        return x >= 0 && z >= 0 && x < nbSquaresNetwork.Value && z < nbSquaresNetwork.Value;
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
        origin.x += (SquareSize * square.x.Value) + SquareOffset;
        origin.z += (SquareSize * square.z.Value) + SquareOffset;
        return origin;
    }
    public bool IsSquareAvailable(int x, int z){
        return doesSquareExist(x,z) && squares[x, z].isEmpty();
    }
    public void setEntityAtPos(int x, int z, Entity e){
        //Debug.Log("setEntityAtPos: (" + x + ";" + z + ")");
        this.squares[x, z].entity = e;
    }
    public Square[,] GetSquares(){
        return squares;
    }

    private Chosen SpawnChosen(int indexPrefab, int x, int z)
    {
        //Vector3 offset = transform.position;
        GameObject go = Instantiate(prefabs[indexPrefab], transform.position + GetSquareCenter(x, z) + Vector3.up * prefabs[indexPrefab].GetComponent<Renderer>().bounds.size.y / 2,
        Quaternion.identity) as GameObject;
        go.GetComponent<NetworkObject>().Spawn();
        go.transform.SetParent(transform);
        Chosen chosen = go.GetComponent<Chosen>();
        setEntityAtPos(x, z, chosen);
        chosen.setBoard(this);
        chosen.initPos(x, z);
        //go.GetComponent<NetworkObject>().Spawn();
        return chosen;
    }

    private void SpawnObstacle(int indexPrefab, int x, int z)
    {
        //Vector3 offset = transform.position;
        GameObject go = Instantiate(prefabs[indexPrefab], transform.position + GetSquareCenter(x, z) + Vector3.up * prefabs[indexPrefab].GetComponent<Renderer>().bounds.size.y / 2,
        Quaternion.identity) as GameObject;
        go.GetComponent<NetworkObject>().Spawn();
        go.transform.SetParent(transform);
        Obstacle obstacle = go.GetComponent<Obstacle>();
        //Debug.Log("board: " + this==null);
        //Debug.Log("obstacle: " + obstacle==null);
        setEntityAtPos(x, z, obstacle);
        obstacle.setBoard(this);
        obstacle.initPos(x, z);
        //go.GetComponent<NetworkObject>().Spawn();
    }

    public List<Chosen> SpawnAllChosen()
    {
        List<Chosen> players = new List<Chosen>();
        players.Add(SpawnChosen(2, 7, 7));
        players.Add(SpawnChosen(2, 0, 0));
        nbPlayers.Value=2;
        return players;
    }

    private void SpawnAllObstacles()
    {
        SpawnObstacle(3, 2, 2);
        SpawnObstacle(3, 3, 0);
        SpawnObstacle(3, 2, 0);
        SpawnObstacle(3, 1, 0);
        SpawnObstacle(3, 2, 1);
        SpawnObstacle(3, 5, 5);
        nbObstacles.Value=6;
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

            if (current.x + 1 < nbSquaresNetwork.Value && IsSquareAvailable(current.x+1, current.z))
            {
                node = new Node(current.x + 1, current.z);
                node.parent = current;
                current.childrens.Add(node);
                res1 = PathFinding(node, x, z, nbMP - 1, leaves);
                if (res1 != null) return res1;
            }
            if (current.z + 1 < nbSquaresNetwork.Value && IsSquareAvailable(current.x, current.z+1))
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
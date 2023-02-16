using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private int selectionX = -1;
    private int selectionY = -1;
    public int playerTurn = 0;
    public int spellSelected = -1;
    public GameObject Canvas;
    public List<GameObject> prefabs; // Filled by Unity with prefabs folder content
    public List<Chosen> players;
    private Board board = new Board(1.0f, 0.5f, 8);
    private Node root;
    private List<Node> leaves = new List<Node>();
    //private List<Spell> spells = new List<Spell>();
    public Dictionary<int, Spell> spells = new Dictionary<int, Spell>();

    private void Start()
    {
        //create all spells in the game
        spells.Add(0, new Spell("Electric blade", 10, 10, 0));
        spells.Add(1, new Spell("Shuriken", 25, 0, 0));
        spells.Add(2, new Spell("Fire ball", 5, 30, 2));

        SpawnAllChosen();

        //Affect spell to chosen
        foreach(Chosen p in players){
            foreach(KeyValuePair<int, Spell> s in spells){
                p.addSpell(s);
            }
        }

        HUDManager hm = Canvas.GetComponent<HUDManager>();
        hm.updateSpellButton();

        SpawnAllObstacles();
    }

    private void Update()
    {
        UpdateSelection();
        DrawBoard();

        if(Input.GetMouseButtonDown(0) && (selectionX != -1 || selectionY != -1))
        {
            Entity e = board.TileContent(selectionX, selectionY);
            if (spellSelected!=-1)
            {
                Debug.Log(spells[spellSelected].getName());
                players[playerTurn].useSpell(e, spells[spellSelected]);
                spellSelected=-1;
            } else if (board.IsTileAvailable(selectionX, selectionY)){
                //Debug.Log("clic ok");
                ChosenMove(selectionX, selectionY);
            }
        }
    }
    private void DrawBoard(){
        Vector3 widthLine = Vector3.right * board.getTileSize() * board.getNbTiles();
        Vector3 heigthLine = Vector3.forward * board.getTileSize() * board.getNbTiles();
    
        for(int i=0; i<=board.getNbTiles(); i++){
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            for(int j=0; j<=board.getNbTiles(); j++){
                start = Vector3.right * i;
                Debug.DrawLine(start, start + heigthLine);
            }
        }

        // Draw selection
        if(selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * selectionX,
            Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
        }
    }

    private void ChosenMove(int x, int z)
    {
        int distance = Utils.range(players[playerTurn].currentX, players[playerTurn].currentZ, x, z);
        //Target tile too far from the chosen with his MP
        if (distance > players[playerTurn].MP)
        {
            Debug.Log("You can't move that far.");
            return;
        }

        //PATHFINDING
        // We try to find the quickest path from current x/z to targeted x/z
        root = new Node(players[playerTurn].currentX, players[playerTurn].currentZ);
        Node leaf = board.PathFinding(root, x, z, distance, leaves);  //We try first with number of MP = Range then we will increase this number if we didn't find a way

        int tmpMP = players[playerTurn].MP;
        tmpMP -= distance;

        int leavesIndex = 0;
        while (leaf == null && tmpMP > 0) // if we aren't on target and we still have MP then we increase MP
        {
            //Debug.Log("mp : " + tmpMP);
            int tmpIndex = leaves.Count;
            for (int i = leavesIndex; i < tmpIndex; i++)
            {
                leaf = board.PathFinding(leaves[i], x, z, 1, leaves);
                if (leaf != null)
                {
                    Debug.Log("mp : " + tmpMP);
                    break;
                }
            }
            tmpMP--;
            leavesIndex = tmpIndex;
        }

        if (leaf == null) //we didn't find a way with all our MP
        {
            //Debug.Log("We didn't find a way to go there");
            leaves = new List<Node>(); //reset leaves AND MAYBE free all of the objects at this point
            return;
        }

        //Debug print path
        while (leaf != null)
        {
            //Debug.Log(leaf.x + " : " + leaf.z);
            leaf = leaf.parent;
        }

        leaves = new List<Node>(); //reset leaves AND MAYBE free all of the objects at this point
        //Debug.Log("MP : " + tmpMP);
        //END PATHFINDING

        players[playerTurn].MP = tmpMP; //MP left to the chosen after moving

        //Moving entities
        board.setElement(x, z, players[playerTurn]);
        board.setElement(players[playerTurn].currentX, players[playerTurn].currentZ, null);
        players[playerTurn].SetPosition(x, z);
        board.TileContent(x, z).transform.position = board.GetTileCenter(x, z)  + Vector3.up * prefabs[0].GetComponent<Renderer>().bounds.size.y / 2;
    }

    private void UpdateSelection()
    {
        if(!Camera.main)
            return;
        
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Plane"))){
            //Debug.Log(hit.point);
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }
 
    private void SpawnChosen(int indexPrefab, int x, int z)
    {
        GameObject go = Instantiate(prefabs[indexPrefab], board.GetTileCenter(x, z) + Vector3.up * prefabs[indexPrefab].GetComponent<Renderer>().bounds.size.y / 2, Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        board.setElement(x, z, go.GetComponent<Chosen>());
        go.GetComponent<Chosen>().SetPosition(x, z);
        players.Add(go.GetComponent<Chosen>());
    }

    private void SpawnObstacle(int indexPrefab, int x, int z)
    {
        GameObject go = Instantiate(prefabs[indexPrefab], board.GetTileCenter(x, z) + Vector3.up * prefabs[indexPrefab].GetComponent<Renderer>().bounds.size.y / 2, Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        board.setElement(x, z, go.GetComponent<Obstacles>());
        go.GetComponent<Obstacles>().SetPosition(x, z);
    }

    private void SpawnAllChosen()
    {
        players = new List<Chosen>();
        board.initElements();
        SpawnChosen(0, 0, 0);
        SpawnChosen(0, 7, 7);
    }

    private void SpawnAllObstacles()
    {
        SpawnObstacle(1, 2, 2);
        SpawnObstacle(1, 3, 0);
        SpawnObstacle(1, 2, 0);
        SpawnObstacle(1, 1, 0);
        SpawnObstacle(1, 2, 1);
        SpawnObstacle(1, 5, 5);
    }

    public void EndTurn()
    {
        players[playerTurn].MPReset();
        playerTurn = (playerTurn + 1) % players.Count;
    }

    public void SetAttackSelected(bool b)
    {
     //   attackSelected = b;
    }
}
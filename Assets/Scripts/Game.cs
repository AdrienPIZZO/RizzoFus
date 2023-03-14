using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private int selectionX = -1;
    private int selectionZ = -1;
    public int playerTurn = 0;
    public Spell spellSelected = null;
    public GameObject Canvas;
    public HUDManager hm;
    public List<GameObject> prefabs;
    public List<Chosen> players;
    public Board board; 
    private Node root;
    private List<Node> leaves = new List<Node>();
    public Dictionary<int, Spell> spells = new Dictionary<int, Spell>();
    Vector3 offset; 

    public Square lastSquareSelected = null;

    private void Start()
    {
        offset = transform.position;
        //board = new Board(1.0f, 0.5f, 16, offset);

        board.init(1.0f, 0.5f, 16, offset);
        SpawnAllChosen();

        int id = 0;
        //create all spells in the game
        Spell electricBlade = new Spell("Electric blade", 20, new CastingCondition((1,1), true, true));
        electricBlade.effects.Add(new PhysicalDamage(board, 5));
        spells.Add(id++, electricBlade);

        Spell fireBall = new Spell("Fire ball", 30, new CastingCondition((2,6), false, true));
        fireBall.effects.Add(new PhysicalDamage(board, 20));
        spells.Add(id++, fireBall);

        Spell frozenGasp = new Spell("Frozen Gasp", 30, new CastingCondition((1,4), false, true));
        frozenGasp.effects.Add(new PhysicalDamage(board, 20));
        frozenGasp.effects.Add(new MPbuff(2, false, -1));
        spells.Add(id++, frozenGasp);

        Spell shuriken = new Spell("Shuriken", 30, new CastingCondition((1,8), true, true));
        shuriken.effects.Add(new PhysicalDamage(board, 10));
        spells.Add(id++, shuriken);

        Spell celerity = new Spell("Celerity", 10, new CastingCondition((0,0), true, true));
        celerity.effects.Add(new MPbuff(1, true, 3));
        spells.Add(id++, celerity);

        Spell jadePalm = new Spell("Jade palm", 40, new CastingCondition((1,3), true, true));
        jadePalm.effects.Add(new PhysicalDamage(board, 5));
        jadePalm.effects.Add(new MoveTarget(board, 2));
        spells.Add(id++, jadePalm);

        //Affect spell to chosen
        players[0].addSpell(new KeyValuePair<int, Spell> (0, spells[0]));
        players[0].addSpell(new KeyValuePair<int, Spell> (1, spells[1]));
        players[0].addSpell(new KeyValuePair<int, Spell> (2, spells[2]));

        players[1].addSpell(new KeyValuePair<int, Spell> (3, spells[3]));
        players[1].addSpell(new KeyValuePair<int, Spell> (4, spells[4]));
        players[1].addSpell(new KeyValuePair<int, Spell> (5, spells[5]));

        SpawnAllObstacles();

        hm = Canvas.GetComponent<HUDManager>();
        hm.initHUD();


    }

    private void Update()
    {
        UpdateSelection();
        DrawBoard();

        // onClick use spell or move chosen
        if(Input.GetMouseButtonDown(0) && (selectionX != -1 || selectionZ != -1))
        {
            lastSquareSelected = board.squares[selectionX, selectionZ];
            if (spellSelected!=null)
            {
                Debug.Log(spellSelected.getName());
                if(board.reachableSquares[selectionX, selectionZ]==2){
                    players[playerTurn].useSpell(lastSquareSelected, spellSelected);
                    hm.updateHUDInfo();
                } else{
                    Debug.Log("Target out of reach!");
                }
            } else if (board.IsSquareAvailable(selectionX, selectionZ)){
                ChosenMove(selectionX, selectionZ);
                hm.updateHUDMP();
            }
            spellSelected=null; //Unselect spell on click
            board.resetReachableSquares();
        }
    }
    private void DrawBoard(){
        Vector3 widthLine = Vector3.right * board.getSquareSize() * board.getNbSquares();
        Vector3 heigthLine = Vector3.forward * board.getSquareSize() * board.getNbSquares();
        
        for(int i=0; i<=board.getNbSquares(); i++){
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start + offset, start + widthLine + offset);
            for(int j=0; j<=board.getNbSquares(); j++){
                start = Vector3.right * i;
                Debug.DrawLine(start + offset, start + heigthLine + offset);
            }
        }

        // Draw selection
        if(selectionX >= 0 && selectionZ >= 0)
        {
            Debug.DrawLine(Vector3.forward * selectionZ + Vector3.right * selectionX + offset,
            Vector3.forward * (selectionZ + 1) + Vector3.right * (selectionX + 1) + offset);
        }

        //TODO: Do better for redraw board model
        for(int x = 0; x < board.GetSquares().GetLength(0); x++){
            for(int z = 0; z < board.GetSquares().GetLength(1); z++){
                if(!board.squares[x, z].isEmpty()){
                    board.squares[x, z].getEntity().transform.position = board.GetSquareCenter(board.squares[x, z]) + Vector3.up * prefabs[0].GetComponent<Renderer>().bounds.size.y / 2 + offset;
                }
            }
        }
    }

    private void ChosenMove(int x, int z)
    {
        int distance = Utils.range(players[playerTurn].currentX, players[playerTurn].currentZ, x, z);
        //Target Square too far from the chosen with his MP
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
        board.setEntityAtPos(x, z, players[playerTurn]);
        board.setEntityAtPos(players[playerTurn].currentX, players[playerTurn].currentZ, null);
        players[playerTurn].SetPosition(x, z);
    }

    private void UpdateSelection()
    {
        if(!Camera.main)
            return;
        
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50.0f, LayerMask.GetMask("Plane"))){
            //Debug.Log(hit.point);
            selectionX = (int)hit.point.x;
            selectionZ = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionZ = -1;
        }
    }
 
    private void SpawnChosen(int indexPrefab, int x, int z)
    {
        GameObject go = Instantiate(prefabs[indexPrefab], board.GetSquareCenter(x, z) + Vector3.up * prefabs[indexPrefab].GetComponent<Renderer>().bounds.size.y / 2 + offset,
        Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        Chosen chosen = go.GetComponent<Chosen>();
        chosen.setBoard(board);
        board.setEntityAtPos(x, z, chosen);
        chosen.SetPosition(x, z);
        players.Add(chosen);
    }

    private void SpawnObstacle(int indexPrefab, int x, int z)
    {
        GameObject go = Instantiate(prefabs[indexPrefab], board.GetSquareCenter(x, z) + Vector3.up * prefabs[indexPrefab].GetComponent<Renderer>().bounds.size.y / 2 + offset,
        Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        Obstacles obstacle = go.GetComponent<Obstacles>();
        obstacle.setBoard(board);
        board.setEntityAtPos(x, z, obstacle);
        obstacle.SetPosition(x, z);
    }

    private void SpawnAllChosen()
    {
        players = new List<Chosen>();
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
        players[playerTurn].passTurn();
        spellSelected = null;
        board.resetReachableSquares();
        playerTurn = (playerTurn + 1) % players.Count;
        hm.updateHUD();
        hm.updateHUDInfo();
        players[playerTurn].beginTurn();
    }

    public void updateSpellSelected(int id)
    {
        
        spellSelected = spells[id];
        board.updateReachableSquare(spellSelected, (players[playerTurn].currentX, players[playerTurn].currentZ));
    }
}
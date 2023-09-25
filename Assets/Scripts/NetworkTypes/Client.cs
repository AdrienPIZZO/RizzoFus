using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Client : Controler
{
    private int selectionX = -1;
    private int selectionZ = -1;
    public bool isASpellSelected = false;
    private bool init = false;
    public GameObject gameHUDPrefab;
    private HUDManager hm;

    // Start is called before the first frame update
    void Start()
    {

        //WARNIG if Instantiate is non blocking, it can start update without HUDManager singleton being set
        enabled = false;
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        hm = Instantiate(gameHUDPrefab, canvas.transform.position,
        Quaternion.identity, canvas.transform).GetComponent<HUDManager>();
        StartCoroutine(WaitInit(1));
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSelection();
        game.DrawGrid();
        if(Input.GetMouseButtonDown(0) && (selectionX != -1 || selectionZ != -1)){
            game.lastSquareSelected = game.board.squares[selectionX, selectionZ];
            if (game.spellSelected!=null)
            {
                Debug.Log("Spell selected: " + game.spellSelected.getName());
                game.SpellRequestServerRpc(game.spellSelectedID, selectionX, selectionZ);
                
            } else if (game.board.IsSquareAvailable(selectionX, selectionZ)){
                game.MoveRequestServerRpc(selectionX, selectionZ);
            }
            game.board.resetReachableSquares();
            game.spellSelected=null; //Unselect spell on click
        }
    }

    private IEnumerator WaitInit(float timeToWait)
    {
        while (!isInitialized())
        {
            yield return new WaitForSeconds(timeToWait);
        }
        enabled = true;
    }

    private bool isInitialized(){
        if(Game.Instance != null && Board.Instance != null && Plane.Instance != null 
        && Board.Instance.nbSquaresNetwork.Value != 0 && Board.Instance.nbPlayers.Value != 0 && Board.Instance.nbObstacles.Value != 0){

            if(Board.Instance.nbPlayers.Value == Chosen.Instances.Count && Board.Instance.nbObstacles.Value == Obstacle.Instances.Count &&
            Board.Instance.nbSquaresNetwork.Value*Board.Instance.nbSquaresNetwork.Value == Square.Instances.Count){
                bool areAllSquarePosInit = true;
                foreach(Square square in Square.Instances){
                    if(square.x.Value==-1 || square.z.Value==-1){
                        areAllSquarePosInit=false;
                        
                    }
                }
                foreach(Chosen chosen in Chosen.Instances){
                    if(chosen.x.Value==-1 || chosen.z.Value==-1 || chosen.chosenID.Value==-1){
                        areAllSquarePosInit=false;

                    }
                }
                foreach(Obstacle obstacle in Obstacle.Instances){
                    if(obstacle.x.Value==-1 || obstacle.z.Value==-1){
                        areAllSquarePosInit=false;

                    }
                }
                if(areAllSquarePosInit){
                    initialize();
                    //Debug.Log("INIT CLIENT SUCCESS!");
                    return true;
                }
            }
        }
        return false;
    }
    private void initialize(){
        game = Game.Instance;
        game.board = Board.Instance;
        game.board.squares = new Square[game.board.nbSquaresNetwork.Value, game.board.nbSquaresNetwork.Value];
        game.board.reachableSquares = new int[game.board.nbSquaresNetwork.Value, game.board.nbSquaresNetwork.Value];
        //int i =0;
        foreach(Square square in Square.Instances){
            game.board.squares[square.x.Value, square.z.Value] = square;
        }
        foreach(Chosen chosen in Chosen.Instances){
            game.chosens.Add(chosen.chosenID.Value, chosen);
            game.board.squares[chosen.x.Value, chosen.z.Value].entity = chosen;
        }
        foreach(Obstacle obstacle in Obstacle.Instances){
            game.board.squares[obstacle.x.Value, obstacle.z.Value].entity = obstacle;
        }

        //TODO: load from db or serialize from server
        int id = 0;
        //create all spells in the game 
        Spell electricBlade = new Spell("Electric blade", 20, new CastingCondition((1,1), true, true));
        electricBlade.effects.Add(new PhysicalDamage(game.board, 5));
        game.spells.Add(id++, electricBlade);

        Spell fireBall = new Spell("Fire ball", 30, new CastingCondition((2,6), false, true));
        fireBall.effects.Add(new PhysicalDamage(game.board, 20));
        game.spells.Add(id++, fireBall);

        Spell frozenGasp = new Spell("Frozen Gasp", 30, new CastingCondition((1,4), false, true));
        frozenGasp.effects.Add(new PhysicalDamage(game.board, 20));
        frozenGasp.effects.Add(new MPbuff(2, false, -1));
        game.spells.Add(id++, frozenGasp);

        Spell shuriken = new Spell("Shuriken", 30, new CastingCondition((1,8), true, true));
        shuriken.effects.Add(new PhysicalDamage(game.board, 10));
        game.spells.Add(id++, shuriken);

        Spell celerity = new Spell("Celerity", 10, new CastingCondition((0,0), true, true));
        celerity.effects.Add(new MPbuff(1, true, 3));
        game.spells.Add(id++, celerity);

        Spell jadePalm = new Spell("Jade palm", 40, new CastingCondition((1,3), true, true));
        jadePalm.effects.Add(new PhysicalDamage(game.board, 5));
        jadePalm.effects.Add(new MoveTarget(game.board, 2));
        game.spells.Add(id++, jadePalm);

        //Affect spell to chosen
        game.chosens[0].addSpell(new KeyValuePair<int, Spell> (0, game.spells[0]));
        game.chosens[0].addSpell(new KeyValuePair<int, Spell> (1, game.spells[1]));
        game.chosens[0].addSpell(new KeyValuePair<int, Spell> (2, game.spells[2]));

        game.chosens[1].addSpell(new KeyValuePair<int, Spell> (3, game.spells[3]));
        game.chosens[1].addSpell(new KeyValuePair<int, Spell> (4, game.spells[4]));
        game.chosens[1].addSpell(new KeyValuePair<int, Spell> (5, game.spells[5]));
        
        for(int x=0; x<game.board.nbSquaresNetwork.Value; x++){
            for (int z=0; z<game.board.nbSquaresNetwork.Value; z++){
                game.board.squares[x,z].GetComponent<MeshRenderer>().material = game.board.materials[game.board.getIndexPrefabSquare(x,z)];
            }
        }
  
        game.playerID = (int) NetworkManager.Singleton.LocalClientId - 1;
        Debug.Log("player ID = " + game.playerID);

        hm.initHUD(this);

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
    public void DrawGrid(){ //TODO: Do better for redraw board model
        Vector3 widthLine = Vector3.right * game.board.getSquareSize() * game.board.getNbSquares();
        Vector3 heigthLine = Vector3.forward * game.board.getSquareSize() * game.board.getNbSquares();
        
        for(int i=0; i<=game.board.getNbSquares(); i++){
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start + game.transform.position, start + widthLine + game.transform.position);
            for(int j=0; j<=game.board.getNbSquares(); j++){
                start = Vector3.right * i;
                Debug.DrawLine(start + game.transform.position, start + heigthLine + game.transform.position);
            }
        }
        // Draw selection
        if(selectionX >= 0 && selectionZ >= 0)
        {
            Debug.DrawLine(Vector3.forward * selectionZ + Vector3.right * selectionX + game.transform.position,
            Vector3.forward * (selectionZ + 1) + Vector3.right * (selectionX + 1) + game.transform.position);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Client : MonoBehaviour
{
    private int selectionX = -1;
    private int selectionZ = -1;
    public bool isASpellSelected = false;
    public Game game = null;
    private bool init = false;

    // Start is called before the first frame update
    void Start()
    {
        //hm = Canvas.GetComponent<HUDManager>();
        //hm.initHUD(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(!init){
            init = isInitialized();
        }else{
            UpdateSelection();
            game.DrawGrid();
            //game.board.DrawChosens();
            if(Input.GetMouseButtonDown(0) && (selectionX != -1 || selectionZ != -1)){
                game.lastSquareSelected = game.board.squares[selectionX, selectionZ];
                if (game.spellSelected!=null)
                {
                    Debug.Log(game.spellSelected.getName());
                    if(game.board.reachableSquares[selectionX, selectionZ]==2){
                        game.players[game.IDplayerTurn.Value].useSpell(game.lastSquareSelected, game.spellSelected);
                        game.hm.updateHUDInfo();
                    } else{
                        Debug.Log("Target out of reach!");
                    }
                } else if (game.board.IsSquareAvailable(selectionX, selectionZ)){
                    //game.ChosenMove(selectionX, selectionZ);
                    MoveServerRpc();

                    game.hm.updateHUDMP();
                }
                game.spellSelected=null; //Unselect spell on click
                game.board.resetReachableSquares();
            }
        }
    }

    private bool isInitialized(){
        if(Game.Instance != null && Board.Instance != null && Plane.Instance != null 
        && Board.Instance.nbSquaresNetwork.Value != 0 && Board.Instance.nbPlayers.Value != 0 && Board.Instance.nbObstacles.Value != 0){
            if(Board.Instance.nbPlayers.Value == Chosen.Instances.Count && Board.Instance.nbObstacles.Value == Obstacle.Instances.Count && Board.Instance.nbSquaresNetwork.Value*Board.Instance.nbSquaresNetwork.Value == Square.Instances.Count){
                    bool areAllSquarePosInit = true;
                    foreach(Square square in Square.Instances){
                        if(square.x.Value==-1 || square.z.Value==-1){
                            areAllSquarePosInit=false;
                        }
                    }
                    foreach(Chosen chosen in Chosen.Instances){
                        if(chosen.x.Value==-1 || chosen.z.Value==-1){
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
                        Debug.Log("INIT CLIENT SUCCESS");
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
        int i =0;
        foreach(Square square in Square.Instances){
            game.board.squares[square.x.Value, square.z.Value] = square;
        }
        foreach(Chosen chosen in Chosen.Instances){
            game.players.Add(chosen);
            game.board.squares[chosen.x.Value, chosen.z.Value].entity = chosen;
        }
        foreach(Obstacle obstacle in Obstacle.Instances){
            game.board.squares[obstacle.x.Value, obstacle.z.Value].entity = obstacle;
        }
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

    [ServerRpc]
    private void MoveServerRpc(){

    }
}

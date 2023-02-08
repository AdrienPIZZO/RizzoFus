using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public GameObject ButtonEndTurn;
    public GameObject ButtonAttack;
    public GameObject Board;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndTurnPressed()
    {
        Board.GetComponent<BoardManager>().EndTurn();
    }

        public void AttackPressed()
    {
        Board.GetComponent<BoardManager>().SetAttackSelected(true);
    }


}

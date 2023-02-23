using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public GameObject Board;
    private BoardManager bm;
    public List<GameObject> prefabs;

    //Declaration for updateSpellButton
    private GameObject go;
    private Button b;
    private TextMeshProUGUI tm; 

    private List<GameObject> spellButtons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initHUD(){
        bm = Board.GetComponent<BoardManager>();

        go = Instantiate(prefabs[0], new Vector3(1400, 25, 0), Quaternion.identity) as GameObject;
            go.transform.SetParent(transform);
            b = go.GetComponent<Button>();
            b.onClick.AddListener(EndTurnPressed);
            tm = b.GetComponentInChildren<TextMeshProUGUI>();
            tm.text = "Pass";
        

        int i = 0;
        foreach(KeyValuePair<int, Spell> s in bm.players[bm.playerTurn].spells){
            go = Instantiate(prefabs[0], new Vector3(i*200 + 100, 25, 0), Quaternion.identity) as GameObject;
            go.transform.SetParent(transform);
            b = go.GetComponent<Button>();
            b.onClick.AddListener(delegate{spellButtonHandler(s.Key);});
            tm = b.GetComponentInChildren<TextMeshProUGUI>();
            tm.text = s.Value.getName();
            i++;
            spellButtons.Add(go);
        }
        
    }

        public void updateHUD(){
        
        foreach(GameObject go in spellButtons){
            Destroy(go);
        }
        int i = 0;
        foreach(KeyValuePair<int, Spell> s in bm.players[bm.playerTurn].spells){
            go = Instantiate(prefabs[0], new Vector3(i*200 + 100, 25, 0), Quaternion.identity) as GameObject;
            go.transform.SetParent(transform);
            b = go.GetComponent<Button>();
            b.onClick.AddListener(delegate{spellButtonHandler(s.Key);});
            tm = b.GetComponentInChildren<TextMeshProUGUI>();
            tm.text = s.Value.getName();
            i++;
        }
        
    }

    public void EndTurnPressed()
    {
        bm.EndTurn();
    }

    public void spellButtonHandler(int id)
    {
        bm.spellSelected = id;
    }

}

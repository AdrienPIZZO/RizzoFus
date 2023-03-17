using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public GameObject gameGO;
    private Game game;
    public List<GameObject> prefabs;

    //Declaration for updateSpellButton
    private GameObject go;
    private Button b;
    private TextMeshProUGUI tm; 

    private List<GameObject> spellButtons = new List<GameObject>();

    private GameObject hpBar;
    private GameObject powerGaugeBar;
    private GameObject mpBar;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initHUD(Game game){

        Debug.Log(game.players[0]);
        Debug.Log(game.players[1]);

        hpBar = Instantiate(prefabs[1], new Vector3(100, 750, 0), Quaternion.identity) as GameObject;
        hpBar.transform.SetParent(transform);
        tm = hpBar.GetComponentInChildren<TextMeshProUGUI>();
        tm.text = game.players[game.playerTurn].HP.ToString() + " HP";

        mpBar = Instantiate(prefabs[1], new Vector3(100, 700, 0), Quaternion.identity) as GameObject;
        mpBar.transform.SetParent(transform);
        tm = mpBar.GetComponentInChildren<TextMeshProUGUI>();
        tm.text = game.players[game.playerTurn].MP.ToString() + " MP";

        powerGaugeBar = Instantiate(prefabs[1], new Vector3(150, 650, 0), Quaternion.identity) as GameObject;
        powerGaugeBar.transform.SetParent(transform);
        powerGaugeBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
        tm = powerGaugeBar.GetComponentInChildren<TextMeshProUGUI>();
        tm.text = game.players[game.playerTurn].powerGauge.ToString() + " POWER";

        game = gameGO.GetComponent<Game>();

        go = Instantiate(prefabs[0], new Vector3(1400, 25, 0), Quaternion.identity) as GameObject;
            go.transform.SetParent(transform);
            b = go.GetComponent<Button>();
            b.onClick.AddListener(EndTurnPressed);
            tm = b.GetComponentInChildren<TextMeshProUGUI>();
            tm.text = "Pass";
        

        int i = 0;
        foreach(KeyValuePair<int, Spell> s in game.players[game.playerTurn].spells){
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
        foreach(KeyValuePair<int, Spell> s in game.players[game.playerTurn].spells){
            go = Instantiate(prefabs[0], new Vector3(i*200 + 100, 25, 0), Quaternion.identity) as GameObject;
            go.transform.SetParent(transform);
            b = go.GetComponent<Button>();
            b.onClick.AddListener(delegate{spellButtonHandler(s.Key);});
            tm = b.GetComponentInChildren<TextMeshProUGUI>();
            tm.text = s.Value.getName();
            i++;
        }
    }

    public void updateHUDInfo(){//hpBar for now, see if we rename both updates
        tm = hpBar.GetComponentInChildren<TextMeshProUGUI>();
        tm.text = game.players[game.playerTurn].HP.ToString() + " HP";
        tm = powerGaugeBar.GetComponentInChildren<TextMeshProUGUI>();
        tm.text = game.players[game.playerTurn].powerGauge.ToString() + " POWER";
        updateHUDMP();
    }

    public void updateHUDMP(){
        tm = mpBar.GetComponentInChildren<TextMeshProUGUI>();
        tm.text = game.players[game.playerTurn].MP.ToString() + " MP";
    }

    public void EndTurnPressed()
    {
        game.EndTurn();
    }

    public void spellButtonHandler(int id)
    {
        game.updateSpellSelected(id);
    }

}
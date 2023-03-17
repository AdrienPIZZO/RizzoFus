using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    private Game game;
    public List<GameObject> prefabs;
    //Declaration for updateSpellButton
    //private GameObject go;
    //private Button b;
    //private TextMeshProUGUI tm; 
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
        this.game = game;
        hpBar = Instantiate(prefabs[1], new Vector3(100, 750, 0), Quaternion.identity) as GameObject;
        hpBar.transform.SetParent(transform);
        TextMeshProUGUI HPTextMesh = hpBar.GetComponentInChildren<TextMeshProUGUI>();
        HPTextMesh.text = game.players[game.playerTurn].HP.ToString() + " HP";

        mpBar = Instantiate(prefabs[1], new Vector3(100, 700, 0), Quaternion.identity) as GameObject;
        mpBar.transform.SetParent(transform);
        TextMeshProUGUI MPTextMesh = mpBar.GetComponentInChildren<TextMeshProUGUI>();
        MPTextMesh.text = game.players[game.playerTurn].MP.ToString() + " MP";

        powerGaugeBar = Instantiate(prefabs[1], new Vector3(150, 650, 0), Quaternion.identity) as GameObject;
        powerGaugeBar.transform.SetParent(transform);
        powerGaugeBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
        TextMeshProUGUI PowerGaugeTextMesh = powerGaugeBar.GetComponentInChildren<TextMeshProUGUI>();
        PowerGaugeTextMesh.text = game.players[game.playerTurn].powerGauge.ToString() + " POWER";

        GameObject passBtnGO = Instantiate(prefabs[0], new Vector3(1400, 25, 0), Quaternion.identity) as GameObject;
        passBtnGO.transform.SetParent(transform);
        Button passBtn = passBtnGO.GetComponent<Button>();
        passBtn.onClick.AddListener(EndTurnPressed);
        TextMeshProUGUI passTextMesh = passBtn.GetComponentInChildren<TextMeshProUGUI>();
        passTextMesh.text = "Pass";

        int i = 0;
        GameObject spellBtnGO;
        Button spellBtn;
        TextMeshProUGUI spellTextMesh;
        foreach(KeyValuePair<int, Spell> s in game.players[game.playerTurn].spells){
            spellBtnGO = Instantiate(prefabs[0], new Vector3(i*200 + 100, 25, 0), Quaternion.identity) as GameObject;
            spellBtnGO.transform.SetParent(transform);
            spellBtn = spellBtnGO.GetComponent<Button>();
            spellBtn.onClick.AddListener(delegate{spellButtonHandler(s.Key);});
            spellTextMesh = spellBtn.GetComponentInChildren<TextMeshProUGUI>();
            spellTextMesh.text = s.Value.getName();
            i++;
            spellButtons.Add(spellBtnGO);
        }
    }

    public void updateHUD(){
        foreach(GameObject go in spellButtons){
            Destroy(go);
        }
        int i = 0;
        GameObject spellBtnGO;
        Button spellBtn;
        TextMeshProUGUI spellTextMesh;
        foreach(KeyValuePair<int, Spell> s in game.players[game.playerTurn].spells){
            spellBtnGO = Instantiate(prefabs[0], new Vector3(i*200 + 100, 25, 0), Quaternion.identity) as GameObject;
            spellBtnGO.transform.SetParent(transform);
            spellBtn = spellBtnGO.GetComponent<Button>();
            spellBtn.onClick.AddListener(delegate{spellButtonHandler(s.Key);});
            spellTextMesh = spellBtn.GetComponentInChildren<TextMeshProUGUI>();
            spellTextMesh.text = s.Value.getName();
            i++;
        }
    }

    public void updateHUDInfo(){//hpBar for now, see if we rename both updates
        TextMeshProUGUI HPTextMesh = hpBar.GetComponentInChildren<TextMeshProUGUI>();
        HPTextMesh.text = game.players[game.playerTurn].HP.ToString() + " HP";
        TextMeshProUGUI powerGaugeTextMesh = powerGaugeBar.GetComponentInChildren<TextMeshProUGUI>();
        powerGaugeTextMesh.text = game.players[game.playerTurn].powerGauge.ToString() + " POWER";
        updateHUDMP();
    }

    public void updateHUDMP(){
        TextMeshProUGUI spellTextMesh;
        spellTextMesh = mpBar.GetComponentInChildren<TextMeshProUGUI>();
        spellTextMesh.text = game.players[game.playerTurn].MP.ToString() + " MP";
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

public class Chosen : Entity
{
    //STATS
    private const int MPMAX = 6;
    private const int HPMAX = 1000;
    private const int PWRGAUGEMAX = 200;
    private const int ARMORMAX = 50;// % based
    private const int STRENGTHMAX = 200;

    public NetworkVariable<int> MP = new NetworkVariable<int>(MPMAX);
    public NetworkVariable<int> HP = new NetworkVariable<int>(HPMAX);
    public NetworkVariable<int> powerGauge = new NetworkVariable<int>(PWRGAUGEMAX/2);
    public NetworkVariable<int> powerRegen = new NetworkVariable<int>(50);
    public NetworkVariable<int> armor = new NetworkVariable<int>(10);
    public NetworkVariable<int> strength = new NetworkVariable<int>(5);// % based

    public Dictionary<int, Spell> spells = new Dictionary<int, Spell>();
    public List<Buff> buffs = new List<Buff>();
    public static List<Chosen> Instances = new List<Chosen>();

    override public void OnNetworkSpawn(){
        MP.OnValueChanged = HUDManager.Singleton.onMPChange;
        HP.OnValueChanged = HUDManager.Singleton.onHPChange;
        powerGauge.OnValueChanged = HUDManager.Singleton.onPowerGaugeChange;
    }

    private void Awake()
    {
        Instances.Add(this);
        //Debug.Log("Awake of Chosen");
    }
    public void addSpell(KeyValuePair<int, Spell> s){
        spells.Add(s.Key, s.Value);
    }

    public void beginTurn(){
        foreach(Buff b in buffs.ToList()){
            if (b.nbTurnRemaining <= 0){
                buffs.Remove(b);
            } else {
                Debug.Log(MP);
                b.applyBuff();
                Debug.Log(MP);
            }
        }
    }
 
    public void passTurn()
    {
        MP.Value = MPMAX;
        powerGauge.Value = powerGauge.Value + powerRegen.Value <= PWRGAUGEMAX ? powerGauge.Value + powerRegen.Value : PWRGAUGEMAX;
    }

    private bool isDead()
    {
        return HP.Value<=0;
    }

    public override bool targeted(Chosen caster, Spell s)
    {
        s.applyAllEffects(caster, this);
        Debug.Log("HP remaining: " + HP.Value);
        return isDead();
    }

    public void useSpell(Square square, Spell spell)
    {
        if(powerGauge.Value - spell.pwrCost >= 0){//if the chosen has enough power to cast the spell
            powerGauge.Value -= spell.pwrCost;
            if (!square.isEmpty()){
                square.entity.targeted(this, spell);
            }
            Debug.Log("Power Gauge: " + powerGauge.Value);
        } else {
            Debug.Log("You don't have enough power to use this spell!");
        }
    }
}
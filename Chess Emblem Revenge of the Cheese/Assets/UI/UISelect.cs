using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine.UI;

public class UISelect : MonoBehaviour
{
    public TMP_Text UnitName;
    public TMP_Text StatsNames;
    public TMP_Text StatsNumbers;
    public Slider HealthSlider;

    public UnitStats Unittos;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void ChangeStatUI(UnitStats Unit, int CurrentHP)
    {
        UnitName.text = Unit.name;
        StatsNumbers.text = CurrentHP + "/" + Unit.hp + "\n" + "\n" + Unit.minDmg + "-" + Unit.maxDmg + "\n" + "\n" + Unit.spd + "\n" + "\n" + Unit.atkRange;
        ChangeSlider(CurrentHP, Unit);
        print("Connection terminated.");
    }
    void ChangeSlider(int CurrentHP, UnitStats Unit)
    {
        HealthSlider.maxValue = Unit.hp;
        HealthSlider.value = CurrentHP;
        
    }
}

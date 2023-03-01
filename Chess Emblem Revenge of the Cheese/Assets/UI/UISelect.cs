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
    public Image SelectIcon;

    public UnitStats Unittos;

    public Sprite[] UICharacters;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void ChangeStatUI(UnitStats Unit, int CurrentHP, string Colour)
    {
        UnitName.text = Unit.className;
        StatsNumbers.text = CurrentHP + "/" + Unit.hp + "\n" + "\n" + Unit.minDmg + "-" + Unit.maxDmg + "\n" + "\n" + Unit.spd + "\n" + "\n" + Unit.atkRange;
        ChangeSlider(CurrentHP, Unit);
        ChangeUICharacter(Unit, Colour, UICharacters);
    }
    void ChangeSlider(int CurrentHP, UnitStats Unit)
    {
        HealthSlider.maxValue = Unit.hp;
        HealthSlider.value = CurrentHP;
    }

    void ChangeUICharacter(UnitStats Unit, string Colour, Sprite[] UICharacters)
    {
        for (int q = 0; q < 12; q++)
        {
            if (Colour + " " + Unit.className == UICharacters[q].name)
            {
                SelectIcon.sprite = UICharacters[q];
            }
        }
    }
}

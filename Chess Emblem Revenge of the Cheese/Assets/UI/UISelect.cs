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
    public TMP_Text TimerText;
    public Slider HealthSlider;
    public Image SelectIcon;

    public UnitStats Unittos;

    public Sprite[] UICharacters;

    int betasecs = 0;
    int betamins = 1;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Update()
    {
        float alfa = Time.time;
        betasecs = Mathf.FloorToInt(alfa);
        if (betasecs >= 60 * betamins)
        {
            betamins += 1;
        }

        // Both mins and secs below 10
        if (betamins < 11 && (betasecs- 60 * (betamins - 1)) < 10)
        {
            TimerText.text = ("0" + (betamins - 1) + ":0" + (betasecs - 60 * (betamins - 1)));
        }
        else if (betamins > 10)
        {
            TimerText.text = ((betamins - 1) + ":0" + (betasecs - 60 * (betamins - 1)));
        }
        else if (betasecs - 60 * (betamins - 1) >= 10)
        {
            TimerText.text = ("0" + (betamins - 1) + ":" + (betasecs - 60 * (betamins - 1)));
        }
        else
        {
            TimerText.text = (betamins - 1 + ":" + (betasecs - 60 * (betamins - 1)));
        }
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

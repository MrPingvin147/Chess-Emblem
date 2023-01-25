using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Stats", menuName = "ScriptableObjects/Unit Stats", order = 1)]
public class UnitStats : ScriptableObject
{
    public int atkRange;
    public int minDmg;
    public int maxDmg;
    public int hp;
    public int spd;
    public bool splashDamage;
}

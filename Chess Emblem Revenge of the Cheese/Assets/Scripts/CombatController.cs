using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class CombatController : MonoBehaviour
{
    MovementController movementController;
    Slider slider;
    public UnitStats unitStats;
    public float currentHealth;
    int minDamage, maxDamage;
    int range;

    public delegate void OnDie(GameObject unit);
    public static event OnDie onDie;

    // Start is called before the first frame update
    void Start()
    {
        movementController = GetComponent<MovementController>();
        slider = GetComponentInChildren<Slider>();
        unitStats = movementController.unitStats;

        slider.maxValue = unitStats.hp;
        slider.value = unitStats.hp;

        currentHealth = unitStats.hp;
        minDamage = unitStats.minDmg;
        maxDamage = unitStats.maxDmg;
        range = unitStats.atkRange;
    }

    //Retunere en skade værdi mellem minDamage og maxDamage
    public int GetDamageValue()
    {
        int damage = Random.Range(minDamage, maxDamage);

        return damage;
    }

    //Retunere en splash skade værdi mellem minDamage og maxDamage divideret med 2
    public int GetSplashDamageValue()
    {
        int damage = Mathf.FloorToInt(Mathf.Min(minDamage/2, maxDamage/2));

        return damage;
    }

    //Updatere liv værdien på livbaren over units
    public void UpdateHealthbar()
    {
        slider.value = currentHealth;
    }

    //Tager en skade værdi og fjerner det fra unit liv
    public void TakeDamage(float damage)
    {
        if (currentHealth - damage <= 0)
        {
            UpdateHealthbar();
            Dead();
            return;
        }
        currentHealth -= damage;

        UpdateHealthbar();
    }

    //Kaldes når unit dør
    private void Dead()
    {
        if (onDie!= null)
        {
            onDie(gameObject);
        }
        print(transform.name + ": died");
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class CombatController : MonoBehaviour
{
    MovementController movementController;
    Slider slider;
    UnitStats unitStats;
    float currentHealth;
    int minDamage, maxDamage;
    bool doSplashDamage = false;
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

    public int GetDamageValue()
    {
        int damage = Random.Range(minDamage, maxDamage);

        return damage;
    }

    public void UpdateHealthbar()
    {
        slider.value = currentHealth;
    }

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

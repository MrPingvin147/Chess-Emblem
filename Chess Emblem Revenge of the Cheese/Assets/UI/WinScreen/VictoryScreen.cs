using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    GridBehaviour gridBehaviour;

    public Image fadePanel;

    public AnimationCurve fadeCurve;

    public GameObject victoryScreen;

    private void Start()
    {
        victoryScreen.SetActive(false);

        gridBehaviour = GridBehaviour.instance.GetComponent<GridBehaviour>();
    }

    private void OnEnable()
    {
        CombatController.onDie += EnableVictory;
    }

    private void OnDisable()
    {
        CombatController.onDie -= EnableVictory;
    }

    private void EnableVictory(GameObject gameObject)
    {
        if (gameObject.GetComponent<MovementController>().unitStats.className ==  "King")
        {
            victoryScreen.SetActive(true);
        }
    }

    public void RestartLevel()
    {
        StartCoroutine(FadeIn());

        gridBehaviour.DeleteLevel();
        gridBehaviour.GenerateLevel();

        victoryScreen.SetActive(false);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    IEnumerator FadeIn()
    {
        Color c = fadePanel.color;
        for (float alpha = 1f; alpha >= 0; alpha -= 0.01f)
        {
            c.a = fadeCurve.Evaluate(alpha);
            fadePanel.color = c;
            yield return new WaitForSeconds(0.02f);
        }
    }
}

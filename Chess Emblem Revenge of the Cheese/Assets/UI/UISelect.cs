using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UISelect : MonoBehaviour
{
    public TMP_Text UnitName;

    // Start is called before the first frame update
    void Start()
    {
        ChangeStatUI();
    }

    public void ChangeStatUI()
    {
        UnitName.text = "Gay";
    }
}

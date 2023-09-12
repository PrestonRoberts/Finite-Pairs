using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    private int value;

    [SerializeField] private TMP_Text text;

    public void SetValue(int newValue)
    {
        value = newValue;
        text.text = value.ToString();
    }
    
    public int GetValue()
    {
        return value;
    }

    public void ShowValue()
    {
        text.text = value.ToString();
    }

    public void HideValue ()
    {
        text.text = "";
    }

    public void HideTile ()
    {
        // Hide text
        HideValue();

        // Disable button component
        GetComponent<UnityEngine.UI.Button>().enabled = false;

        // Disable image component
        GetComponent<UnityEngine.UI.Image>().enabled = false;
    }
}

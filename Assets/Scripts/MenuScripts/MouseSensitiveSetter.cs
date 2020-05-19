using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitiveSetter : MonoBehaviour
{
    public Slider X;
    public Slider Y;

    public TextMeshProUGUI XT;
    public TextMeshProUGUI YT;

    // Start is called before the first frame update
    void Start()
    {
        X.value = PlayerPrefs.GetFloat("X", 1);
        Y.value = PlayerPrefs.GetFloat("Y", 1);

        XT.text = X.value.ToString();
        YT.text = Y.value.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnChange(Slider slider)
    {
        if(slider == X)
        {
            PlayerPrefs.SetFloat("X", slider.value);
        }
        else if(slider == Y)
        {
            PlayerPrefs.SetFloat("Y", slider.value);
        }

        XT.text = X.value.ToString();
        YT.text = Y.value.ToString();
    }
}
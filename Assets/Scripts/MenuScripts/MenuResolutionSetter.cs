using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuResolutionSetter : MonoBehaviour
{
    public TMP_Dropdown ResDrop;
    public Toggle FullSsToggle;

    // Start is called before the first frame update
    void Start()
    {
        List<TMP_Dropdown.OptionData> l = new List<TMP_Dropdown.OptionData>();

        foreach (Resolution r in Screen.resolutions)
        {
            l.Add(new TMP_Dropdown.OptionData($"{r.width}x{r.height} | {r.refreshRate}Hz"));
        }

        ResDrop.ClearOptions();
        ResDrop.AddOptions(l);

        ResDrop.onValueChanged.AddListener(OnValueChange);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnValueChange(int i)
    { 
        string[] sel = ResDrop.options[i].text.Split('x');
        int w = int.Parse(sel[0]);
        int h = int.Parse(sel[1].Split('|')[0]);

        Screen.SetResolution(w, h, FullSsToggle.isOn);
    }
}
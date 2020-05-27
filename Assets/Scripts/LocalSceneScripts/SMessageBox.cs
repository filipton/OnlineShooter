using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SMessageBox : MonoBehaviour
{
    public static SMessageBox singleton;

    [Header("MessageBoxContent")]
    public GameObject MessageBox;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DescriptionText;

    private void Awake()
    {
        if (singleton != null)
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        singleton = this;
    }

    public void ShowMessageBox(string title, string description)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        TitleText.text = title;
        DescriptionText.text = description;

        MessageBox.SetActive(true);
    }

    public void CloseMessageBox()
    {
        MessageBox.SetActive(false);
    }
}
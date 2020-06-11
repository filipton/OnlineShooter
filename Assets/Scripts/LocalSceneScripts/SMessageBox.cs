using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SMessageBox : MonoBehaviour
{
    public static SMessageBox singleton;

    [Header("MessageBoxContent")]
    public GameObject MessageBox;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DescriptionText;

    private void Awake()
    {
		SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        if (FindObjectsOfType<SMessageBox>().Length > 1)
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        singleton = this;
    }

	private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
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
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
    public CanvasGroup cg;

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
        CursorManager.RefreshLock("_smb", false);
        TitleText.text = title;
        DescriptionText.text = description;

        MessageBox.SetActive(true);
    }
    public void ShowMessageBox(string title, string description, float time)
    {
        StartCoroutine(SMBTime(title, description, time));
    }

    IEnumerator SMBTime(string title, string description, float time)
	{
        CursorManager.RefreshLock("_sbm", false);
        TitleText.text = title;
        DescriptionText.text = description;

        MessageBox.SetActive(true);

        yield return new WaitForSeconds(time);

        bool alphaToZero = true;
        float t = 0;
		while (alphaToZero)
		{
            t += 0.01f;
            cg.alpha = Mathf.Lerp(cg.alpha, 0, t);

            if(cg.alpha <= 0)
			{
                alphaToZero = false;
			}

            yield return new WaitForSeconds(0.01f);
        }

        CursorManager.RefreshLock("_sbm", true);
        TitleText.text = string.Empty;
        DescriptionText.text = string.Empty;

        MessageBox.SetActive(false);
        cg.alpha = 1;
    }

    public void CloseMessageBox()
    {
        MessageBox.SetActive(false);
        CursorManager.RefreshLock("_sbm", true);
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuReferences : MonoBehaviour
{
    public static MenuReferences singleton;

    [Header("Login Fields")]
    public TMP_InputField l_nickField;
	public TMP_InputField l_passwordField;

    [Header("Register Fields")]
    public TMP_InputField r_nickField;
    public TMP_InputField r_passwordField;
    public TMP_InputField r_passwordAgainField;

    private void Awake()
	{
        singleton = this;
	}

	public void LoginB(bool b)
	{
        LoginSystem ls = FindObjectOfType<LoginSystem>();
        string ret = ls.Login(b ? r_nickField.text : l_nickField.text, b ? r_passwordField.text : l_passwordField.text);

        if(ret.Contains("AUTH CODE: "))
		{
            ls.LoginToken = ret.Replace("AUTH CODE: ", "");
            //Debug.Log($"<b><color=green>{ls.LoginToken}</color></b>");
        }

        SceneManager.LoadScene("Menu");
	}

    public void RegisterB()
    {
        if(r_passwordField.text == r_passwordAgainField.text)
		{
            LoginSystem ls = FindObjectOfType<LoginSystem>();
            string ret = ls.Register(r_nickField.text, r_passwordField.text);

            //Debug.Log($"<b><color=green>{ret}</color></b>");
        }

        LoginB(true);
    }
}
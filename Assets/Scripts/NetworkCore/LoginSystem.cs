using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LoginSystem : MonoBehaviour
{
    #region Keys
    string publicKeyString = "<?xml version=\"1.0\" encoding=\"utf-16\"?><RSAParameters xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><Exponent>AQAB</Exponent><Modulus>iFbg2vbsBFPxVRLNJq97AnwTNn4OyBTbaovlq+SOtChPTvGNOCnnJ9XqB17ygOe8Z5OwL5l2WDBngPGFiqq4Q1wwTFv2v/hY1YCbI5HnrchJJ5yGD76LPpz1zLg8ey8/YWDIbFnu7MU2apvDV2Z3H4/L9RYGDKdj38bi+s6EBWaZei1gHEwfoJuCwpYNszdwqO7uWSI6T9bCnoRJAeuNg6X0s66KsAmTCV9d5obtGf3HpqHqJBVdy59hW16weMycGZ+LMWaEaQk1m2alkUnAZe37OWs4I5bCpaCSVUGQdJafTo0P/Gu+S/OFSPoFj201TuI9lb681lqR8hlImvYSKKtpHfYg9mXes95bgujJoAU60zsFkSvFmpuDX6IDovG1vl/o+GCm9cFTiTYPlLaKvvMys7OGnoRwFO5Lwo3ojgBBjuGbu9quN2jmd2EbJv59h2lbOlNwomyzmfiiEp1CdLMeePXOSxIZ8I3N1vhMpTJMiCST6MxXyNgNi13pjIP12qr+2LlPKjpLdqCTit9+l6Sur03quWYG73EsQyXFIYV9O8DAv8sPZby6rHrmeqIreIRGhkdeXKUElIGJPjoBD3aazryyLZG3d0WSXjuhJyWu6id8eDZhV+DQueOic7BVBmx5PyGCsjqYN+TaY/pRTj1QEt7ZptWFY2E7oyAD9Qs=</Modulus></RSAParameters>";
    string privateKeyString = "<?xml version=\"1.0\" encoding=\"utf-16\"?><RSAParameters xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><Exponent>AQAB</Exponent><Modulus>iFbg2vbsBFPxVRLNJq97AnwTNn4OyBTbaovlq+SOtChPTvGNOCnnJ9XqB17ygOe8Z5OwL5l2WDBngPGFiqq4Q1wwTFv2v/hY1YCbI5HnrchJJ5yGD76LPpz1zLg8ey8/YWDIbFnu7MU2apvDV2Z3H4/L9RYGDKdj38bi+s6EBWaZei1gHEwfoJuCwpYNszdwqO7uWSI6T9bCnoRJAeuNg6X0s66KsAmTCV9d5obtGf3HpqHqJBVdy59hW16weMycGZ+LMWaEaQk1m2alkUnAZe37OWs4I5bCpaCSVUGQdJafTo0P/Gu+S/OFSPoFj201TuI9lb681lqR8hlImvYSKKtpHfYg9mXes95bgujJoAU60zsFkSvFmpuDX6IDovG1vl/o+GCm9cFTiTYPlLaKvvMys7OGnoRwFO5Lwo3ojgBBjuGbu9quN2jmd2EbJv59h2lbOlNwomyzmfiiEp1CdLMeePXOSxIZ8I3N1vhMpTJMiCST6MxXyNgNi13pjIP12qr+2LlPKjpLdqCTit9+l6Sur03quWYG73EsQyXFIYV9O8DAv8sPZby6rHrmeqIreIRGhkdeXKUElIGJPjoBD3aazryyLZG3d0WSXjuhJyWu6id8eDZhV+DQueOic7BVBmx5PyGCsjqYN+TaY/pRTj1QEt7ZptWFY2E7oyAD9Qs=</Modulus><P>oO/NPf8+YcVqt23ZFDi5Yoz0WeoQ4pf0NnrnkekSwjzNIdyOAutmtIUBxBS3j7n9/wSh2C31FlS8E2XB5x71PQ/fR/uC0tF+cu1oEd2sy9BQV+qK3smExuHVNLD0xbbmRCLvxDsW4MWFVMbO2gnIx+6ROJ2RO+lyOkz8iuaKuA9eHG6NSwFWaRoaUdvCBeax1/FQXjwWPv8lfN67U4toPwt7tHWLrxpoT2XhCOy6Zdo1RC5upSx5qfpC9KUo+7BCaGhkNqzjRrRVI2/bR0eQDVEfS50Ng+Lr5F4oyU9qe7I24zhfihTR1dK63E5BQH5iwMulo5IpRWGZeQjB3REUlw==</P><Q>2N+QbCLmOVtR3mSSlJaB3RQesOXE7HyMEgNMZST2vujkZWSrsymtortNv8uZziVOeoF0Gw4HHY1DSRhMso/68qHNTcLP4RtJr0FjLkISTRTDTDhunyvygEfbWPkJKutcH4U/mkRfkhOdwPvmY8zYTkSD0p0nRMwqQl6unJKJi4wMji8e3jOhVm3r0lATMNJo4Okdx3l1WyEtYjXfI9LHNVi7FXujSnxjRGDqiww0WRdLCOVhrzQbY5428ZJAFfixCp+cCGYESJrK2lJlseLlcPJ6jAbOow+QgoNvSRjxW6utv9WPLrsxoIGrSeax+7U/zf5v50bFInaL8mQn6smtrQ==</Q><DP>HFxF4eACAT7F4I82Cu84FBrc0J+D3DqE6bjc6ASXrq0CRS5VxEPxdGxf9ErmUiinjvIlBzCKGSZTgmOTOQRmmR0b7tZupnIOg3g8t7FFyyDpl6SYVkxS3q1bYg6xA8qIZ4d0kzMKhA+qwFbMivXgcQMkikKSHmm0HHwGoP+hgHAM2JvOwGH69UiJTNWaKmoN+wzmacTS8sUJuSiTAFT3yOxgH1v7gp73tM4ySt1Ut7AtZNd93hOpWP/mHrCgOUJFYZrL1iq/Xm1W94i1rqj4xLAAANYrVIt/oF3LeDZLgILWU1JS28VxeOnKzhnoO9JJ2snIZbvTFAvv/LlDWI6y/w==</DP><DQ>JYapqsxvIcWXaO/mHSfd2K8+kEVxyanTYtqHQYkf2PDbdF/zBdFUZjAax/Em7/Csn63+tR+8Q3TKC7/UpaEcPxeYOrhW2BCss1KRItPz7rqFAI9TNN+Z2nCUl9SfrXmEO0AHPV5PWythxJ7RwrsTQpfAZ5Uh0lmxhkJz3EvO2QKmuYOVx8UPSbFDeOS6LVx7IU+1A2sUz2vyOmgv8DbsmWJ9HL6EBsN3OcG7U8wM0s2hAi0Otg5CHv6lay9vP14E5F2x1hIw8481Qom3Zwxnvx917PzQjI6I5sxvWSqjuMUAD42YdPGXKLrHNMZiTbeTDsXT/yTTv57boFmMPqgfEQ==</DQ><InverseQ>iD/GaxK2k8uoWqqWrKIxgHbsJNaj8449Gn8e4ZMA7mFm2wfrAQVfK5OjKWhgdRVYhstm+yKRpWwg6x/ZNS0/hBhf0269LlYNdAP6N0tNE405sSu0q/dSJs97Ec3MJ39lWYvkxYhm/pROtbhW295gf5X1qE2PHhjftYtoAH5ULu8pvGFEULnqW+dGeC20E/y96n2w9WGwrEs5XWp5rzFD1HFmCtnsuVGZYf0v0GxX5IlL8W+h2RT03BZBX3DqhFcFvjorJ6LANvudxAUil3rvIMrcVITPnWYzl2QLBIMDhDBnUCNylxbqvbE6ucKeS1grj06th/9uG6Ea1tPczLF3hQ==</InverseQ><D>WQt88iXBAhadEPYlnPWRPo8wsS9lkhHlOFJEX574APwuyB2FIUBk46SXo9bERaKUb4WK1ZvCKlj60MFteaKElCH6vXR21OxrgI3tRS7/BIZCGSfmexP0t6El/F37ymySVZQzcmjOXj9zie9Php66gJeXGuwWaV9OIJIyHqKQoUJgTOaj1fd11Jycq1/7dKhp4Tuj2f073nciRwuY7zMcCvZxuoTtuYmXSdDKcEyBQK96HP+0/3og92OR324Zjdv8u9NywoH1SvCj3WoOCrUDfs8OfHLliIDnges8h4g5w5u4UkJZRYBNGifjTkVVKlamMW4b0NhxVwJyPY971xGNy6FejLRVqkljBsWOG+xCKlzQTTBvd35TgSDlyJ5wp5VXz7wU2rdmbbWs9srEiZS47RxAvAsh857vHXFLyxNgKg1UW/U2l32janziav5jlHjkt////IAxYH209AlUG5SfUByL4gZ7iuy0Ez4q7w1sFMURbhYJJTzQn23Tn++OR0uohWwKC2PWaE+7o5Ts2z1uNmeECkYrbUy1wl68KwQ04jZ+g7p62FenD/BdT+8jCHEQpGfQq/sW+quQBFSa/r/JxAolcM2lH8FkylRxvRpykRcKRLrReyOtQJUDtK+E5Jxk0+FMQIkQ4on2zV9ZiCfzEvT6N/kMO2gOdOA88tp8ldk=</D></RSAParameters>";
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.W))
		{
            print(Login("filip", "HGFesug337+"));
		}
        else if (Input.GetKeyDown(KeyCode.S))
		{
            print(Register("filip", "HGFesug337+"));
        }
    }

    public string Login(string nick, string password)
    {
        if (PlayerPrefs.HasKey("User") && PlayerPrefs.HasKey("Password"))
        {
            if(!string.IsNullOrWhiteSpace(nick) && !string.IsNullOrWhiteSpace(password))
			{
                PlayerPrefs.SetString("User", Encrypt(nick, publicKeyString));
                PlayerPrefs.SetString("Password", Encrypt(password, publicKeyString));
            }

            nick = Decrypt(PlayerPrefs.GetString("User"), privateKeyString);
            password = Decrypt(PlayerPrefs.GetString("Password"), privateKeyString);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(nick) && !string.IsNullOrWhiteSpace(password))
			{
                PlayerPrefs.SetString("User", Encrypt(nick, publicKeyString));
                PlayerPrefs.SetString("Password", Encrypt(password, publicKeyString));
            }
		}

        if (!string.IsNullOrWhiteSpace(nick) && !string.IsNullOrWhiteSpace(password))
		{
            string r = new WebClient().DownloadString($"https://login.filipton.space/UserManagment.php?mode=l&u={nick}&p={password}");

			if(r.Contains("Error code: "))
			{
				switch (r)
				{
                    case "Error code: 0":
                        return "Everything is ok!";
                    case "Error code: 1":
                        return "User already exists!";
                    case "Error code: 2":
                        return "Error in creating user!";
                    case "Error code: 3":
                        return "User doesn't exists!";
                    case "Error code: 4":
                        return "Incorrect Password!";
                }
			}
			else
			{
                return $"AUTH CODE: {r}";
			}
        }

        return "Error code: 5";
    }

    public string Register(string nick, string password)
    {
        if (!string.IsNullOrWhiteSpace(nick) && !string.IsNullOrWhiteSpace(password))
        {
            PlayerPrefs.SetString("User", Encrypt(nick, publicKeyString));
            PlayerPrefs.SetString("Password", Encrypt(password, publicKeyString));

            string r = new WebClient().DownloadString($"https://login.filipton.space/UserManagment.php?mode=r&u={nick}&p={password}");

            if (r.Contains("Error code: "))
            {
                switch (r)
                {
                    case "Error code: 0":
                        return "Everything is ok!";
                    case "Error code: 1":
                        return "User already exists!";
                    case "Error code: 2":
                        return "Error in creating user!";
                    case "Error code: 3":
                        return "User doesn't exists!";
                    case "Error code: 4":
                        return "Incorrect Password!";
                }
            }
            else
            {
                return $"AUTH CODE: {r}";
            }
        }

        return "Error code: 5";
    }

    public static string Encrypt(string textToEncrypt, string publicKeyString)
    {
        var bytesToEncrypt = Encoding.UTF8.GetBytes(textToEncrypt);

        using (var rsa = new RSACryptoServiceProvider(4096))
        {
            try
            {
                rsa.FromXmlString(publicKeyString.ToString());
                var encryptedData = rsa.Encrypt(bytesToEncrypt, true);
                var base64Encrypted = Convert.ToBase64String(encryptedData);
                return base64Encrypted;
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }
    }

    public static string Decrypt(string textToDecrypt, string privateKeyString)
    {
        var bytesToDescrypt = Encoding.UTF8.GetBytes(textToDecrypt);

        using (var rsa = new RSACryptoServiceProvider(4096))
        {
            try
            {

                // server decrypting data with private key                    
                rsa.FromXmlString(privateKeyString);

                var resultBytes = Convert.FromBase64String(textToDecrypt);
                var decryptedBytes = rsa.Decrypt(resultBytes, true);
                var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                return decryptedData.ToString();
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }
    }

    public static string GetKeyString(RSAParameters publicKey)
    {

        var stringWriter = new System.IO.StringWriter();
        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
        xmlSerializer.Serialize(stringWriter, publicKey);
        return stringWriter.ToString();
    }
}
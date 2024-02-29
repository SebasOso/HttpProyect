using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class AuthManager : MonoBehaviour
{ 
    [Header("Login")]
    public TMP_InputField usernameLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text confirmLoginText;
    public TMP_Text warningLoginText;
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_Text confirmRegisterText;
    public TMP_Text warningRegisterText;

    private string _url = "";
    public void SendRegister()
    {
        AuthData authData = new AuthData();
        authData.username = usernameRegisterField.text;
        authData.password = passwordRegisterField.text;
        string _jsonData = JsonUtility.ToJson(authData);

        StartCoroutine(Register(_jsonData));
    }
    public void SendLogin()
    {
        AuthData authData = new AuthData();
        authData.username = usernameRegisterField.text;
        authData.password = passwordRegisterField.text;
        string _jsonData = JsonUtility.ToJson(authData);

        StartCoroutine(Login(_jsonData));
    }
    private IEnumerator Register(string _json)
    {
        _url = "https://sid-restapi.onrender.com/api/usuarios";
        UnityWebRequest req = UnityWebRequest.Put(_url, _json);
        req.method = "POST";

        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error: " + req.error);
        }
        else
        {
            if (req.responseCode == 200)
            {
                confirmRegisterText.text = "Signed In!";
                warningRegisterText.text = "";
                Debug.Log("Signed In!");
            }
            else
            {
                warningRegisterText.text = "Error: " + "|" + req.error;
            }
        }
    }
    private IEnumerator Login(string _json)
    {
        _url = "https://sid-restapi.onrender.com/api/auth/login";
        UnityWebRequest req = UnityWebRequest.Put(_url, _json);
        req.method = "POST";

        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if(req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error: " + req.error);
        }
        else
        {
            if(req.responseCode == 200)
            {
                confirmLoginText.text = "Logged In!";
                warningLoginText.text = "";
                Debug.Log("Logged In!");
            }
            else
            {
                confirmLoginText.text = "";
                warningLoginText.text = "Error: " + "|" + req.error;
            }
        }
    }
}

[System.Serializable]
public class AuthData
{
    public string username;
    public string password;
}

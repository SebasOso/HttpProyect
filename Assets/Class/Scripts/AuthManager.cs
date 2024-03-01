using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.UI;

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
    [Header("Score")]
    public Transform contentTransform;
    public GameObject scorePrefab;
    public TMP_Text scoreWarning;
    public TMP_InputField pointsField;

    [Header("For Debugging")]
    [SerializeField] private string currentUserToken;
    [SerializeField] private string currentUsername;
    [SerializeField] private string currentPassword;

    [Header("Buttons")]
    [SerializeField] private Button registerButton;
    [SerializeField] private Button scoreButton;
    [SerializeField] private Button updateScoreButton;

    private string _url = "";
    private void Start()
    {
        currentUserToken = PlayerPrefs.GetString("userToken");
        currentUsername = PlayerPrefs.GetString ("username");
        currentPassword = PlayerPrefs.GetString ("password");
        if(currentUsername != "")
        {
            AuthData authData = new AuthData();
            authData.username = currentUsername;
            authData.password = currentPassword;
            string _jsonData = JsonUtility.ToJson(authData);

            StartCoroutine(Login(_jsonData));
            registerButton.interactable = false;
            scoreButton.interactable = true;
            updateScoreButton.interactable = true;
        }
        else
        {
            registerButton.interactable = true;
            scoreButton.interactable = false;
            updateScoreButton.interactable = false;
        }
    }
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
    public void UpdateScoreBoard()
    {
        contentTransform.transform.position = new Vector3(contentTransform.transform.position.x, -10000f, 0f);
        StartCoroutine(Score());
    }
    public void UpdateScore()
    {
        PatchScore patchScore = new PatchScore();
        patchScore.username = PlayerPrefs.GetString("username");
        Data data = new Data();
        if (int.TryParse(pointsField.text, out int points))
        {
            data.score = points;
        }
        else
        {
            data.score = 0;
        }
        patchScore.data = data;
        string _jsonData = JsonUtility.ToJson(patchScore);
        Debug.Log(_jsonData);
        StartCoroutine(UpdateScore(_jsonData));
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
                StartCoroutine(Login(_json));
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
                AuthData data = JsonUtility.FromJson<AuthData>(req.downloadHandler.text);
                Debug.Log("Token: " + data.token);
                currentUserToken = data.token;
                PlayerPrefs.SetString("username", data.usuario.username);
                PlayerPrefs.SetString("userToken", currentUserToken);
                PlayerPrefs.SetString("password", data.password);
                Debug.Log("Logged In!");
                Debug.Log(data.usuario.username);
                Debug.Log(data.usuario.data.score);
                registerButton.interactable = false;
                scoreButton.interactable = true;
                updateScoreButton.interactable = true;
            }
            else
            {
                confirmLoginText.text = "";
                warningLoginText.text = "Error: " + "|" + req.error;
            }
        }
    }
    private IEnumerator UpdateScore(string _json)
    {
        _url = "https://sid-restapi.onrender.com/api/usuarios";
        UnityWebRequest req = UnityWebRequest.Put(_url, _json);
        req.method = "PATCH";

        req.SetRequestHeader("x-token", currentUserToken);
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
                Debug.Log("Score Updated!");
            }
            else
            {
                Debug.LogError("Error" + req.error);
            }
        }
    }
    private IEnumerator Score()
    {
        _url = "https://sid-restapi.onrender.com/api/usuarios";
        UnityWebRequest req = UnityWebRequest.Get(_url);
        if(currentUserToken == "")
        {
            Debug.Log("No User Token");
        }
        req.SetRequestHeader("x-token", currentUserToken);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error: " + req.error);
        }
        else
        {
            if (req.responseCode == 200)
            {
                foreach (Transform child in contentTransform.transform)
                {
                    Destroy(child.gameObject);
                }

                AuthData data = JsonUtility.FromJson<AuthData>(req.downloadHandler.text);

                data.usuarios = data.usuarios.OrderByDescending(u => u.data.score).ToArray();

                for (int i = 0; i < Mathf.Min(6, data.usuarios.Length); i++)
                {
                    string username = data.usuarios[i].username;
                    string id = data.usuarios[i]._id;
                    int points = data.usuarios[i].data.score;
                    GameObject userScore = Instantiate(scorePrefab, contentTransform);
                    userScore.GetComponent<ScoreElement>().NewScoreElement(username, points);
                }
                Debug.Log("Scores!");
                scoreWarning.text = "";
            }
            else
            {
                scoreWarning.text = "No User Token recognized, please login...";
                Debug.LogError("No token, please login...");
            }
        }
    }
}
[System.Serializable]
public class AuthData
{
    public string username;
    public string password;
    public UserScore usuario;
    public UserScore[] usuarios;
    public string token;
}
[System.Serializable]
public class UserScore
{
    public string _id;
    public string username;
    public Data data;
}
[System.Serializable]
public class PatchScore
{
    public string username;
    public Data data;
}
[System.Serializable]
public class Data
{
    public int score;
}
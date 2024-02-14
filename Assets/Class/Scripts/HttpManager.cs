using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HttpManager : MonoBehaviour
{
    [SerializeField] private string url;
    private void Start()
    {
        
    }
    public void SendRequest()
    {

        StartCoroutine(GetCharacters());
    }
    private IEnumerator GetCharacters()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://pokeapi.co/api/v2/pokemon");
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error");
            
        }
        else
        {
            if(request.responseCode == 200)
            {
                Debug.Log("Connceted!");
                Debug.Log(request.downloadHandler.text);
                JsonData data = JsonUtility.FromJson<JsonData>(request.downloadHandler.text);
                foreach (CharacterData character in data.results)
                {
                    Debug.Log("name: " + character.name);
                }
            }
        }
    }
}
[System.Serializable]
public class JsonData
{
    public CharacterData[] results;
}
[System.Serializable]
public class CharacterData
{
    public string name;
    public string url;
    public string image;
}

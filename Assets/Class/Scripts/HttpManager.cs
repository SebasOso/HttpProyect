using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HttpManager : MonoBehaviour
{
    [SerializeField] private string fakeApiUrl = "https://my-json-server.typicode.com/SebasOso/HttpProyect";
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
        UnityWebRequest request = UnityWebRequest.Get(fakeApiUrl + "/players");
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error");
            
        }
        else
        {
            if(request.responseCode == 200)
            {
                Debug.Log("Connected!");
                Debug.Log(request.downloadHandler.text);

                string json = "{ \"players\" :"+request.downloadHandler.text + "}";

                JsonData data = JsonUtility.FromJson<JsonData>(json);
                foreach (CharacterData player in data.players)
                {
                    Debug.Log("name: " + player.name);
                    Debug.Log("id: " + player.id);
                    foreach (var card in player.deck)
                    {
                        Debug.Log("Card: " + card);
                    }
                }
            }
        }
    }
}
[System.Serializable]
public class JsonData
{
    public CharacterData[] players;
}
[System.Serializable]
public class CharacterData
{
    public int id;
    public string name;
    public int[] deck;
}

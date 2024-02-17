using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class HttpManager : MonoBehaviour
{
    [SerializeField] private List<User> users = new List<User>();
    [SerializeField] private string fakeApiUrl = "https://my-json-server.typicode.com/SebasOso/HttpProyect";
    private string url = "https://rickandmortyapi.com/api/character";

    [SerializeField] private List<List<Texture>> userTextures = new List<List<Texture>>();

    private void Start()
    {
        // Initialize userTextures lists
        for (int i = 0; i < 5; i++)
        {
            userTextures.Add(new List<Texture>());
        }
    }
    public void SendRequest()
    {
        StartCoroutine(GetCharactersNames());
    }
    public void UpdateCards()
    {

    }
    private IEnumerator GetCharactersNames()
    {
        UnityWebRequest request = UnityWebRequest.Get(fakeApiUrl + "/players");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error");
        }
        else
        {
            if (request.responseCode == 200)
            {
                Debug.Log("Connected!");
                Debug.Log(request.downloadHandler.text);

                string json = "{ \"players\" :" + request.downloadHandler.text + "}";

                JsonData data = JsonUtility.FromJson<JsonData>(json);
                for (int i = 0; i < data.players.Length; i++)
                {
                    int[] deck = data.players[i].deck;
                    for (int j = 0; j < deck.Length; j++)
                    {
                        string urlToDownloadImage = url + "/avatar/" + deck[j] + ".jpeg";
                        Debug.Log(urlToDownloadImage);
                        StartCoroutine(DownloadImage(i, urlToDownloadImage));
                    }
                }
                for (int i = 0;i < data.players.Length; i++)
                {
                    users[i].SetName(data.players[i].name);
                    users[i].SetTextures(userTextures[i]);
                }
                StartCoroutine(SetCardsImages());
                StartCoroutine(HideUsers());
            }
        }
    }
    private IEnumerator SetCardsImages()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < users.Count; i++)
        {
            users[i].SetCards();
        }
    }
    private IEnumerator HideUsers()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 1; i < users.Count; i++)
        {
            users[i].gameObject.SetActive(false);
        }
    }
    private IEnumerator DownloadImage(int userIndex, string url)
    {
        UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(req.error);
        }
        else
        {
            Texture texture = ((DownloadHandlerTexture)req.downloadHandler).texture;
            userTextures[userIndex].Add(texture);
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

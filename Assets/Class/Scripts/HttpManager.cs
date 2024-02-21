using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class HttpManager : MonoBehaviour
{
    [SerializeField] private User userInfo;
    [SerializeField] private string fakeApiUrl = "https://my-json-server.typicode.com/SebasOso/HttpProyect";
    private string url = "https://rickandmortyapi.com/api/character";

    [SerializeField] private List<List<Card>> decks = new List<List<Card>>();

    private List<CharacterData> users = new List<CharacterData>();

    public void GetAllDecks()
    {
        StartCoroutine(GetUserDecks());
    }
    public void GetDeck(int userId)
    {
        GetUserCards(userId);
    }
    private IEnumerator GetUserDecks()
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
                    users.Add(data.players[i]);
                    userInfo.SetName(data.players[0].name);
                    int[] deck = data.players[i].deck;
                    List<Card> userDeck = new List<Card>();
                    for (int j = 0; j < deck.Length; j++)
                    {
                        string characterUrl = url + "/" + deck[j];
                        yield return StartCoroutine(GetCardFromCharacterUrl(characterUrl, (card) =>
                        {
                            userDeck.Add(card);
                        }));
                    }
                    decks.Add(userDeck);
                }
                GetNamesCardsDeck01();
            }
        }
    }
    private IEnumerator GetUserCards(int userId)
    {
        List<Texture> cardImages = new List<Texture>();  
        List<Card> userDeck = decks[userId];
        for (int i = 0; i < userDeck.Count; i++)
        {
            string imageUrl = userDeck[i].image;
            yield return StartCoroutine(DownloadImageIntoList(imageUrl, cardImages));
        }
        userInfo.SetCardImages(cardImages);
    }
    private IEnumerator GetCardFromCharacterUrl(string characterUrl, Action<Card> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(characterUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error");
        }
        else
        {
            string json = request.downloadHandler.text;
            Card card = JsonUtility.FromJson<Card>(json);
            callback(card);
        }
    }
    private IEnumerator DownloadImageIntoList(string url, List<Texture> imagesToStore)
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
            imagesToStore.Add(texture);
        }
    }
    public void GetNamesCardsDeck(int userId)
    {
        List<string> deckNames = new List<string>();
        foreach (Card card in decks[userId])
        {
            deckNames.Add(card.name);
        }
        userInfo.SetCardNames(deckNames);
    }
    public void GetNamesCardsDeck01()
    {
        userInfo.SetName(users[0].name);
        GetNamesCardsDeck(0);
        StartCoroutine(GetUserCards(0));
    }
    public void GetNamesCardsDeck02()
    {
        userInfo.SetName(users[1].name);
        GetNamesCardsDeck(1);
        StartCoroutine(GetUserCards(1));
    }
    public void GetNamesCardsDeck03()
    {
        userInfo.SetName(users[2].name);
        GetNamesCardsDeck(2);
        StartCoroutine(GetUserCards(2));
    }
    public void GetNamesCardsDeck04()
    {
        userInfo.SetName(users[3].name);
        GetNamesCardsDeck(3);
        StartCoroutine(GetUserCards(3));
    }
    public void GetNamesCardsDeck05()
    {
        userInfo.SetName(users[4].name);
        GetNamesCardsDeck(4);
        StartCoroutine(GetUserCards(4));
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
    [System.Serializable]
    public class Card
    {
        public int id;
        public string name;
        public string image;
    }
}
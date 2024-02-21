using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    [SerializeField] List<RawImage> cards = new List<RawImage>();
    [SerializeField] List<TextMeshProUGUI> cardsNames = new List<TextMeshProUGUI>();
    [SerializeField] List<Texture> textures = new List<Texture>();
    [SerializeField] List<string> names = new List<string>();
    [SerializeField] TextMeshProUGUI userName;
    public void SetName(string name)
    {
        userName.text = name;
    }
    public void SetCardImages(List<Texture> textures)
    {
        this.textures = textures;
        SetImages();
    }
    private void SetImages()
    {
        for (int i = 0; i < 5; i++)
        {
            cards[i].texture = textures[i];
        }
    }
    public void SetCardNames(List<string> names)
    {
        this.names = names;
        SetCardsNames();
    }
    private void SetCardsNames()
    {
        for (int i = 0; i < 5; i++)
        {
            cardsNames[i].text = names[i];
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    [SerializeField] List<RawImage> cards = new List<RawImage>();
    [SerializeField] List<Texture> textures = new List<Texture>();
    [SerializeField] TextMeshProUGUI userName;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void SetName(string name)
    {
        userName.text = name;
    }
    public void SetTextures(List<Texture> textures)
    {
        this.textures = textures;
    }
    public void SetCards()
    {
        for (int i = 0; i < 5; i++)
        {
            cards[i].texture = textures[i];
        }
    }
}

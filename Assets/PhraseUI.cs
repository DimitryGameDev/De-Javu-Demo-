using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhraseUI : MonoBehaviour
{
    [SerializeField] private TMP_Text phraseText;
    [SerializeField] private Image portraitImage;

    public void Setup(string phraseText, Sprite portraitImage)
    {
        this.phraseText.text = phraseText;
        this.portraitImage.sprite = portraitImage;
    }
    
}

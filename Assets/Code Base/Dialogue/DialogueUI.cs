using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class DialogueUI : SingletonBase<DialogueUI>
{
    
    [Space]
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image portraitImage;
    [Space]
    [SerializeField] private Transform answerButtonsContainer;
    [SerializeField] private Transform dialogueTextContainer;
    [SerializeField] private Transform dialogueNameContainer;
    [SerializeField] private Transform buttonLog;
    [Space]
    [SerializeField] private GameObject answerButtonPrefab;
    [Space]
    [SerializeField] private EmotionObj emotionsObj;
    
    private void Awake()
    {
        Init();
        ClearAnswers();
    }

    public void SetProperties(Npc npc,string npcText,NpcEmotion emotion)
    {
        switch (npc)
        {
            case Npc.Hero:
                dialogueTextContainer.GetComponent<Image>().color = new Color32(146,255,255,255);
                dialogueNameContainer.GetComponent<Image>().color = new Color32(146,255,255,255);
                break;
            case Npc.April:
                dialogueTextContainer.GetComponent<Image>().color = new Color32(255,217,217,255);
                dialogueNameContainer.GetComponent<Image>().color = new Color32(255,217,217,255);
                break;
            case Npc.Patient:
                dialogueTextContainer.GetComponent<Image>().color = Color.green;
                break;
            default: dialogueTextContainer.GetComponent<Image>().color = Color.white;
                break;
        }
        npcNameText.text = npc.ToString();
        dialogueText.text = npcText;
        var portrait = FindEmotions(npc,emotion);
        portraitImage.sprite = portrait;
        DialogueLog.Instance.AddPhrase(npcText,portrait);
        ShowDialogue();
    }

    private Sprite FindEmotions(Npc npc,NpcEmotion emotion)
    {
        foreach (var emotions in emotionsObj.npc)
        {
            if (emotions.npc != npc) continue;
            foreach (var type in emotions.sprites)
            {
                if (type.NpcEmotion == emotion)
                {
                    return type.Sprites[Random.Range(0, type.Sprites.Length)];
                }
            }
        }
        return null;
    }
    private void ShowDialogue()
    {
        dialogueTextContainer.gameObject.SetActive(true);
        portraitImage.gameObject.SetActive(true);
        buttonLog.gameObject.SetActive(true);
        HideAnswers();
    }

    private void HideDialogue()
    {
        dialogueTextContainer.gameObject.SetActive(false);
        portraitImage.gameObject.SetActive(false);
    }

    private void ShowAnswers()
    {
        answerButtonsContainer.gameObject.SetActive(true);
        HideDialogue();
    }

    private void HideAnswers()
    {
        answerButtonsContainer.gameObject.SetActive(false);
    }
    
    public void CreateAnswer(PlayerAnswer[] playerAnswers)
    {
        foreach (var t in playerAnswers)
        {
            var buttonPrefab = Instantiate(answerButtonPrefab, answerButtonsContainer);
            var portrait = FindEmotions(Npc.Hero,NpcEmotion.Neutral);
            buttonPrefab.GetComponent<AnswerButton>().SetProperties(t.text,t.toNode,t.value,t.exit,portrait);
        }
        ShowAnswers();
    }

    public void ClearAnswers()
    {
        HideAnswers();
        for (int i = 0; i < answerButtonsContainer.childCount; i++)
        {
            Destroy(answerButtonsContainer.GetChild(i).gameObject);
        }
    }
    
    public void EndDialogue()
    {
        buttonLog.gameObject.SetActive(false);
        ClearAnswers();
        HideDialogue();
    }
}

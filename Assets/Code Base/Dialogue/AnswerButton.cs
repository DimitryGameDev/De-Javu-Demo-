using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    private Button _button;
    private int _nextNode;
    private int _value;
    private string _text;
    private Sprite _sprite;

    public void SetProperties(string question,int nextNode, int value,bool isExit,Sprite portrait)
    {
        _value = value;
        _nextNode = nextNode;
        _text = question;
        _sprite = portrait;
        
        _button = GetComponent<Button>();
        _button.GetComponentInChildren<TMP_Text>().text = _text;
        if (isExit)
        {
            _button.onClick.AddListener(EndDialogue);
            return;
        }
        _button.onClick.AddListener(SendValue);
    }

    private void EndDialogue()
    {
        DialogueManager.Instance.EndDialogue();
        DialogueLog.Instance.AddPhrase(_text,_sprite);
        DialogueUI.Instance.ClearAnswers();
    }

    private void SendValue()
    {
        DialogueManager.Instance.NextNode(_nextNode);
        ValueManager.Instance.AddValue(_value);
        DialogueLog.Instance.AddPhrase(_text,_sprite);
        DialogueUI.Instance.ClearAnswers();
    }
}

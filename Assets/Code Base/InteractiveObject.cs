using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InteractiveObject: MonoBehaviour
{
    [Header("Settings")]
    private readonly float  _timeBoxHide = 0.1f;
    private InteractiveBoxUI _interactiveBoxUI;

    protected virtual void Start()
    {
        _interactiveBoxUI = InteractiveBoxUI.Instance;
        _interactiveBoxUI.Disable();
    }

    public virtual void Hit()
    {
        StartCoroutine(ShowText());
    }
    
    protected IEnumerator ShowText()
    {
        _interactiveBoxUI.Enable();
        yield return new WaitForSeconds(_timeBoxHide);
        _interactiveBoxUI.Disable();
    }

    public virtual void Interact() { }

    public virtual void StayTrigger() { }
    
    public virtual void EnterTrigger() { }

    public virtual void ExitTrigger() { }
}
using UnityEngine;

public class ValueManager : SingletonBase<ValueManager>
{
    [SerializeField] private int value;

    private void Awake()
    {
        Init();
    }
    
    public void SetValue(int value)
    {
        this.value = value;
    }

    public int GetValue()
    {
        return this.value;
    }

    public void AddValue(int value)
    {
        this.value += value;
    }
}

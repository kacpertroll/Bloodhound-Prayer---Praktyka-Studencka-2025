using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    public int maxBlood = 100;
    public int CurrentBlood { get; private set; }

    private void Start()
    {
        CurrentBlood = maxBlood;
    }

    public void UseBlood(int amount)
    {
        CurrentBlood = Mathf.Max(CurrentBlood - amount, 0);
    }

    public void AddBlood(int amount)
    {
        CurrentBlood = Mathf.Min(CurrentBlood + amount, maxBlood);
    }
}

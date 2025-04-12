using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    public float attackSpeed = 1f;
    protected float lastAttackTime;

    public virtual void PrimaryAttack()
    {

    }

    public virtual void SecondaryAttack()
    {
        Debug.Log("Default Secondary Attack");
    }

    public virtual void OnEquipped()
    {
        // Zresetuj stan, za³aduj animacje, etc.
        Debug.Log($"{gameObject.name} equipped.");
    }
}

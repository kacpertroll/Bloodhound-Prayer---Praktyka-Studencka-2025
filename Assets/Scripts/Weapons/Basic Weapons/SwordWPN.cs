using UnityEngine;

public class SwordWPN : WeaponItem
{
    public float slashRange = 2f;
    public int damage = 10;
    public LayerMask targetLayers;

    public override void PrimaryAttack()
    {
        if (Time.time - lastAttackTime < 1f / attackSpeed)
            return;

        lastAttackTime = Time.time;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, slashRange, targetLayers))
        {
            Debug.Log($"Hit {hit.collider.name} with sword slash for {damage} damage.");
            // Mo¿na dodaæ interakcjê z HP itp.
        }
    }

    public override void SecondaryAttack()
    {
        Debug.Log("Sword special attack (blood ability) - placeholder.");
    }
}

using UnityEngine;

public class SwordWPN : WeaponItem
{
    public float slashRange = 2f;
    public int damage = 10;
    public LayerMask targetLayers;

    private Animator baseAnimator;

    private void Start()
    {
        baseAnimator = GetComponentInChildren<Animator>();
    }

    public override void PrimaryAttack()
    {
        if (Time.time - lastAttackTime < 1f / attackSpeed) // Blokada wykonywania ataku je¿eli nie min¹³ cooldown ataku. WA¯NE!!
            return;
        
        // Animacja ataku
        baseAnimator.SetBool("attack1", true);

        Debug.Log("Atak 1: " + baseAnimator.GetBool("attack1"));
        Debug.Log("Atak 2: " + baseAnimator.GetBool("attack2"));

        if (baseAnimator.GetBool("attack1") == true && baseAnimator.GetBool("attack2") == false)
        {
            baseAnimator.SetBool("attack2", true);
            baseAnimator.SetBool("attack1", false);
        }
        if (baseAnimator.GetBool("attack1") == false && baseAnimator.GetBool("attack2") == true)
        {
            baseAnimator.SetBool("attack2", false);
        }

        lastAttackTime = Time.time;
        DealDamage();
    }

    public override void SecondaryAttack()
    {
        Debug.Log("Sword special attack (blood ability) - placeholder.");
    }

    public void DealDamage()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, slashRange, targetLayers))
        {
            Debug.Log($"Hit {hit.collider.name} with sword slash for {damage} damage.");
            // Mo¿na dodaæ interakcjê z HP itp.
        }
    }

}

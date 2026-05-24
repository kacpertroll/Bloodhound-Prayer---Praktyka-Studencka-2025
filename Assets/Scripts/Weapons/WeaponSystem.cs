using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Header("Inventory")]
    public Transform weaponHolder;
    public WeaponItem equippedWeapon;
    public WeaponItem backupWeapon;

    [Header("Pickup")]
    public float pickupRange = 3f;
    public LayerMask weaponLayer;

    [Header("Key Binding")]
    public KeyCode pickupKey = KeyCode.F;
    public KeyCode swapKey = KeyCode.Q;

    private void Update()
    {
        HandlePickup();

        if (Input.GetMouseButtonDown(0) && equippedWeapon != null)
            equippedWeapon.PrimaryAttack();

        if (Input.GetMouseButtonDown(1) && equippedWeapon != null)
            equippedWeapon.SecondaryAttack();

        if (Input.GetKeyDown(swapKey))
            SwapWeapons();
    }

    void HandlePickup()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, weaponLayer))
            {
                WeaponItem foundWeapon = hit.collider.GetComponent<WeaponItem>();
                if (foundWeapon != null)
                {
                    PickupWeapon(foundWeapon);
                    if (foundWeapon.GetComponentInChildren<Animator>() != null)
                    foundWeapon.GetComponentInChildren<Animator>().SetTrigger("Picked Up");
                    foundWeapon.GetComponentInChildren<Animator>().SetBool("Holding", true);
                }
            }
        }
    }

    void PickupWeapon(WeaponItem newWeapon)
    {
        // Jeœli oba sloty s¹ zajête – wyrzuæ aktualnie trzyman¹ broñ
        if (equippedWeapon != null && backupWeapon != null)
        {
            DropWeapon(equippedWeapon); // wyrzucamy to, co by³o w d³oni
        }
        // Jeœli mamy tylko equippedWeapon, przenosimy j¹ do backup
        else if (equippedWeapon != null && backupWeapon == null)
        {
            backupWeapon = equippedWeapon;
            backupWeapon.gameObject.SetActive(false); // ukrywamy zapasow¹
        }

        // Nowa broñ trafia do slotu equipped
        equippedWeapon = newWeapon;
        equippedWeapon.transform.SetParent(weaponHolder);
        equippedWeapon.transform.localPosition = Vector3.zero;
        equippedWeapon.transform.localRotation = Quaternion.identity;

        Rigidbody rb = equippedWeapon.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Collider col = equippedWeapon.GetComponent<Collider>();
        if (col) col.enabled = false;

        equippedWeapon.gameObject.SetActive(true);
        equippedWeapon.OnEquipped();
    }

    void DropWeapon(WeaponItem weapon)
    {
        weapon.transform.SetParent(null);
        weapon.gameObject.SetActive(true);
        weapon.GetComponentInChildren<Animator>().SetBool("Holding", false);

        Rigidbody rb = weapon.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero; // reset prêdkoœci
            rb.angularVelocity = Vector3.zero; // reset obrotu

            // Rzut w przód
            Vector3 throwDirection = Camera.main.transform.forward;
            float throwForce = 5f; // mo¿esz dostosowaæ si³ê
            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }

        Collider col = weapon.GetComponent<Collider>();
        if (col) col.enabled = true;
    }

    void SwapWeapons()
    {
        if (backupWeapon == null || equippedWeapon == null)
            return;

        WeaponItem temp = equippedWeapon;

        // Ukryj aktualnie wyposa¿on¹ broñ
        temp.gameObject.SetActive(false);
        temp.transform.SetParent(null);

        // Przenieœ zapasow¹ broñ na slot g³ówny
        equippedWeapon = backupWeapon;
        backupWeapon = temp;

        equippedWeapon.transform.SetParent(weaponHolder);
        equippedWeapon.transform.localPosition = Vector3.zero;
        equippedWeapon.transform.localRotation = Quaternion.identity;

        Rigidbody rb = equippedWeapon.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Collider col = equippedWeapon.GetComponent<Collider>();
        if (col) col.enabled = false;

        equippedWeapon.gameObject.SetActive(true); // <- poka¿ now¹
        equippedWeapon.OnEquipped();
        equippedWeapon.GetComponentInChildren<Animator>().SetTrigger("Picked Up");
        equippedWeapon.GetComponentInChildren<Animator>().SetBool("Holding", true);
    }
}

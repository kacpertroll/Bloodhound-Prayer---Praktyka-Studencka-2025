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
    public KeyCode pickupKey = KeyCode.E;

    private void Update()
    {
        HandlePickup();

        if (Input.GetMouseButtonDown(0) && equippedWeapon != null)
            equippedWeapon.PrimaryAttack();

        if (Input.GetMouseButtonDown(1) && equippedWeapon != null)
            equippedWeapon.SecondaryAttack();

        if (Input.GetKeyDown(KeyCode.Q))
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
                }
            }
        }
    }

    void PickupWeapon(WeaponItem newWeapon)
    {
        if (equippedWeapon != null)
        {
            if (backupWeapon != null)
            {
                DropWeapon(backupWeapon);
            }
            backupWeapon = equippedWeapon;
        }

        equippedWeapon = newWeapon;
        equippedWeapon.transform.SetParent(weaponHolder);
        equippedWeapon.transform.localPosition = Vector3.zero;
        equippedWeapon.transform.localRotation = Quaternion.identity;

        Rigidbody rb = equippedWeapon.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Collider col = equippedWeapon.GetComponent<Collider>();
        if (col) col.enabled = false;

        equippedWeapon.OnEquipped();
    }

    void DropWeapon(WeaponItem weapon)
    {
        weapon.transform.SetParent(null);

        Rigidbody rb = weapon.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;

        Collider col = weapon.GetComponent<Collider>();
        if (col) col.enabled = true;
    }

    void SwapWeapons()
    {
        if (backupWeapon == null || equippedWeapon == null)
            return;

        WeaponItem temp = equippedWeapon;
        DropWeapon(temp);

        equippedWeapon = backupWeapon;
        backupWeapon = temp;

        equippedWeapon.transform.SetParent(weaponHolder);
        equippedWeapon.transform.localPosition = Vector3.zero;
        equippedWeapon.transform.localRotation = Quaternion.identity;

        Rigidbody rb = equippedWeapon.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Collider col = equippedWeapon.GetComponent<Collider>();
        if (col) col.enabled = false;

        equippedWeapon.OnEquipped();
    }
}

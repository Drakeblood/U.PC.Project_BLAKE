using System.Xml.Serialization;
using UnityEngine;

public class WeaponPickup : Interactable
{
    public WeaponDefinition WeaponDefinition;

    [SerializeField] 
    private GameObject pickupGameObject;

    [SerializeField] 
    private float rotateForce = 12f;

    [HideInInspector]
    public WeaponInstanceInfo WeaponInstanceInfo;

    private GameObject weaponGFX;

    private void Start()
    {
        weaponGFX = Instantiate(WeaponDefinition.WeaponGFX, pickupGameObject.transform.position, pickupGameObject.transform.rotation, pickupGameObject.transform);
    }

    private void Update()
    {
        pickupGameObject.transform.Rotate(Vector3.up * Time.deltaTime * rotateForce);
    }

    public override void Interact(GameObject interacter)
    {
        WeaponsManager weaponsManager = interacter.GetComponent<WeaponsManager>();
        if(weaponsManager == null)
        {
            Debug.LogWarning("WeaponsManager is not valid");
            return;
        }

        int index = weaponsManager.ActiveWeaponIndex == 0 ? 1 : weaponsManager.ActiveWeaponIndex;

        WeaponDefinition weaponDefinition = null;
        if(weaponsManager.Weapons[index] != null)
        {
            weaponDefinition = weaponsManager.Weapons[index].WeaponDefinition;
        }

        WeaponInstanceInfo weaponInstanceInfoToSave = null;
        Weapon oldWeapon = weaponsManager.Weapons[index];
        if (oldWeapon != null)
        {
            weaponInstanceInfoToSave = oldWeapon.GenerateWeaponInstanceInfo();
        }

        weaponsManager.Equip(WeaponDefinition, index);
        if(weaponsManager.ActiveWeaponIndex == 0) weaponsManager.SetActiveIndex(index);

        if (WeaponInstanceInfo != null)
        {
            weaponsManager.Weapons[index].LoadWeaponInstanceInfo(WeaponInstanceInfo);
        }

        WeaponInstanceInfo = weaponInstanceInfoToSave;
        weaponsManager.OnPlayerPickupWeapon();

        if (weaponDefinition != null)
        {
            WeaponDefinition = weaponDefinition;
            ChangeVisuals(weaponDefinition);
            return;
        }

        PlayerInteractables playerInteractables = interacter.GetComponent<PlayerInteractables>();
        if (playerInteractables != null)
        {
            playerInteractables.RemoveInteractable(this);
        }
        Destroy(gameObject);
    }

    private void ChangeVisuals(WeaponDefinition newWeapon)
    {
        if (weaponGFX != null)
        {
            Destroy(weaponGFX);
        }

        weaponGFX = Instantiate(newWeapon.WeaponGFX, pickupGameObject.transform);
        weaponGFX.transform.localPosition = newWeapon.PickupLocationOffset;
        weaponGFX.transform.localRotation = newWeapon.PickupRotation;
    }
}

public class WeaponInstanceInfo
{

}

public class RangedWeaponInstanceInfo : WeaponInstanceInfo
{
    public int bulletsLeft;
}
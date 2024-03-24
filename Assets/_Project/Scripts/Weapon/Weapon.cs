using MBT;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField]
    public WeaponDefinition WeaponDefinition;

    //BlakeCharacter
    private GameObject owner;
    public GameObject Owner
    {
        get => owner;
        set
        {
            if (owner == value) return;
            owner = value;
            OnOwnerChanged();
        }
    }

    protected AudioSource audioSource;
    protected WeaponsManager weaponsManager;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public abstract void PrimaryAttack();

    public virtual void OnOwnerChanged()
    {
        if (Owner == null) { return; }

        weaponsManager = Owner.GetComponent<WeaponsManager>();
    }

    public virtual bool CanPrimaryAttack() => true;

    public virtual bool CanAttack()
    {

        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(owner.transform.position);
        if (viewportPosition.x <= 0 || viewportPosition.x >= 1 || viewportPosition.y <= 0 || viewportPosition.y >= 1) return false;

        return true;
    }

    public virtual WeaponInstanceInfo GenerateWeaponInstanceInfo(bool randomize = false) => new WeaponInstanceInfo();
    public abstract void LoadWeaponInstanceInfo(WeaponInstanceInfo weaponInstanceInfo);
}

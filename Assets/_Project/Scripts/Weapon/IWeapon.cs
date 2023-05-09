using UnityEngine;

public interface IWeapon
{
    public bool PrimaryAttack();
    public void SetAmmo(int newAmmo);
    public GameObject GetGameObject();

    public float GetWeaponRange();

    public float GetWeaponFireRate();
}

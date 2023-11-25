using GameFramework.Abilities;
using System.Collections;
using UnityEngine;

public class A_WeaponFire : Ability
{
    private RangedWeapon weaponSource;

    public override bool CanActivateAbility()
    {
        if (weaponSource == null)
        {
            weaponSource = SourceObject as RangedWeapon;
            if (weaponSource == null) { return false; }
        }
        return weaponSource.CanPrimaryAttack() && base.CanActivateAbility();
    }

    public override void ActivateAbility()
    {
        base.ActivateAbility();

        weaponSource.PrimaryAttack();

        StartCoroutine(EndAbilityCoroutine());
    }

    public IEnumerator EndAbilityCoroutine()
    {
        yield return new WaitForSeconds(weaponSource.FireDelayTime);
        EndAbility();
    }
}

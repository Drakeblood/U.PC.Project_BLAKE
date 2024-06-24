using _Project.Scripts.Weapon;
using UnityEngine;

namespace GameFramework.Abilities
{
    public class WeaponAbility : Ability
    {
        protected Weapon weaponSource;

        public override bool CanActivateAbility()
        {
            if (weaponSource == null)
            {
                weaponSource = SourceObject as Weapon;
                if (weaponSource == null)
                {
                    return false; 
                }
            }
            return weaponSource.CanPrimaryAttack() && base.CanActivateAbility() && weaponSource.Owner.IsAlive;
        }

        public sealed override void ActivateAbility()
        {
            base.ActivateAbility();

            AbilitySkill();

            EndAbility();
        }

        protected virtual void AbilitySkill()
        {
            Debug.LogError("Method is not overriden, using PrimaryAttack on weapon");
            weaponSource.PrimaryAttack();
        }
    }
}
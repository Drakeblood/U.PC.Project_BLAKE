using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Services/Shoot")]
public class BTS_Shoot : Service
{
    public BoolReference HasLineOfSightReference;
    public AIController AIController;

    public override void Task()
    {
        if (AIController == null) return;
        if (AIController.Weapon == null) return;
        if (!AIController.Weapon.CanAttack()) return;
        if (!AIController.Weapon.CanPrimaryAttack()) return;
        if (!HasLineOfSightReference.Value) return;
        AIController.Weapon.PrimaryAttack();
    }
}

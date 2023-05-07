using UnityEngine;
using Random = UnityEngine.Random;

public class SniperAttack : MonoBehaviour, IAttack
{
    [Tooltip("Declares how often player can shoot")]
    [SerializeField] private float timeBetweenShooting;
    [Tooltip("Declares range of bullet spawn, while player is moving")]
    [SerializeField] private float spread;
    private Weapon usedWeapon;
    private Rigidbody _ownerRigidbodyRef;

    //TODO: Change this if we'll have reference manager
    
    public bool Attack(Weapon weapon)
    {
        usedWeapon = weapon;
        _ownerRigidbodyRef = usedWeapon.GetRigidbodyOfWeaponOwner();
        return Shot();
    }
    
    private bool Shot()
    {
        usedWeapon.isLastShotOver = false;
        usedWeapon.As.PlayOneShot(usedWeapon.As.clip);
        
        //Choose spread depending on player's controls
        float chosenSpread = _ownerRigidbodyRef.velocity == Vector3.zero ? 0 : Random.Range(-spread, spread);
        
        Instantiate(usedWeapon.BulletPrefab, usedWeapon.BulletsSpawnPoint.position, usedWeapon.transform.rotation).GetComponent<IBullet>().SetupBullet(chosenSpread, usedWeapon.transform.parent.gameObject, usedWeapon.Range);
        
        if (!usedWeapon.infinityAmmo) usedWeapon.BulletsLeft--;
        usedWeapon.Invoke(nameof(usedWeapon.ResetShot), timeBetweenShooting);
        return true;
    }

    public float ReturnFireRate()
    {
        return timeBetweenShooting;
    }
}

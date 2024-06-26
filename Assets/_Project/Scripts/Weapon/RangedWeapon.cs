using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Weapon
{
    public struct RangedWeaponStatistics
    {
        public RangedWeaponStatistics(BulletType bulletType, SpreadType spreadType, float spread, float spreadResetThreshold, int projectilesPerShot, float range, float fireDelayTime)
        {
            BulletType = bulletType;
            SpreadType = spreadType;
            Spread = spread;
            SpreadResetThreshold = spreadResetThreshold;
            ProjectilesPerShot = projectilesPerShot;
            Range = range;
            FireDelayTime = fireDelayTime;
        }

        public BulletType BulletType;
        public SpreadType SpreadType;
        public float Spread;
        public float SpreadResetThreshold;
        public int ProjectilesPerShot;
        public float Range;
        public float FireDelayTime;
    }
    
    [RequireComponent(typeof(AudioSource))]
    public class RangedWeapon : Weapon
    {
        [SerializeField]
        private Transform bulletsSpawnPoint;
        [SerializeField]
        private bool infinityAmmo = false;

        private RangedWeaponStatistics savedRangedWeaponStatistics;
        private RangedWeaponDefinition rangedWeaponDefinition;
        private float fireDelayTime;
        private BulletType bulletType;
        private SpreadType spreadType;
        private float spread;
        private float spreadStep;
        private float spreadThreshold;
        private float spreadResetThreshold;
        private int projectilesPerShot;
        private int magazineSize;
        private float range;
        
        private float lastFireTime;
        private float negativeSpreadThreshold;
        private float positiveSpreadThreshold;
        private float currentSpread;
        
        public float Range => range;
        public int BulletsLeft { get; set; }
        public SpreadType CurrentSpreadType { get; set; }

        protected override void Awake()
        {
            base.Awake();

            SetupWeaponDefinition();
        }

        public override void PrimaryAttack()
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(audioSource.clip);
        
            Shot();

            lastFireTime = Time.time;

            weaponsManager?.BroadcastOnPrimaryAttack();
        }

        public override bool CanPrimaryAttack()
        {
            if(BulletsLeft <= 0)
            {
                StartCoroutine("UnequipSelf");
                return false;
            }
            
            if (Time.time - lastFireTime < fireDelayTime) return false;

            return true;
        }

        private IEnumerator UnequipSelf()
        {
            yield return new WaitForEndOfFrame();

            if (Owner.TryGetComponent(out WeaponsManager weaponsManager))
            {
                weaponsManager.Unequip(weaponsManager.ActiveWeaponIndex);
                weaponsManager.SetActiveIndex(weaponsManager.ActiveWeaponIndex - 1);
            }
        }

        private bool Shot()
        {
            if (BulletsLeft == 0) return false;

            var list = GetCalculatedProjectilesAngles();
            for (var index = 0; index < list.Count; index++)
            {
                var bulletSpreadValue = list[index];

                if (spreadType == SpreadType.StaticSeries)
                {
                    DOVirtual.DelayedCall(index / 10f, () => CreateBullet(bulletSpreadValue));
                }
                else
                {
                    CreateBullet(bulletSpreadValue);
                }
            }

            if (!infinityAmmo) BulletsLeft--;

            return true;
        }

        private void CreateBullet(float bulletSpreadValue)
        {
            //TODO: Add pooling
            var bulletPrefab = rangedWeaponDefinition.BasicBullet;
            var bullet = Instantiate(bulletPrefab, bulletsSpawnPoint.position, transform.rotation);
                
            bullet.SetupBullet(bulletSpreadValue, transform.parent.gameObject, range, bulletType);
        }

        private List<float> GetCalculatedProjectilesAngles()
        {
            var projectilesAngles = new List<float>();
            
            if (spreadType == SpreadType.Undefined)
            {
                Debug.LogError($"SpreadType in {rangedWeaponDefinition.WeaponName} is Undefined!");
            }

            else if (spreadType == SpreadType.NoSpread)
            {
                projectilesAngles.Add(0f);
            }
            
            else if (spreadType is SpreadType.Static)
            {
                var projectileAngle = Random.Range(negativeSpreadThreshold, positiveSpreadThreshold);
                projectilesAngles.Add(projectileAngle);
            }
            
            else if (spreadType is SpreadType.StaticSeries)
            {
                for (var i = 0; i < projectilesPerShot; i++)
                {
                    var projectileAngle = Random.Range(negativeSpreadThreshold, positiveSpreadThreshold);
                    projectilesAngles.Add(projectileAngle);
                }
            }

            else if (spreadType == SpreadType.StaticMultiShot)
            {
                var anglePerProjectile = spread / (projectilesPerShot - 1);

                for (var i = 0; i < projectilesPerShot; i++)
                {
                    var projectileAngle = negativeSpreadThreshold + anglePerProjectile * i;
                    projectilesAngles.Add(projectileAngle);
                }
            }

            else if (spreadType == SpreadType.NoSpreadThenStatic)
            {
                if (Time.time - lastFireTime < spreadResetThreshold)
                { 
                    var projectileAngle = Random.Range(negativeSpreadThreshold, positiveSpreadThreshold);
                    projectilesAngles.Add(projectileAngle);
                }
                else
                {
                    projectilesAngles.Add(0f);
                }
            }

            else if (spreadType == SpreadType.GraduallyIncrease)
            {
                if (Time.time - lastFireTime > fireDelayTime + spreadResetThreshold)
                {
                    ResetSpread();
                }
                
                var projectileAngle = Random.Range(negativeSpreadThreshold, positiveSpreadThreshold);
                projectilesAngles.Add(projectileAngle);
                Debug.Log($"{currentSpread}, ");
                
                if (currentSpread < spreadThreshold)
                {
                    currentSpread += spreadStep;
                    UpdateSpreadThresholds();
                }
            }

            return projectilesAngles;
        }

        public override WeaponInstanceInfo GenerateWeaponInstanceInfo(bool randomize)
        {
            if(randomize)
            {
                return new RangedWeaponInstanceInfo
                {
                    bulletsLeft = Random.Range(magazineSize / 2, magazineSize + 1)
                };
            }

            return new RangedWeaponInstanceInfo
            {
                bulletsLeft = BulletsLeft
            };
        }

        private void SetupWeaponDefinition()
        {
            if (WeaponDefinition is not RangedWeaponDefinition definition)
            {
                Debug.LogError("Wrong WeaponDefinition is attached to the weapon!");
                return;
            }
            
            rangedWeaponDefinition = definition;
            fireDelayTime = rangedWeaponDefinition.FireDelayTime;
            bulletType = rangedWeaponDefinition.BulletType;
            spreadType = rangedWeaponDefinition.SpreadType;
            spread = rangedWeaponDefinition.Spread;
            spreadStep = rangedWeaponDefinition.SpreadStep;
            spreadThreshold = rangedWeaponDefinition.SpreadThreshold;
            spreadResetThreshold = rangedWeaponDefinition.SpreadResetThreshold;
            projectilesPerShot = rangedWeaponDefinition.ProjectilesPerShot;
            magazineSize = rangedWeaponDefinition.MagazineSize;
            range = rangedWeaponDefinition.Range;
            BulletsLeft = magazineSize;
            
            ResetSpread();
        }

        public RangedWeaponStatistics SaveAndGetRangedWeaponStatistics()
        {
            return savedRangedWeaponStatistics = new RangedWeaponStatistics(bulletType, spreadType, spread, spreadResetThreshold, projectilesPerShot, range, fireDelayTime);
        }

        public void ApplyRangedWeaponStatistics(RangedWeaponStatistics rangedWeaponStatistics)
        {
            bulletType = rangedWeaponStatistics.BulletType;
            spreadType = rangedWeaponStatistics.SpreadType;
            spread = rangedWeaponStatistics.Spread;
            spreadResetThreshold = rangedWeaponStatistics.SpreadResetThreshold;
            projectilesPerShot = rangedWeaponStatistics.ProjectilesPerShot;
            range = rangedWeaponStatistics.Range;
            fireDelayTime = rangedWeaponStatistics.FireDelayTime;
            lastFireTime = Time.time;

            ResetSpread();
        }

        public void RestoreRangedWeaponStatistics()
        {
            ApplyRangedWeaponStatistics(savedRangedWeaponStatistics);
        }

        private void ResetSpread()
        {
            currentSpread = spread;
            UpdateSpreadThresholds();
        }

        private void UpdateSpreadThresholds()
        {
            negativeSpreadThreshold = -(currentSpread / 2);
            positiveSpreadThreshold = currentSpread / 2;
        }

        public override void LoadWeaponInstanceInfo(WeaponInstanceInfo weaponInstanceInfo)
        {
            RangedWeaponInstanceInfo rangedWeaponInstanceInfo = weaponInstanceInfo as RangedWeaponInstanceInfo;
            if (rangedWeaponInstanceInfo != null)
            {
                BulletsLeft = rangedWeaponInstanceInfo.bulletsLeft;
            }
        }

#if UNITY_EDITOR
        public void SetInfiniteAmmo(bool setInfinite)
        {
            infinityAmmo = setInfinite;
        }
#endif
    
    }
}

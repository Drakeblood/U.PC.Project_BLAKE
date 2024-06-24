using System;
using System.Collections;
using _Project.Scripts;
using _Project.Scripts.Interfaces;
using UnityEngine;

public abstract class BlakeCharacter : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected int health = 1;

    [SerializeField] 
    protected float timeBetweenDamages = .5f;

    public int Health
    {
        get => health;
        protected set { health = value; }
    }

    public int RespawnsLeft => maxRespawns - respawnCounter;

    public bool IsAlive => !isDead;

    private bool hasShield = false;

    protected int respawnCounter = 0;
    [SerializeField]
    protected int maxRespawns = 3;

    [SerializeField] 
    protected GameObject explosionParticle;

    [SerializeField] 
    protected Animator animator;

#if UNITY_EDITOR
    [SerializeField]
    private bool godMode;
#endif

    protected GameObject explosionParticleInstantiated;
    protected int defaultHealth;
    protected bool recentlyDamaged = false;
    protected bool isDead = false;
    protected Vector3 respawnPos;

    public delegate void OnDeath(BlakeCharacter blakeCharacter);
    public event OnDeath onDeath;
    public delegate void OnRespawn();
    public event OnRespawn onRespawn;
    public event Action<GameObject> OnDamageTaken;

    public virtual void Die(GameObject killer)
    {
        isDead = true;
        if (respawnCounter >= maxRespawns)
        {
            ReferenceManager.LevelHandler.EndRun();
            return;
        }
        
        respawnCounter++;
        onDeath?.Invoke(this);
    }

    public virtual bool TryTakeDamage(GameObject instigator, int damage)
    {
#if UNITY_EDITOR
        if (godMode) return false;
#endif
        if (recentlyDamaged) return false;
        if (health < 1) return false;
        if(!CanTakeDamage(instigator)) return false;
        
        if(hasShield)
        {
            DeactivateShield();
            return false;
        }
        Debug.Log(instigator.name + " dealt " + damage + " damage to " + name);
        Health -= damage;

        if (health > 0)
        {
            StartCoroutine(StopTakingDamageForPeriod(timeBetweenDamages));
        }
        else if (!isDead)
        {
            Die(instigator);
        }

        OnDamageTaken?.Invoke(instigator);
        return true;
    }

    public virtual bool CanTakeDamage(GameObject instigator)
    {
        if (instigator == null) return false;

        BlakeCharacter other = instigator.GetComponent<BlakeCharacter>();
        if(other != null)
        {
            return GetType() != other.GetType();
        }

        return true;
    }

    public void AddRespawnCounter()
    {
        maxRespawns++;
    }

    public void ActivateShield()
    {
        hasShield = true;
    }

    public void DeactivateShield()
    {
        hasShield = false;
    }

    public void SetRespawnPosition(Vector3 position)
    {
        respawnPos = position;
    }

    public Vector3 GetRespawnPosition()
    {
        return respawnPos;
    }

    private IEnumerator StopTakingDamageForPeriod(float period)
    {
        recentlyDamaged = true;
        yield return new WaitForSeconds(period);
        recentlyDamaged = false;
    }

    protected void Respawn()
    {
        isDead = false;
        Destroy(explosionParticleInstantiated);
        transform.position = respawnPos;
        gameObject.SetActive(true);
        GetComponent<CapsuleCollider>().enabled = true;
        health = defaultHealth;
        onRespawn?.Invoke();
    }

#if UNITY_EDITOR
    public void SetGodMode(bool isEnabled)
    {
        godMode = isEnabled;
    }
#endif
}

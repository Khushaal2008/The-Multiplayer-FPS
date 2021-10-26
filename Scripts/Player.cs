using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get{return _isDead;}
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    

    [SyncVar]
    private int currentHealth;

    public float GetHealthPct()
    {
        return (float)currentHealth/maxHealth;
    }

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;

        [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;
    
    public void SetupPlayer()
    {
       if(isLocalPlayer)
       {
           GameManager.instance.SetSceneCameraActive(false);
        GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
       }
        
        CmdBroadCastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if(firstSetup)
        {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }

        firstSetup = false;
        }
        

        SetDefaults();
    }

   /* void Update()
    {
        if(!isLocalPlayer)
        return;

        if(Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(9999);
        }
    }*/

    [ClientRpc]
    public void RpcTakeDamage(int _amount)
    {
        if(isDead)
        return;

        currentHealth -= _amount;
        
        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if(currentHealth <= 0 )
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        Debug.Log(transform.name + " is Dead !");

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        for (int i = 0; i < disableObjectsOnDeath.Length; i++)
        {
            disableObjectsOnDeath[i].SetActive(false);
        }

          Collider _col = GetComponent<Collider>();
        if(_col != null)
        _col.enabled = false;

        GameObject _gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns,3f);

        if(isLocalPlayer)
        {
          GameManager.instance.SetSceneCameraActive(true);
          GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;
        if(isLocalPlayer)

        yield return new WaitForSeconds(0.1f);
        SetupPlayer();
        Debug.Log(transform.name + " has respawned");
    }

    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        for (int i = 0; i < disableObjectsOnDeath.Length; i++)
        {
            disableObjectsOnDeath[i].SetActive(true);
        }

        Collider _col = GetComponent<Collider>();
        if(_col != null)
        _col.enabled = true;


        

            GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns,3f);

    }

}

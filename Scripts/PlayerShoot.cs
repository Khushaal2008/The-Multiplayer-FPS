using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";
   


    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    private bool isPressed = false;

    void Start()
    {
        if(cam == null)
        {
            Debug.LogError("No Camera referenced!");
            this.enabled = false;
        }
        weaponManager = GetComponent<WeaponManager>();
    }

    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if(PauseMenu.IsOn)
        return;

        if(currentWeapon.bullets < currentWeapon.maxBullets)
        {
            if(Input.GetKeyDown(KeyCode.R))
        {
            weaponManager.Reload();
            return;
        }
        }

        

        if(currentWeapon.fireRate <= 0)
        {
            if(Input.GetButtonDown("Fire1"))
            {
                
                    Shoot();
                
                
                
            }
        }
        else
        {
            if(Input.GetButtonDown("Fire1"))
            {
            InvokeRepeating("Shoot",0f,1f/currentWeapon.fireRate);
            }
             else 
             if(Input.GetButtonUp("Fire1"))
                {
                     CancelInvoke("Shoot");
                }
        }
        
    }

    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcDoHitEffect(_pos,_normal);
    }

    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
        GameObject _hitEffect = (GameObject)Instantiate (weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos,Quaternion.LookRotation(_normal));
        Destroy(_hitEffect,0.5f);
    }


    [Client]
    public void Shoot()
    {
        isPressed = true;
        Debug.Log("SHOOT!");
        if(!isLocalPlayer || weaponManager.isReloading)
        {
            return;
        }
        

        if(currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
            return;
        }

        currentWeapon.bullets --;

        Debug.Log("Remaining bullets: " + currentWeapon.bullets);
        CmdOnShoot();

        RaycastHit hit;

        if(Physics.Raycast(cam.transform.position,cam.transform.forward,out hit, currentWeapon.range,mask))
        {
            if(hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(hit.collider.name, currentWeapon.damage);
            }
            CmdOnHit(hit.point, hit.normal);
        }
        

        if(currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
        }
    }

    [Command]
    void CmdPlayerShot(string _playerID, int _damage)
    {
        Debug.Log(_playerID + " Has been shot.");
        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);

    }
}

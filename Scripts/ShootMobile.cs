using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class ShootMobile : NetworkBehaviour
{

     private const string PLAYER_TAG = "Player";
   


    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    private bool isPressed = false;


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
        if(!isLocalPlayer)
        {
            return;
        }

        

        if(isPressed == true)
        {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour
{
     [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    [SerializeField]
    private Transform weaponHolder;

    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;

    public bool isReloading = false;
    
    void Start()
    {
        EquipWeapon(primaryWeapon);
        
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }


    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }


    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;
        GameObject _weaponIns = (GameObject)Instantiate(_weapon.graphics,weaponHolder.position,weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);

        currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();
        if(currentGraphics == null)
        {
            Debug.Log("No weapon graphics component on the weapon object" + _weaponIns.name);
        }

        if(isLocalPlayer)
        Util.SetLayerRecursively(_weaponIns,LayerMask.NameToLayer(weaponLayerName));
    }

    public void Reload()
    {
        if(isReloading)
        return;

        StartCoroutine(Reload_Coroutine());
    }

    private IEnumerator Reload_Coroutine()
    {
        Debug.Log("Reloading...");

        isReloading = true;

        CmdOnReload();

        yield return new WaitForSeconds(currentWeapon.reloadTime);

        currentWeapon.bullets = currentWeapon.maxBullets;

        isReloading = false;
    }

    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {
        Animator anim = currentGraphics.GetComponent<Animator>();
        if(anim != null)
        {
            anim.SetTrigger("Reload");
        }
    }

    
}

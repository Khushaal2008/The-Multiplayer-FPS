using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";
    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;
    
    void Start()
    {
        if(!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            

            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if(ui == null)
                Debug.LogError("Error!!!");
             ui.SetPlayer(GetComponent<Player>());
            
            GetComponent<Player>().SetupPlayer();
        }

       

    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
   
    public override void OnStartClient()
    {
        base.OnStartClient();

    string _netID = GetComponent<NetworkIdentity>().netId.ToString();
    Player _player = GetComponent<Player>();

        GameManager.RegisterPlayer(_netID,_player);
    }


    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
    }

    void OnDisable()
    {

        Destroy(playerUIInstance);
    if(isLocalPlayer)
        GameManager.instance.SetSceneCameraActive(true);

        GameManager.UnRegisterPlayer(transform.name);
    }
}

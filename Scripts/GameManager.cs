using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MatchSettings matchSettings;

    [SerializeField]
    private GameObject sceneCamera;

    void Awake ()
    {
        if(instance != null)
        {
            Debug.Log("More than one GameManager in this scene.");
        } else
        {
            instance = this;
        }
       
    }

    public void SetSceneCameraActive(bool isActive)
    {
        if(sceneCamera == null)
        return;

        sceneCamera.SetActive(isActive);
    }

    #region Player tracking

    private const string PLAYER_ID_PREFIX = "Player";
   
   private static Dictionary<string,Player> players = new Dictionary<string,Player>();
    public static void RegisterPlayer(string _netID, Player _player)
    {
        string _PlayerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_PlayerID,_player);
        _player.transform.name = _PlayerID;
    }

    public static void UnRegisterPlayer(string _PlayerID)
    {
        players.Remove(_PlayerID);
    }

    public static Player GetPlayer(string _playerID)
    {
        return players[_playerID];
    }

    /*void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200,200,200,500));
        GUILayout.BeginVertical();

        foreach (string _playerID in players.Keys)
        {
    GUILayout.Label(_playerID + " - " + players[_playerID].transform.name);            
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }*/

    #endregion
   
}

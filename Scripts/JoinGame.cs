using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour
{
    private NetworkManager networkManager;

    [SerializeField]
    private Text status;
[SerializeField]
private Transform roomListParent;
    [SerializeField]
    private GameObject roomListItemPrefab;

    List<GameObject> roomList = new List<GameObject>();

    void Start()
    {
        networkManager = NetworkManager.singleton;
        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();

    }

    public void RefreshRoomList()
    {
        ClearRoomList();
        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
    status.text = "Loading...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
{
    status.text = "";
    if (!success || matchList == null)
    {
        status.text = "Couldn't get room list.";
        return;
    }

    
    foreach(MatchInfoSnapshot match in matchList)
    {
        GameObject _roomListItemGO = Instantiate(roomListItemPrefab);
        _roomListItemGO.transform.SetParent(roomListParent);
        

        RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();
        if(_roomListItem != null)
        {
            _roomListItem.Setup(match, JoinRoom);
        }

        // that will take care of setting up the name / amount of users.
        // as well as setting up a callback function that will join the game.

        roomList.Add(_roomListItemGO);
    }

        if(roomList.Count == 0)
        {
            status.text = "No Room Available At Present.";
        }

    }

    void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot _match)
    {
       // Debug.Log("Joining " + _match.name);
       networkManager.matchMaker.JoinMatch(_match.networkId,"","","",0,0,networkManager.OnMatchJoined);
       ClearRoomList();
       status.text = "joining...";
    }

}

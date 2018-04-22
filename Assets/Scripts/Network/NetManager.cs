
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : NetworkManager
{

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("Client has connected to server: IP: {0}, connection ID: {1}. Welcome!".Form(conn.address, conn.connectionId));

    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var go = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        // Setup player. For now just give them a faction and a name based on their player number.
        Player player = go.GetComponent<Player>();
        player.Name = "Player #" + Player.All.Count;
        player.Faction = (Faction)(byte)Player.All.Count; // There can be no more than (FactionNumber) clients that join the game.

        NetworkServer.AddPlayerForConnection(conn, go, playerControllerId);
        //Debug.Log("Spawned player GO for client on connection {0} with controller ID {1}".Form(conn.address, playerControllerId));
    }

}
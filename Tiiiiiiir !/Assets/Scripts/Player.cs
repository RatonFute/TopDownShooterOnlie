using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private Vector3 _movement = new Vector3 (0, 0, 0);

    [Client]
    private void Update()
    {
        if (!authority) { }
        if (!Input.GetKeyDown(KeyCode.A)) return;
        //transform.Translate(_movement);
        CmdMove();
    }

    [Command]
    private void CmdMove()
    {

        RpcMove();
    }

    [ClientRpc]
    private void RpcMove()
    {
        transform.Translate(_movement);
    }

}

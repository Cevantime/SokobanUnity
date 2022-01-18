using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveBlock : MonoBehaviour
{
    public delegate void BlockReceivedDel();

    public BlockReceivedDel onBlockReceived;

    public delegate void BlockExitedDel();

    public BlockExitedDel onBlockExited;
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Block"))
        {
            if (onBlockReceived != null)
            {
                onBlockReceived();
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Block"))
        {
            if (onBlockExited != null)
            {
                onBlockExited();
            }
        }
    }
}

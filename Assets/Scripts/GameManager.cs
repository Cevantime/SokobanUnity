using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int slotCount;
    private int blockedPlacedCount = 0;
    private Stack<Move> moves = new Stack<Move>();
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] slots = GameObject.FindGameObjectsWithTag("Slot");
        slotCount = slots.Length;
        foreach (GameObject slot in slots)
        {
            ReceiveBlock receiveBlock = slot.GetComponent<ReceiveBlock>();
            receiveBlock.onBlockReceived += OnBlockPlaced;
            receiveBlock.onBlockExited += OnBlockExited;
        }

        foreach (GameObject block in GameObject.FindGameObjectsWithTag("Block"))
        {
            Displaceable displaceable = block.GetComponent<Displaceable>();
            displaceable.onDisplacementStarted += OnBlockDisplaced;
        }
        player = GameObject.Find("Player");
    }

    private void OnBlockPlaced()
    {
        blockedPlacedCount++;
        if (blockedPlacedCount >= slotCount)
        {
            Debug.Log("Win");
        }
    }

    private void OnBlockExited()
    {
        blockedPlacedCount--;
    }

    private void OnBlockDisplaced(GameObject block, Vector3 from, Vector3 to)
    {
        moves.Push(new Move(block, from, to));
    }

    public void Undo()
    {
        if (moves.Count == 0)
        {
            return;
        }

        Move m = moves.Pop();
        Vector3 diff = m.from - m.to;
        Vector3 posPlayerTo = m.from + diff;
        posPlayerTo.y = player.transform.position.y;
        player.GetComponent<PlayerController>().Displace(posPlayerTo);
        m.block.GetComponent<Displaceable>().DisplaceTo(m.from);
        moves.Pop();
    }
}

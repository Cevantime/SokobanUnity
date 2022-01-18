using UnityEngine;
public struct Move
{
    public Vector3 from, to;
    public GameObject block;

    public Move(GameObject block, Vector3 from, Vector3 to)
    {
        this.block = block;
        this.from = from;
        this.to = to;
    }
}
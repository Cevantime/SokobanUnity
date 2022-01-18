using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Displaceable : MonoBehaviour
{
    public float speed = 10;
    private Vector3 target;
    private bool isDisplaced = false;

    public delegate void DisplacementFinishedDel();

    public DisplacementFinishedDel onDisplacementFinished;

    public delegate void DisplacementStartedDel(GameObject self, Vector3 from, Vector3 to);
    public DisplacementStartedDel onDisplacementStarted;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isDisplaced)
        {
            Vector3 diff = target - transform.position;
            Vector3 dir = new Vector3(diff.x, 0, diff.z).normalized;
            float distance = diff.magnitude;
            float displacement = Time.deltaTime * speed;

            if (distance < displacement)
            {
                transform.position = target;
                isDisplaced = false;
                if (onDisplacementFinished != null)
                {
                    onDisplacementFinished();
                }
            }
            else
            {
                transform.Translate(dir * displacement, Space.World);
            }
        }
    }

    public void DisplaceTo(Vector3 target)
    {
        this.target = target;
        isDisplaced = true;
        if (onDisplacementStarted != null)
        {
            onDisplacementStarted(gameObject, transform.position, target);
        }
    }
}

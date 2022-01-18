using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        FREEZED, NORMAL, TRANSLATING
    }
    public float speed = 10;
    public float rotationSpeed = 720;
    public LayerMask blockMask;
    public float pushDelay = 0.2f;

    private State state = State.NORMAL;
    private Rigidbody rb;
    private Animator animator;
    private Transform blockDetect;
    private bool waitingPush = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        blockDetect = transform.Find("BlockDetect");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (state != State.NORMAL)
        {
            return;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;

        rb.velocity = move * speed;

        animator.SetFloat("Speed", move.magnitude);

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }

        Collider[] colliders = Physics.OverlapSphere(blockDetect.position, 0.1f, blockMask);

        if (colliders.Length > 0 && rb.velocity != Vector3.zero && !waitingPush)
        {
            // TODO push
            StartCoroutine(PushCoroutine(colliders[0]));
        }
        else if ((colliders.Length == 0 || move == Vector3.zero) && waitingPush)
        {
            waitingPush = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator PushCoroutine(Collider collider)
    {
        waitingPush = true;
        yield return new WaitForSeconds(pushDelay);
        Push(collider);
        yield return new WaitForSeconds(pushDelay);
        waitingPush = false;
    }

    private void Push(Collider collider)
    {
        Vector3 diff = collider.transform.position - transform.position;

        Vector3 dir = Vector3.zero;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
        {
            dir.x = Mathf.Sign(diff.x);
        }
        else
        {
            dir.z = Mathf.Sign(diff.z);
        }

        // if (Physics.Raycast(transform.position, dir, diff.magnitude * 2))
        // {
        //     return;
        // }

        if (Physics.OverlapSphere(transform.position + diff * 2, 0.1f).Length > 0 || dir == Vector3.zero)
        {
            return;
        }

        Displaceable displaceable = collider.GetComponent<Displaceable>();
        displaceable.DisplaceTo(collider.transform.position + dir * 3);
        state = State.FREEZED;

        displaceable.onDisplacementFinished += OnDisplacementFinished;

        animator.SetBool("Jumping", true);
    }

    public void Displace(Vector3 to)
    {
        state = State.TRANSLATING;
        Displaceable displaceable = GetComponent<Displaceable>();
        displaceable.DisplaceTo(to);
        displaceable.onDisplacementFinished += OnPlayerDisplacementFinished;
        rb.isKinematic = true;
    }

    private void OnPlayerDisplacementFinished()
    {
        state = State.NORMAL;
        rb.isKinematic = false;
    }

    private void OnDisplacementFinished()
    {
        state = State.NORMAL;
        animator.SetBool("Jumping", false);
    }
}

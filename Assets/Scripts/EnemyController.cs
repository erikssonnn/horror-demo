using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
    [SerializeField] private LayerMask lm = 0;
    // [SerializeField] private SphereCollider triggerCol = null;
    // [SerializeField] private CapsuleCollider col = null;

    private bool chase = false;

    private Transform player = null;
    private Animator anim = null;
    private NavMeshAgent agent = null;

    private void Start() {
        player = FindObjectOfType<MovementController>().transform;
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void LateUpdate() {
        Animate();
        Move();
    }

    private void Move() {
        if (chase) {
            agent.SetDestination(player.position);
        }
    }

    private void Animate() {
        anim.SetFloat("speed", agent.velocity.magnitude);
    }

    private void OnTriggerStay(Collider col) {
        if (col.transform == player) {
            RaycastHit[] hits;
            Vector3 dir = (player.position - transform.position).normalized;
            float dist = Vector3.Distance(transform.position, player.position);
            Ray ray = new Ray(transform.position, dir);

            hits = Physics.RaycastAll(ray, dist, lm);

            if(hits.Length > 0) {
                if (hits[0].transform != player) {
                    chase = false;
                } else {
                    chase = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider col) {
        if(col.transform == player) {
            chase = false;
        }
    }
}

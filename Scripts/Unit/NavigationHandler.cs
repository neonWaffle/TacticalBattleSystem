using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NavigationHandler : MonoBehaviour
{
    Unit unit;

    [SerializeField, Min(0.0f)] float stoppingDistance = 0.1f;
    [SerializeField, Min(0.5f)] float movementSpeed = 5.0f;
    [SerializeField, Min(1.0f)] float rotationSpeed = 45.0f;

    bool isMoving = false;

    public event Action OnDestinationReached;
    public event Action OnStartedMoving;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public void Move(List<GridNode> path)
    {
        if (!isMoving)
        {
            StartCoroutine(HandleMovement(path));
        }
    }

    IEnumerator HandleMovement(List<GridNode> path)
    {
        OnStartedMoving?.Invoke();
        isMoving = true;
        unit.Animator.SetBool("IsMoving", true);
        int currentNodeId = 0;
        while (currentNodeId < path.Count && path[currentNodeId].IsReachable)
        {
            var dir = (path[currentNodeId].transform.position - transform.position).normalized;
            transform.position += movementSpeed * Time.deltaTime * dir;
            if (dir != Vector3.zero)
            {
                var rot = Quaternion.LookRotation(dir);
                rot = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
                transform.rotation = rot;
            }

            if (Vector3.SqrMagnitude(transform.position - path[currentNodeId].transform.position) <= Mathf.Pow(stoppingDistance, 2))
            {
                AssignUnitNode(path[currentNodeId]);
                if (currentNodeId == path.Count - 1)
                    break;
                
                currentNodeId++;
            }
            yield return null;
        }

        unit.Animator.SetBool("IsMoving", false);
        OnDestinationReached?.Invoke();
        isMoving = false;
    }

    void AssignUnitNode(GridNode node)
    {
        unit.Node.Unit = null;
        unit.Node = node;
        unit.Node.Unit = unit;
    }
}

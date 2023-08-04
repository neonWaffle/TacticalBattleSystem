using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathVisualiser : MonoBehaviour
{
    [SerializeField] Vector3 offset = new Vector3(0, 0.01f, 0);
    [SerializeField] LineRenderer reachablePathRenderer;
    [SerializeField] LineRenderer unreachablePathRenderer;

    [SerializeField] GameObject invalidDestinationVisualiser;
    [SerializeField] GameObject validDestinationVisualiser;

    void Awake()
    {
        DisableVisuals();
    }

    public void DisableVisuals()
    {
        reachablePathRenderer.enabled = false;
        unreachablePathRenderer.enabled = false;
        validDestinationVisualiser.SetActive(false);
        invalidDestinationVisualiser.SetActive(false);
    }

    public void ShowPath(List<GridNode> path, GridNode startNode)
    {
        var reachableNodes = path.TakeWhile(node => node.IsReachable).ToList();
        var unreachableNodes = path.SkipWhile(node => node.IsReachable).ToList();

        if (reachableNodes.Count > 0)
        {
            reachablePathRenderer.positionCount = 1;
            reachablePathRenderer.SetPosition(0, startNode.transform.position);
            reachablePathRenderer.enabled = true;

            unreachablePathRenderer.positionCount = 1;
            unreachablePathRenderer.SetPosition(0, reachableNodes[reachableNodes.Count - 1].transform.position);
            unreachablePathRenderer.enabled = true;
        }
        else
        {
            unreachablePathRenderer.positionCount = 1;
            unreachablePathRenderer.SetPosition(0, startNode.transform.position);
            unreachablePathRenderer.enabled = true;

            reachablePathRenderer.enabled = false;
        }

        for (int i = 0; i < reachableNodes.Count; i++)
        {
            reachablePathRenderer.positionCount++;
            reachablePathRenderer.SetPosition(i + 1, reachableNodes[i].transform.position + offset);
        }

        for (int i = 0; i < unreachableNodes.Count; i++)
        {
            unreachablePathRenderer.positionCount++;
            unreachablePathRenderer.SetPosition(i + 1, unreachableNodes[i].transform.position + offset);
        }
    }
}

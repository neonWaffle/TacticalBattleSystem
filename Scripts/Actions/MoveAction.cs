using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    Unit executingUnit;
    List<GridNode> path;

    public MoveAction(Unit executingUnit, List<GridNode> path) : base(false)
    {
        this.executingUnit = executingUnit;
        this.path = path;
    }

    public override void Execute()
    {
        executingUnit.NavigationHandler.OnDestinationReached += ReachDestination;
        executingUnit.NavigationHandler.Move(path);
    }

    void ReachDestination()
    {
        executingUnit.NavigationHandler.OnDestinationReached -= ReachDestination;
        CompleteAction();
    }
}

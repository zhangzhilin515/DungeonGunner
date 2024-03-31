using System.Collections.Generic;
using UnityEngine;

public static class Astar
{
    public static Stack<Vector3> BuildPath(Room room,Vector3Int startGridPosition,Vector3Int endGridPosition)
    {
        startGridPosition -= (Vector3Int)room.templateLowerBounds;
        endGridPosition -= (Vector3Int)room.templateLowerBounds;

        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();

        GridNodes gridNodes = new GridNodes(room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.y, endGridPosition.y);

        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, room.instantiatedRoom);
        if(endPathNode!=null)
        {
            return CreatePathStack(endPathNode, room);
        }
        return null;
    }
    private static Node FindShortestPath(Node startNode,Node targetNode,GridNodes gridNodes,List<Node> openNodeList,
        HashSet<Node> closedNodeHashSet,InstantiatedRoom instantiatedRoom)
    {
        openNodeList.Add(startNode);
        while(openNodeList.Count>0)
        {
            openNodeList.Sort();

            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            if (currentNode == targetNode) return currentNode;

            closedNodeHashSet.Add(currentNode);

            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, instantiatedRoom);
        }
        return null;
    }
    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList,
        HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                validNeighbourNode = GetValidNodeNeightbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridNodes, closedNodeHashSet, instantiatedRoom);
                if(validNeighbourNode!=null)
                {
                    int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[validNeighbourNode.gridPosition.x, validNeighbourNode.gridPosition.y];
                    int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode)+movementPenaltyForGridSpace;
                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);
                    if(newCostToNeighbour<validNeighbourNode.gCost||!isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;
                        if(!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }
    private static Node GetValidNodeNeightbour(int x,int y,GridNodes gridNodes,HashSet<Node> closedNodeHashSet,InstantiatedRoom instantiatedRoom)
    {
        if(x<0||x>=instantiatedRoom.room.templateUpperBounds.x-instantiatedRoom.room.templateLowerBounds.x||
            y<0||y>=instantiatedRoom.room.templateUpperBounds.y-instantiatedRoom.room.templateLowerBounds.y)
        {
            return null;
        }
        Node neighbourNode = gridNodes.GetGridNode(x, y);
        int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[x, y];
        int itemObstacleForGridSpace = instantiatedRoom.aStarItemObstacles[x, y];
        if(closedNodeHashSet.Contains(neighbourNode)||movementPenaltyForGridSpace==0||itemObstacleForGridSpace==0)
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }
    private static int GetDistance(Node node1,Node node2)
    {
        int dstX = Mathf.Abs(node1.gridPosition.x - node2.gridPosition.x);
        int dstY = Mathf.Abs(node1.gridPosition.y - node2.gridPosition.y);
        if (dstX > dstY) return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
    private static Stack<Vector3> CreatePathStack(Node endPathNode,Room room)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();
        Node nextNode = endPathNode;

        Vector3 cellMidPoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while(nextNode!=null)
        {
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x + room.templateLowerBounds.x,
                nextNode.gridPosition.y + room.templateLowerBounds.y, 0));
            worldPosition += cellMidPoint;
            movementPathStack.Push(worldPosition);
            nextNode = nextNode.parentNode;
        }
        return movementPathStack;
    }
}

//Name: Robert MacGillivray
//File: Maze.cs
//Date: Aug.03.2016
//Purpose: Stores information about an instanced maze. Also handles all of the pathfinding for the instanced maze.

//Last Updated: Jul.01.2022 by Robert MacGillivray

using UnityEngine;
using System.Collections.Generic;

namespace UmbraEvolution.UmbraMazeMagician
{
    /// <summary>
    /// Stores information about an instanced maze. Also capable of handling pathfinding in that maze.
    /// </summary>
    public class Maze : MonoBehaviour
    {
        public MazeMap mazeMap;
        public MazeNode[] mazeNodes;

        /// <summary>
        /// Given a set of maze coordinates, returns the MazeNode component attached to the floor tile at those coordinates.
        /// </summary>
        /// <param name="coordinates">The coordinates to look at.</param>
        /// <returns>The MazeNode component attached to the floor tile at the given coordinates.</returns>
        public MazeNode GetNodeFromCoordinates(Vector2Int coordinates)
        {
            if (coordinates[0] < 0 || coordinates[0] >= mazeMap.mazeWidth || coordinates[1] < 0 || coordinates[1] >= mazeMap.mazeLength)
            {
                Debug.LogError("Coordinates out of bounds. Returning null.");
                return null;
            }
            return mazeNodes[(coordinates[1] * mazeMap.mazeWidth) + coordinates[0]];
        }

        /// <summary>
        /// Given a position in World Space, finds the nearest MazeNode component (i.e. floor tile) in this maze.
        /// </summary>
        /// <param name="position">The World Space position to search from.</param>
        /// <returns>The MazeNode component attached to the floor tile nearest to the given World Space position.</returns>
        public MazeNode GetNearestNode(Vector3 position)
        {
            float minDist = float.MaxValue;
            MazeNode foundNode = null;

            foreach (MazeNode node in mazeNodes)
            {
                float testDist = Vector3.Distance(position, node.transform.position);
                if (testDist < minDist)
                {
                    minDist = testDist;
                    foundNode = node;
                }
            }

            return foundNode;
        }

        /// <summary>
        /// Given a position in World Space, finds the nearest MazeNode component (i.e. floor tile) in this maze that does not represent a wall.
        /// </summary>
        /// <param name="position">The World Space position to search from.</param>
        /// <returns>The MazeNode component attached to the floor tile nearest to the given World Space position that does not represent a wall.</returns>
        public MazeNode GetNearestNodeNotWall(Vector3 position)
        {
            float minDist = float.MaxValue;
            MazeNode foundNode = null;

            foreach (MazeNode node in mazeNodes)
            {
                if (!node)
                {
                    continue;
                }

                if (!MazeGenerator.IsWall(node.tileType))
                {
                    float testDist = Vector3.Distance(position, node.transform.position);
                    if (testDist < minDist)
                    {
                        minDist = testDist;
                        foundNode = node;
                    }
                }
            }

            if (!foundNode)
            {
                Debug.LogWarning("No node found with GetNearestNodeNotWall(Vector3 position) - Returning Null");
            }

            return foundNode;
        }

        /// <summary>
        /// Finds the shortest path through the maze from the first World Space position to the second World Space position. 
        /// Note that it will first find approximate nodes in the maze close to the given positions, then pathfind between those.
        /// </summary>
        /// <param name="startPosition">The initial point to pathfind from.</param>
        /// <param name="endPosition">The final point to pathfind to.</param>
        /// <returns>A list of MazeNode components. The start position will be at index 0, and the end position will be the final element in the list.</returns>
        public List<MazeNode> PathFind(Vector3 startPosition, Vector3 endPosition)
        {
            return PathFind(GetNearestNodeNotWall(startPosition), GetNearestNodeNotWall(endPosition));
        }

        /// <summary>
        /// Finds the shortest path through the maze from the first set of coordinates to the second set of coordinates. 
        /// </summary>
        /// <param name="startPosition">The initial space to pathfind from.</param>
        /// <param name="endPosition">The final space to pathfind to.</param>
        /// <returns>A list of MazeNode components. The start coordinates will be at index 0, and the end coordinates will be the final element in the list.</returns>
        public List<MazeNode> PathFind(Vector2Int startCoords, Vector2Int endCoords)
        {
            return PathFind(GetNodeFromCoordinates(startCoords), GetNodeFromCoordinates(endCoords));
        }

        /// <summary>
        /// Finds the shortest path through the maze from the first MazeNode to the second MazeNode. 
        /// Note that this is the method that all other PathFind() methods resolve to.
        /// </summary>
        /// <param name="startPosition">The initial MazeNode to pathfind from.</param>
        /// <param name="endPosition">The final MazeNode to pathfind to.</param>
        /// <returns>A list of MazeNode components. The start coordinates will be at index 0, and the end coordinates will be the final element in the list.</returns>
        public List<MazeNode> PathFind(MazeNode startNode, MazeNode endNode)
        {
            if (startNode == null)
            {
                Debug.LogError("Cannot pathfind from a null node. Returning null.");
                return null;
            }

            if (endNode == null)
            {
                Debug.LogError("Cannot pathfind to a null node. Returning null.");
                return null;
            }

            if (MazeGenerator.IsWall(startNode.tileType))
            {
                Debug.LogWarning("Cannot pathfind starting from within a wall. Returning null.");
                return null;
            }

            if (MazeGenerator.IsWall(endNode.tileType))
            {
                Debug.LogWarning("Cannot pathfind into a wall. Returning null.");
                return null;
            }

            //Must reset pathfinding metadata. Could be modified so that temporary node objects are created and manipulated in memory but,
            //for large mazes, that has a big memory overhead for marginal realtime speed improvements.
            foreach (MazeNode node in mazeNodes)
            {
                if (node)
                {
                    node.ClearPathfinding();
                }
            }

            List<MazeNode> nextToVisit = new List<MazeNode>();

            MazeNode currentNode = null;
            startNode.ChangeParent(null, endNode);
            nextToVisit.Add(startNode);

            while (nextToVisit.Count > 0 && endNode.Parent == null)
            {
                //Because of how nodes are added to the open set (nextToVisit list), the last one is always the most desirable candidate to add to the path (basically a priority queue).
                //We use the last position in the list because it is far more efficient to manipulate that position and remove items from the end of the list than the front.
                //In theory, this could be modified so that it's a heap, but the overhead of maintaining the heap is typically not worth it since, most of the time, we will be pathfinding in the correct direction.
                currentNode = nextToVisit[nextToVisit.Count - 1];
                nextToVisit.RemoveAt(nextToVisit.Count - 1);

                EvaluateNode(currentNode, currentNode.up, endNode, nextToVisit);
                EvaluateNode(currentNode, currentNode.right, endNode, nextToVisit);
                EvaluateNode(currentNode, currentNode.down, endNode, nextToVisit);
                EvaluateNode(currentNode, currentNode.left, endNode, nextToVisit);
            }

            //If the target has a parent, there is a chain of parents along the shortest path back to the start. We build that path and return it as a list from start to end.
            if (endNode.Parent != null)
            {
                return BuildPath(endNode);
            }
            //If the target does not have a parent, it means we ran out of nodes to resolve without finding it. This indicates that there is no valid path between the two points
            //Note that this is impossible with the default functionality of this maze generator since it creates "perfect" mazes. The algorithm or maze have been modified such that there can be unreachable spots now.
            else
            {
                Debug.LogWarning("No valid path was found. Returning null.");
                return null;
            }
        }

        /// <summary>
        /// Determines whether or not the given node needs updated pathfinding metadata or is a fresh node that can be added to the list of nodes to visit.
        /// </summary>
        /// <param name="currentNode">The node that is currently being resolved in pathfinding.</param>
        /// <param name="testNode">A neighbouring node that we are testing.</param>
        /// <param name="endNode">The goal for the pathfinding algorithm.</param>
        /// <param name="nodesToVisit">The list of nodes to visit in the future.</param>
        /// <returns>Nothing. nodesToVisit will be updated as necessary.</returns>
        private void EvaluateNode(MazeNode currentNode, MazeNode testNode, MazeNode endNode, List<MazeNode> nodesToVisit)
        {
            //If we're not evaluating a real test node, return.
            if (testNode == null)
                return;

            //If this is a wall, it's not a valid neighbour for pathfinding.
            if (MazeGenerator.IsWall(testNode.tileType))
                return;

            //If this node has been visited before, see if we've found a more efficient path to it.
            if (testNode.Visited)
            {
                if (currentNode.DistanceToStart + 1 < testNode.DistanceToStart)
                {
                    //Update pathfinding metadata for this node since we've found a shorter path to it than the one it's currently part of
                    testNode.ChangeParent(currentNode, endNode);
                }
                return;
            }

            //This is a fresh node. Add it to the current shortest path and fill in the other pathfinding metadata.
            testNode.ChangeParent(currentNode, endNode);

            //Insert new node into list of nodes to visit based on heuristic (simple Manhattan distance to goal).
            //Inserting at the end indicates that this node will be resolved next, so closer nodes should be toward the end.
            //Note that while this is linear time (kind of slow), most of the time we won't have to traverse much of the open set.
            bool found = false;
            for (int index = nodesToVisit.Count - 1; index >= 0 && !found; index--)
            {
                if (testNode.EstimatedPathLength <= nodesToVisit[index].EstimatedPathLength)
                {
                    nodesToVisit.Insert(index + 1, testNode);
                    found = true;
                }
            }
            if (!found)
            {
                nodesToVisit.Insert(0, testNode);
            }
        }

        /// <summary>
        /// Starting from the target node, builds a list of nodes representing a path from start to finish by grabbing successive parents from the pathfinding metadata.
        /// </summary>
        /// <param name="endNode">The node we were targeting with pathfinding.</param>
        /// <returns></returns>
        private List<MazeNode> BuildPath(MazeNode endNode)
        {
            List<MazeNode> path = new List<MazeNode>();
            MazeNode current = endNode;
            while (current != null)
            {
                path.Add(current);
                current = current.Parent;
            }
            //Reverse the list so that it starts with the start node at index 0 and ends with the target node.
            path.Reverse();
            return path;
        }
    }
}

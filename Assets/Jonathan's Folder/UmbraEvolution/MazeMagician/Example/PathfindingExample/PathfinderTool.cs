// Name: Robert MacGillivray
// File: PathfinderTool.cs
// Date: Dec.13.2019
// Purpose: To illustrate the new pathfinding tool.

// Last Updated: Jul.01.2022 by Robert MacGillivray

using UnityEngine;
using System.Collections.Generic;

namespace UmbraEvolution.UmbraMazeMagician
{
    /// <summary>
    /// Illustrative example for the pathfinding system
    /// </summary>
    public class PathfinderTool : MonoBehaviour
    {
        [Tooltip("The object to mark a path with.")]
        public GameObject Breadcrumb;
        [Tooltip("The maze to test pathfinding with.")]
        public Maze TestMaze;
        
        public enum PathfindingType
        {
            Positions,
            Coordinates,
            Nodes
        }
        [Tooltip("The type of pathfinding to test.")]
        public PathfindingType TestPathfindingMethod;

        [Tooltip("The start world position to pathfind from. Will find the nearest tile.")]
        public Vector3 StartPosition;
        [Tooltip("The end world position to pathfind to. Will find the nearest tile.")]
        public Vector3 EndPosition;

        [Tooltip("The start coordinates of a tile to pathfind from. Bottom-left of the maze is (0,0).")]
        public Vector2Int StartCoordinate;
        [Tooltip("The end coordinates of a tile to pathfind to. Bottom-left of the maze is (0,0).")]
        public Vector2Int EndCoordinate;

        [Tooltip("A specific maze node to pathfind from. MazeNode components are attached to all floor tiles, including the ones underneath walls.")]
        public MazeNode StartNode;
        [Tooltip("A specific maze node to pathfind to. MazeNode components are attached to all floor tiles, including the ones underneath walls.")]
        public MazeNode EndNode;

        // Stores all of the little breadcrumbs for visualization
        private GameObject _breadcrumbHolder;

        public void RunTest()
        {
            if (!Breadcrumb)
            {
                Debug.LogError("Must have a Breadcrumb object to mark the path.");
                return;
            }

            if (!TestMaze)
            {
                Debug.LogError("Must have a TestMaze to pathfind through.");
                return;
            }

            // All pathfinding returns a list of nodes that make up a path from the start to the goal. 
            // path[0] will be the start and path[path.Count - 1] will be the goal.
            List<MazeNode> path = new List<MazeNode>();
            switch (TestPathfindingMethod)
            {
                case PathfindingType.Positions:
                    // This is a little helper for when you want to work with world-space coordinates.
                    path = TestMaze.PathFind(StartPosition, EndPosition);
                    break;
                case PathfindingType.Coordinates:
                    // This is a little helper for when you want to work with internal maze coordinates.
                    // Each tile has its own coordinate, starting from (0,0) in the "bottom-left" corner
                    path = TestMaze.PathFind(StartCoordinate, EndCoordinate);
                    break;
                case PathfindingType.Nodes:
                    // This is what all pathfinding uses internally. It relies on the MazeNode component of each tile.
                    path = TestMaze.PathFind(StartNode, EndNode);
                    break;
            }

            // Some code to display the path
            if (path == null)
            {
                Debug.LogWarning("No valid path found.");
            }
            else
            {
                CleanUpTest();

                _breadcrumbHolder = new GameObject("Breadcrumb Holder");
                for (int index = 0; index < path.Count; ++index)
                {
                    GameObject temp = Instantiate(Breadcrumb);
                    temp.transform.SetParent(_breadcrumbHolder.transform);
                    temp.transform.position = path[index].transform.position;
                    temp.transform.LookAt(path[(index + 1) % path.Count].transform);
                }
            }
        }

        public void CleanUpTest()
        {
            if (_breadcrumbHolder)
            {
                if (Application.isEditor && !Application.isPlaying)
                {
                    DestroyImmediate(_breadcrumbHolder);
                }
                else
                {
                    Destroy(_breadcrumbHolder);
                }
            }
        }
    }
}

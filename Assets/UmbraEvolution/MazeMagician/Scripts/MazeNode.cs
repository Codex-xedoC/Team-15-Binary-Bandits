//Name: Robert MacGillivray
//Date: Apr.21.2017
//File: MazeNode.cs
//Purpose: To keep track of connections between nodes of the maze (most likely for pathfinding purposes only)

 //Last Updated: Jul.01.2022 by Robert MacGillivray

using UnityEngine;

namespace UmbraEvolution.UmbraMazeMagician
{
    /// <summary>
    /// Models connections between nodes in the maze for pathfinding purposes.
    /// </summary>
    public class MazeNode : MonoBehaviour
    {
        [ReadOnlyInInspector]
        [Tooltip("The state of this tile in the maze map")]
        public MazeMap.TileState tileType;
        [ReadOnlyInInspector]
        [Tooltip("The (column, row) coordinates of this tile in the maze map")]
        public Vector2Int coordinates;
        [ReadOnlyInInspector]
        [Tooltip("The tile up from this one")]
        public MazeNode up;
        [ReadOnlyInInspector]
        [Tooltip("The tile right from this one")]
        public MazeNode right;
        [ReadOnlyInInspector]
        [Tooltip("The tile down from this one")]
        public MazeNode down;
        [ReadOnlyInInspector]
        [Tooltip("The tile left from this one")]
        public MazeNode left;

        #region Pathfinding Metadata
        //Pathfinding metadata is not visible in the inspector, and should never be set directly
        public MazeNode Parent { get; private set; }
        public float DistanceToStart { get; private set; }
        public float EstimatedPathLength { get; private set; }
        public bool Visited { get; private set; }
        #endregion

        /// <summary>
        /// Adds a connnection between this node and the next node along the Z-axis
        /// </summary>
        /// <param name="connectedNode"></param>
        public void AddConnectionUp(MazeNode connectedNode)
        {
            up = connectedNode;
            connectedNode.down = this;
        }

        /// <summary>
        /// Adds a connection between this node and the next node along the X-axis
        /// </summary>
        /// <param name="connectedNode"></param>
        public void AddConnectionRight(MazeNode connectedNode)
        {
            right = connectedNode;
            connectedNode.left = this;
        }

        /// <summary>
        /// Adds a connection between this node and the next node along the negative Z-axis
        /// </summary>
        /// <param name="connectedNode"></param>
        public void AddConnectionDown(MazeNode connectedNode)
        {
            down = connectedNode;
            connectedNode.up = this;
        }

        /// <summary>
        /// Adds a connection between this node and the next node along the negative X-axis
        /// </summary>
        /// <param name="connectedNode"></param>
        public void AddConnectionLeft(MazeNode connectedNode)
        {
            left = connectedNode;
            connectedNode.right = this;
        }
        
        /// <summary>
        /// Clears the pathfinding metadata on this node
        /// </summary>
        public void ClearPathfinding()
        {
            Parent = null;
            DistanceToStart = -1;
            EstimatedPathLength = -1;
            Visited = false;
        }

        /// <summary>
        /// Updates the pathfinding metadata for this node based on its new parent node on the path being built and the ultimate target of the current pathfinding operation
        /// </summary>
        /// <param name="newParent">The node before this one in the path currently being built</param>
        /// <param name="target">The ultimate target of the current pathfinding operation</param>
        public void ChangeParent(MazeNode newParent, MazeNode target)
        {
            Parent = newParent;
            //Calculate the real distance from this node to the origin of the current pathfinding operation. Variable movement cost is not taken into account at this time.
            DistanceToStart = Parent != null ? Parent.DistanceToStart + 1 : 0f;
            //Estimate the distance from this node to the ultimate target of the pathfinding operation. Simple Manhattan distance assuming uniform width/length tiles. Variable movement cost is not taken into account at this time.
            EstimatedPathLength = DistanceToStart + Mathf.Abs(target.coordinates[0] - coordinates[0]) + Mathf.Abs(target.coordinates[1] - coordinates[1]);

            //Update pathfinding metadata of all child nodes recursively (a node that has never been visited cannot have child nodes)
            if (Visited)
            {
                if (up != null)
                    if (up.Parent == this)
                        up.ChangeParent(this, target);
                if (right != null)
                    if (right.Parent == this)
                        right.ChangeParent(this, target);
                if (down != null)
                    if (down.Parent == this)
                        down.ChangeParent(this, target);
                if (left != null)
                    if (left.Parent == this)
                        left.ChangeParent(this, target);
            }

            //Mark this node as visited. Fewer operations need to be performed on nodes that are not already part of a path.
            Visited = true;
        }
    }
}
//Name: Robert MacGillivray
//Date: Jun.10.2015
//File: MazeMap.cs
//Purpose: A class that holds all information needed to instantiate the pieces of a maze

//Last Updated: Jul.01.2022 by Robert MacGillivray

namespace UmbraEvolution.UmbraMazeMagician
{
    [System.Serializable]
    public class MazeMap
    {
        public enum TileState //the enum with every possible type of tile
        {
            unexplored, //any empty tile that hasn't been visited during the generation step
            wall, //any tile that will be a normal wall
            brokenWall, //any tile that was originally a wall, but that the generator has deleted to make the path
            visitedOnce, //used in generation to let the generator know not to go back over this spot unless necessary (prevents dead zones in the maze)
            visitedTwice, //used in generation to let the generator know not to go back over this spot
            plaza, //any space slotted to contain a plaza prefab
            deadEnd, //a floor tile that's located at a dead end
            finish, //the tile that will be marked as the exit
            outOfBounds, //used to indicate that a checked location is not valid (i.e. outside the maze dimensions)
            cornerWall, //any tile that will be a wall at a junction
            endWallUp, //any tile that will be the end of a wall with z-axis aligned to world z-axis
            endWallRight, //any tile that will be the end of a wall with z-axis aligned to world x-axis 
            endWallDown, //any tile that will be the end of a wall with z-axis aligned to negative z-axis
            endWallLeft, //any tile that will be the end of a wall with z-axis aligned to negative x-axis
            start, //the tile that will be marked as the start
            unique //marks this as a possible spawn for a unique tile
        }
        public TileState[] map; //the array of enums that logically maps out the maze
        public int mazeWidth; //the width of the maze in tiles
        public int mazeLength; //the length of the maze in tiles
    }
}

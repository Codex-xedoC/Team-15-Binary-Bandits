//Name: Robert MacGillivray
//File: MazeGenerator.cs
//Date: Nov.08.2014
//Purpose: To generate a new maze

//Last Updated: Jul.01.2022 by Robert MacGillivray


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UmbraEvolution.UmbraMazeMagician
{
    /// <summary>
    /// Main script for generating mazes
    /// </summary>
    [ExecuteInEditMode]
    public class MazeGenerator : MonoBehaviour
    {
        public enum MazeDimension
        {
            TwoDimensional,
            ThreeDimensional
        }
        [Tooltip("The dimensional space the maze will be generated in")]
        public MazeDimension mazeDimension = MazeDimension.ThreeDimensional;
        [Tooltip("If set to true, you may enter a seed value to control the generation of the maze. Assuming the variables on this Maze Generator are identical, setting the seed value will result in the exact same maze being generated every time. If any of the settings have changed however (i.e. the width or length is different), then the maze will generate differently.")]
        public bool useSeedValue;
        [Tooltip("The seed value used to generate this maze. Entering the same seed value will result in the same placement of floor tiles, walls, and details.")]
        public int seedValue;
        [Tooltip("Shows the space the maze will take up in the inspector once generated as a cube gizmo.")]
        public bool showDimensionsGizmo = true;
        [Tooltip("Sets the colour of the gizmo that shows what the generated maze dimensions will be.")]
        public Color dimensionsGizmoColour = new Color(0f, 1f, 0f, 0.5f);
        [Tooltip("How many units wide is the maze?")]
        [GreaterThanInt(0, false)]
        public int mazeWidth = 13;
        [Tooltip("How many units long is the maze?")]
        [GreaterThanInt(0, false)]
        public int mazeLength = 13;
        [Tooltip("Where do you want the maze to be located? This will be the coordinates of the corner of the maze.")]
        public Vector3 mazePosition;
        [Tooltip("If checked, dead ends will be deleted in order to turn the maze into a braid maze (there is more than one path to any point in the maze).")]
        public bool makeBraidMaze;
        [Tooltip("If checked, a floor tile will be removed or replaced for the exit.")]
        public bool makeFloorExit;
        [Tooltip("If checked, a floor tile will be removed or replaced for the start.")]
        public bool makeStart;
        [Tooltip("If checked, any specified unique tiles will be added to the maze.")]
        public bool makeUniqueTiles;
        [Tooltip("Each of these unique tiles will be placed somewhere throughout the maze.")]
        public UniqueTile[] uniqueTiles = new UniqueTile[1];
        public enum FloorExitType
        {
            random,     //random inside the maze
            center,     //random, center 1/3 of the maze
            outside     //random, outer 1/3 of the maze
        }
        [Tooltip("Center: tile will be located in roughly the center third of the maze.\nOutside: tile will be placed in the ring around that center\nRandom: tile can be anywhere.")]
        public FloorExitType exitType = FloorExitType.random;
        [Tooltip("Center: tile will be located in roughly the center third of the maze.\nOutside: tile will be placed in the ring around that center\nRandom: tile can be anywhere.")]
        public FloorExitType startType = FloorExitType.random;
        [Tooltip("If true, the generator will attempt to place pre-defined plazas throughout the maze (think wide open gardens and things like that)")]
        public bool makePlazas;
        [Tooltip("If true, the generator will delete floor tiles inside pre-defined plaza spaces. Note that the built-in pathfinding will not work across plazas with floors removed.")]
        public bool removePlazaFloors;
        [Tooltip("How many plazas to attempt to place")]
        public int numberOfPlazas;
        [Tooltip("How many times to attempt placing each plaza. If the algorithm attempts to place a plaza and it will intersect another one that counts as an attempt.")]
        [GreaterThanInt(1, true)]
        public int plazaPlacementAttempts;
        [Tooltip("An array of the potential plazas that can be placed.")]
        public PlazaPrefab[] plazas = new PlazaPrefab[0];
        [Tooltip("The width and length of all wall pieces, floor pieces, and other tiles in the maze. Mazes without square tiles look bad, so I got rid of those options to simplify the controls.")]
        [GreaterThanFloat(0f, false)]
        public float mazeTileWidthAndLength;
        [Tooltip("The number of unique normal wall piece variations in your maze. Press the plus/minus buttons to adjust.")]
        public int numberOfWallPieces = 1;
        public enum OutsideWallDeleteMode
        {
            random,
            opposite,
            symmetric
        }
        [Tooltip("The manner in which to delete outer walls. Random is random, opposite deletes pieces in pairs on opposite sides. Symmetric deletes pieces in pairs on opposite sides in symmetric locations.")]
        public OutsideWallDeleteMode outsideWallDeleteMode = OutsideWallDeleteMode.random;
        public bool deleteOutsideWalls = false;
        [Tooltip("The number of wall pieces around the outside edge to delete. Useful for entrances and exits.")]
        public int outsideWallPiecesToDelete = 0;
        [Tooltip("The number of unique corner wall piece variations in your maze. Press the plus/minus buttons to adjust.")]
        public int numberOfCornerWallPieces = 1;
        [Tooltip("The number of unique end wall piece variations in your maze. Press the plus/minus buttons to adjust.")]
        public int numberOfEndWallPieces = 1;
        [Tooltip("Use a different set of pieces to place in corners.")]
        public bool differentCorners;
        [Tooltip("Use a different set of pieces to place at the ends of walls.")]
        public bool differentEnds;
        [Tooltip("The number of unique floor pieces in your maze. Press the plus/minus buttons to adjust.")]
        public int numberOfFloorPieces = 1;
        [Tooltip("The number of possible unique exits that could be instantiated in your maze. Press the plus/minus buttons to adjust.")]
        public int numberOfExitPieces = 1;
        [Tooltip("The number of possible unique starts that could be instantiated in your maze. Press the plus/minus buttons to adjust.")]
        public int numberOfStartPieces = 1;
        [Tooltip("What can possibly be placed at the exit of the maze. If you're generating an exit and leave this null, an empty tile will be left here as a sort of floor exit. Your imagination is the limit - triggers, an elevator, whatever you can fit into a prefab that will take up the same space as your other tiles will do the trick.")]
        public GameObject[] exitPieces = new GameObject[1];
        [Tooltip("What can possibly be placed at the start of the maze. If you're generating an start and leave this null, an empty tile will be left here. Your imagination is the limit - triggers, an elevator, whatever you can fit into a prefab that will take up the same space as your other tiles will do the trick.")]
        public GameObject[] startPieces = new GameObject[1];
        [Tooltip("If checked, you will be able to populate your maze with details.")]
        public bool addDetails = false;
        [Tooltip("All of the details to be placed along the walls (randomly distributed).")]
        public WallDetail[] wallDetails = new WallDetail[1];
        [Tooltip("All of the details to be placed throughout the maze (randomly distributed and rotated around the y-axis).")]
        public OtherDetail[] otherDetails = new OtherDetail[1];
        [Tooltip("The name of the maze, will be given to the GameObject created in the scene view. When generating or loading, if there is already a maze with this name in the scene, the new maze will replace it. If there is not a maze with this name in the scene, a new one will be instantiated.")]
        public string mazeName;
        [Tooltip("Enabling this will allow you to use Unity's built-in undo functions on maze generation in the editor. The drawback is that it slows down generation in the editor A LOT. It also causes memory issues after generating a number of large mazes since Unity's undo feature is extremely RAM hungry when it comes to a lot of instantiation and destruction.")]
        public bool allowEditorUndo;

        #region 3D Specific Variables
        [Tooltip("The default height of your wall pieces. Note that it is assumed your pivot point is in the center of the model in the algorithm calculations.")]
        [GreaterThanFloat(0f, false)]
        public float defaultWallHeight;
        [Tooltip("The default thickness of your floor pieces. Note that it is assumed your pivot point is in the center of the model in the algorithm calculations.")]
        [GreaterThanFloat(0f, false)]
        public float defaultFloorThickness;
        [Tooltip("All of the possible wall pieces to be placed (randomly distributed).")]
        public WallPiece[] wallPieces = new WallPiece[1];
        [Tooltip("All of the possible corner wall pieces to be placed (randomly distributed).")]
        public WallPiece[] cornerWallPieces = new WallPiece[1];
        [Tooltip("All of the possible end wall pieces to be placed (randomly distributed).")]
        public WallPiece[] endWallPieces = new WallPiece[1];
        [Tooltip("All of the possible floor pieces to be placed (randomly distributed). Note that they must all have the same dimensions you entered above.")]
        public FloorPiece[] floorPieces = new FloorPiece[1];
        #endregion

        #region 2D Specific Variables
        [Tooltip("The default z-value of the wall pieces")]
        public float defaultWallZPlane;
        [Tooltip("The z-value of the floor pieces")]
        public float floorZPlane;
        public GameObject[] floorPieces2D = new GameObject[1];
        public WallPiece2D[] wallPieces2D = new WallPiece2D[1];
        public WallPiece2D[] cornerWallPieces2D = new WallPiece2D[1];
        public WallPiece2D[] endWallPieces2D = new WallPiece2D[1];
        [Tooltip("The two-dimensional details that will be scattered throughout a two-dimensional maze")]
        public TwoDimensionalDetail[] twoDimensionalDetails = new TwoDimensionalDetail[1];
        #endregion

        public GameObject theMaze; //the root GameObject of the most current maze being generated in the editor (will have a Maze component once generated)

        public MazeMap latestMazeMap;//the map of the maze that was most recently generated or loaded in the editor

        private Dictionary<Vector2Int, UniqueTile> uniqueTileDict; //used to keep track of where each Unique Tile goes

        //used during generation to test a specific direction to generate into
        internal enum TestDirection
        {
            left = 1,
            up,
            right,
            down
        }

        #region Custom Inspector Variables
        //the following varibles are just used to store the state of foldouts in the custom inspector (I wish there were a cleaner way to do this, but alas, there is not
        [HideInInspector]
        public bool showFloorPieces = false;
        [HideInInspector]
        public bool showExitPieces = false;
        [HideInInspector]
        public bool showStartPieces = false;
        [HideInInspector]
        public bool showWallPieces = false;
        [HideInInspector]
        public bool showCornerWallPieces = false;
        [HideInInspector]
        public bool showEndWallPieces = false;
        [HideInInspector]
        public bool showDetails = false;
        #endregion

        void Awake()
        {
            #region Validating Prefab Arrays
            if (wallPieces == null || wallPieces.Length == 0)
            {
                wallPieces = new WallPiece[1];
                wallPieces[0] = new WallPiece(null, defaultWallHeight);
            }

            if (cornerWallPieces == null || cornerWallPieces.Length == 0)
            {
                cornerWallPieces = new WallPiece[1];
                cornerWallPieces[0] = new WallPiece(null, defaultWallHeight);
            }

            if (endWallPieces == null || endWallPieces.Length == 0)
            {
                endWallPieces = new WallPiece[1];
                endWallPieces[0] = new WallPiece(null, defaultWallHeight);
            }

            if (wallPieces2D == null || wallPieces2D.Length == 0)
            {
                wallPieces2D = new WallPiece2D[1];
                wallPieces2D[0] = new WallPiece2D(null, defaultWallZPlane);
            }

            if (cornerWallPieces2D == null || cornerWallPieces2D.Length == 0)
            {
                cornerWallPieces2D = new WallPiece2D[1];
                cornerWallPieces2D[0] = new WallPiece2D(null, defaultWallZPlane);
            }

            if (endWallPieces2D == null || endWallPieces2D.Length == 0)
            {
                endWallPieces2D = new WallPiece2D[1];
                endWallPieces2D[0] = new WallPiece2D(null, defaultWallZPlane);
            }

            if (floorPieces == null || floorPieces.Length == 0)
            {
                floorPieces = new FloorPiece[1];
                floorPieces[0] = new FloorPiece(null, defaultFloorThickness);
            }

            if (floorPieces2D == null || floorPieces2D.Length == 0)
            {
                floorPieces2D = new GameObject[1];
                floorPieces2D[0] = null;
            }
            #endregion

            //makes sure that we're not under the minimum number of wall/floor pieces
            if (numberOfWallPieces < 1)
            {
                numberOfWallPieces = 1;
            }

            if (numberOfFloorPieces < 1)
            {
                numberOfFloorPieces = 1;
            }

            uniqueTileDict = new Dictionary<Vector2Int, UniqueTile>();
        }

        void OnDrawGizmosSelected()
        {
            if (showDimensionsGizmo)
            {
                float offset = 0.1f;
                if (mazeDimension == MazeDimension.TwoDimensional)
                {
                    Gizmos.color = dimensionsGizmoColour;
                    Gizmos.DrawCube(mazePosition + new Vector3(((mazeWidth / 2f) * mazeTileWidthAndLength) - mazeTileWidthAndLength / 2f, ((mazeLength / 2f) * mazeTileWidthAndLength) - mazeTileWidthAndLength / 2f, 0f), new Vector3(mazeWidth * mazeTileWidthAndLength, mazeLength * mazeTileWidthAndLength, 1.3f));
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireCube(mazePosition + new Vector3(((mazeWidth / 2f) * mazeTileWidthAndLength) - mazeTileWidthAndLength / 2f, ((mazeLength / 2f) * mazeTileWidthAndLength) - mazeTileWidthAndLength / 2f, 0f), new Vector3(mazeWidth * mazeTileWidthAndLength, mazeLength * mazeTileWidthAndLength, 1.3f));
                }
                else
                {
                    Gizmos.color = dimensionsGizmoColour;
                    Gizmos.DrawCube(mazePosition + new Vector3(((mazeWidth / 2f) * mazeTileWidthAndLength) - mazeTileWidthAndLength / 2f, (defaultWallHeight / 2f) - (defaultFloorThickness / 2f), ((mazeLength / 2f) * mazeTileWidthAndLength) - mazeTileWidthAndLength / 2f), new Vector3(mazeWidth * mazeTileWidthAndLength + offset, defaultWallHeight + defaultFloorThickness + offset, mazeLength * mazeTileWidthAndLength + offset));
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireCube(mazePosition + new Vector3(((mazeWidth / 2f) * mazeTileWidthAndLength) - mazeTileWidthAndLength / 2f, (defaultWallHeight / 2f) - (defaultFloorThickness / 2f), ((mazeLength / 2f) * mazeTileWidthAndLength) - mazeTileWidthAndLength / 2f), new Vector3(mazeWidth * mazeTileWidthAndLength + offset, defaultWallHeight + defaultFloorThickness + offset, mazeLength * mazeTileWidthAndLength + offset));
                }
            }
        }

        void OnValidate()
        {
            //makes sure the maze width and height will not be too small or even as even numbers don't work with the algorithm
            if (mazeWidth < 3)
            {
                mazeWidth = 3;
            }
            if (mazeLength < 3)
            {
                mazeLength = 3;
            }
            if (mazeWidth % 2 != 1)
            {
                mazeWidth++;
            }
            if (mazeLength % 2 != 1)
            {
                mazeLength++;
            }

            if (plazas == null)
            {
                plazas = new PlazaPrefab[0];
            }

            if (deleteOutsideWalls)
            {
                int totalOutsideWalls = (mazeLength * 2) + (mazeWidth * 2) - 4;
                if (outsideWallPiecesToDelete > totalOutsideWalls)
                {
                    outsideWallPiecesToDelete = totalOutsideWalls;
                }
                if (outsideWallDeleteMode == OutsideWallDeleteMode.opposite || outsideWallDeleteMode == OutsideWallDeleteMode.symmetric)
                {
                    if (outsideWallPiecesToDelete % 2 == 1)
                    {
                        outsideWallPiecesToDelete++;
                    }
                }
            }

            //sets a default name to a generated maze if the user hasn't entered anything
            if (string.IsNullOrEmpty(mazeName))
            {
                mazeName = "NoName";
            }

            //if we're going to place unique tiles, better make sure we have a dictionary for it
            if (makeUniqueTiles)
            {
                if (uniqueTileDict == null)
                {
                    uniqueTileDict = new Dictionary<Vector2Int, UniqueTile>();
                }
            }

            if (addDetails)
            {
                //clamps wall detail positions to a specific face of wall
                foreach (WallDetail detail in wallDetails)
                {
                    if (detail.detailPrefab)
                    {
                        //Name string check
                        if (string.IsNullOrEmpty(detail.detailName))
                        {
                            detail.detailName = detail.detailPrefab.name;
                        }

                        //horizontal clamping
                        if (detail.minHorizontalOffset < -mazeTileWidthAndLength / 2 + detail.width / 2)
                        {
                            detail.minHorizontalOffset = -mazeTileWidthAndLength / 2 + detail.width / 2;
                        }
                        if (detail.maxHorizontalOffset < -mazeTileWidthAndLength / 2 + detail.width / 2)
                        {
                            detail.maxHorizontalOffset = -mazeTileWidthAndLength / 2 + detail.width / 2;
                        }
                        if (detail.minHorizontalOffset > mazeTileWidthAndLength / 2 - detail.width / 2)
                        {
                            detail.minHorizontalOffset = mazeTileWidthAndLength / 2 - detail.width / 2;
                        }
                        if (detail.maxHorizontalOffset > mazeTileWidthAndLength / 2 - detail.width / 2)
                        {
                            detail.maxHorizontalOffset = mazeTileWidthAndLength / 2 - detail.width / 2;
                        }
                        if (detail.maxHorizontalOffset < detail.minHorizontalOffset)
                        {
                            detail.maxHorizontalOffset = detail.minHorizontalOffset;
                        }

                        //vertical clamping
                        if (detail.minHeight < detail.height / 2)
                        {
                            detail.minHeight = detail.height / 2;
                        }
                        if (detail.maxHeight > defaultWallHeight - detail.height / 2)
                        {
                            detail.maxHeight = defaultWallHeight - detail.height / 2;
                        }
                        if (detail.minHeight > defaultWallHeight - detail.height / 2)
                        {
                            detail.minHeight = defaultWallHeight - detail.height / 2;
                        }
                        if (detail.maxHeight < detail.height / 2)
                        {
                            detail.maxHeight = detail.height / 2;
                        }
                        if (detail.maxHeight < detail.minHeight)
                        {
                            detail.maxHeight = detail.minHeight;
                        }
                    }
                }

                //clamps Other Details to a specific tile-space
                foreach (OtherDetail detail in otherDetails)
                {
                    if (detail.detailPrefab)
                    {
                        //Name string Check
                        if (string.IsNullOrEmpty(detail.detailName))
                        {
                            detail.detailName = detail.detailPrefab.name;
                        }

                        //vertical clamping
                        if (detail.minHeight < detail.height / 2)
                        {
                            detail.minHeight = detail.height / 2;
                        }
                        if (detail.maxHeight < detail.minHeight)
                        {
                            detail.maxHeight = detail.minHeight;
                        }

                        //x-axis clamping
                        if (detail.minXOffset < -mazeTileWidthAndLength / 2 + detail.width / 2)
                        {
                            detail.minXOffset = -mazeTileWidthAndLength / 2 + detail.width / 2;
                        }
                        if (detail.maxXOffset < -mazeTileWidthAndLength / 2 + detail.width / 2)
                        {
                            detail.maxXOffset = -mazeTileWidthAndLength / 2 + detail.width / 2;
                        }
                        if (detail.minXOffset > mazeTileWidthAndLength / 2 - detail.width / 2)
                        {
                            detail.minXOffset = mazeTileWidthAndLength / 2 - detail.width / 2;
                        }
                        if (detail.maxXOffset > mazeTileWidthAndLength / 2 - detail.width / 2)
                        {
                            detail.maxXOffset = mazeTileWidthAndLength / 2 - detail.width / 2;
                        }
                        if (detail.maxXOffset < detail.minXOffset)
                        {
                            detail.maxXOffset = detail.minXOffset;
                        }

                        //z-axis clamping
                        if (detail.minZOffset < -mazeTileWidthAndLength / 2 + detail.width / 2)
                        {
                            detail.minZOffset = -mazeTileWidthAndLength / 2 + detail.width / 2;
                        }
                        if (detail.maxZOffset < -mazeTileWidthAndLength / 2 + detail.width / 2)
                        {
                            detail.maxZOffset = -mazeTileWidthAndLength / 2 + detail.width / 2;
                        }
                        if (detail.minZOffset > mazeTileWidthAndLength / 2 - detail.width / 2)
                        {
                            detail.minZOffset = mazeTileWidthAndLength / 2 - detail.width / 2;
                        }
                        if (detail.maxZOffset > mazeTileWidthAndLength / 2 - detail.width / 2)
                        {
                            detail.maxZOffset = mazeTileWidthAndLength / 2 - detail.width / 2;
                        }
                        if (detail.maxZOffset < detail.minZOffset)
                        {
                            detail.maxZOffset = detail.minZOffset;
                        }
                    }
                }

                foreach (TwoDimensionalDetail detail in twoDimensionalDetails)
                {
                    if (detail.detailPrefab)
                    {
                        //Name string Check
                        if (string.IsNullOrEmpty(detail.detailName))
                        {
                            detail.detailName = detail.detailPrefab.name;
                        }

                        //x-axis clamping
                        if (detail.minXOffset < -mazeTileWidthAndLength / 2 + detail.width / 2)
                        {
                            detail.minXOffset = -mazeTileWidthAndLength / 2 + detail.width / 2;
                        }
                        if (detail.maxXOffset < -mazeTileWidthAndLength / 2 + detail.width / 2)
                        {
                            detail.maxXOffset = -mazeTileWidthAndLength / 2 + detail.width / 2;
                        }
                        if (detail.minXOffset > mazeTileWidthAndLength / 2 - detail.width / 2)
                        {
                            detail.minXOffset = mazeTileWidthAndLength / 2 - detail.width / 2;
                        }
                        if (detail.maxXOffset > mazeTileWidthAndLength / 2 - detail.width / 2)
                        {
                            detail.maxXOffset = mazeTileWidthAndLength / 2 - detail.width / 2;
                        }
                        if (detail.maxXOffset < detail.minXOffset)
                        {
                            detail.maxXOffset = detail.minXOffset;
                        }

                        //y-axis clamping
                        if (detail.minYOffset < -mazeTileWidthAndLength / 2 + detail.width / 2)
                        {
                            detail.minYOffset = -mazeTileWidthAndLength / 2 + detail.width / 2;
                        }
                        if (detail.maxYOffset < -mazeTileWidthAndLength / 2 + detail.width / 2)
                        {
                            detail.maxYOffset = -mazeTileWidthAndLength / 2 + detail.width / 2;
                        }
                        if (detail.minYOffset > mazeTileWidthAndLength / 2 - detail.width / 2)
                        {
                            detail.minYOffset = mazeTileWidthAndLength / 2 - detail.width / 2;
                        }
                        if (detail.maxYOffset > mazeTileWidthAndLength / 2 - detail.width / 2)
                        {
                            detail.maxYOffset = mazeTileWidthAndLength / 2 - detail.width / 2;
                        }
                        if (detail.maxYOffset < detail.minYOffset)
                        {
                            detail.maxYOffset = detail.minYOffset;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates a new maze in a single frame
        /// </summary>
        /// <returns>Returns the root GameObject of the generated maze which will have the Maze component</returns>
        public GameObject GenerateNewMaze()
        {
            int rowTracker;
            int columnTracker;
            int indexTracker;
            bool keepGoing;

            StartMazeMap(); //starts the maze map at a blank slate
            Vector2Int currentTileCoordinates = new Vector2Int(1, 1); //sets the current tile to the bottom-left corner

            //if we're not using a pre-set seed value, it needs to be set now
            if (!useSeedValue)
            {
                seedValue = Random.Range(int.MinValue, int.MaxValue);
            }
            Random.InitState(seedValue);

            keepGoing = true; //a bool that creates a gate for the following loop to exit              
            do
            {
                keepGoing = GetNextTile(latestMazeMap, ref currentTileCoordinates); //uses RNGesus to find a new tile and generate the maze one tile at a time, when finished, keepGoing will be false
            } while (keepGoing);

            if (makeBraidMaze)
            {
                rowTracker = 0;
                columnTracker = 0;
                keepGoing = true;
                do
                {
                    keepGoing = EraseSomeDeadEnds(latestMazeMap, 10000, ref rowTracker, ref columnTracker); //erases dead ends
                } while (keepGoing);
            }

            if (deleteOutsideWalls)
            {
                EraseSomeOuterWalls(latestMazeMap, outsideWallPiecesToDelete, outsideWallDeleteMode);
            }

            if (Application.isPlaying)
            {
                CheckDuplicateMazeNames(mazeName);
            }
            theMaze = new GameObject(mazeName, typeof(Maze)); //either we've checked for duplicate names here or in editor code, so now is a safe time to create the new GameObject
            theMaze.transform.position = mazePosition; //since we're generating a new maze, better move it to the right spot

            if (makePlazas && plazas.Length > 0)
            {
                for (int counter = 0; counter < numberOfPlazas; counter++)
                {
                    bool validAvailableChoice = false;
                    foreach (PlazaPrefab plaza in plazas)
                    {
                        if (plaza)
                        {
                            if (!plaza.fixedPosition || !plaza.placedOnce)
                            {
                                validAvailableChoice = true;
                                break;
                            }
                        }
                    }
                    if (validAvailableChoice)
                    {
                        PlazaPrefab chosenPlaza = plazas[Random.Range(0, plazas.Length)];
                        while (chosenPlaza.fixedPosition && chosenPlaza.placedOnce)
                        {
                            chosenPlaza = plazas[Random.Range(0, plazas.Length)];
                        }
                        keepGoing = true;
                        int timesTested = 0;
                        do
                        {
                            keepGoing = SetPlaza(latestMazeMap, chosenPlaza, theMaze.transform, mazeLength, mazeWidth, mazeDimension);
                            timesTested++;
                        } while (keepGoing && timesTested < plazaPlacementAttempts);
                    }
                }
                foreach (PlazaPrefab plaza in plazas)
                {
                    plaza.placedOnce = false;
                }
            }

            if (makeFloorExit)
            {
                SetStartOrFinish(latestMazeMap, exitType, false); //sets an exit point in the maze
            }

            if (makeStart)
            {
                SetStartOrFinish(latestMazeMap, startType, true); //sets a start point in the maze
            }

            if (makeUniqueTiles)
            {
                SetUniqueTiles(latestMazeMap, uniqueTiles);
            }

            if(differentCorners)
            {
                rowTracker = 0;
                columnTracker = 0;
                keepGoing = true;
                do
                {
                    keepGoing = FindCornerWalls(latestMazeMap, -1, ref rowTracker, ref columnTracker);
                } while (keepGoing);
            }

            if(differentEnds)
            {
                rowTracker = 0;
                columnTracker = 0;
                keepGoing = true;
                do
                {
                    keepGoing = FindEndWalls(latestMazeMap, -1, ref rowTracker, ref columnTracker);
                } while (keepGoing);
            }

            rowTracker = 0;
            columnTracker = 0;
            keepGoing = true;
            MazeNode[] nodes = new MazeNode[mazeWidth * mazeLength];
            do
            {
                if (mazeDimension == MazeDimension.TwoDimensional)
                {
                    keepGoing = InstantiateMaze2D(latestMazeMap, theMaze.transform, wallPieces2D, cornerWallPieces2D, endWallPieces2D, floorPieces2D, exitPieces, startPieces, mazeTileWidthAndLength, floorZPlane, removePlazaFloors, -1, ref rowTracker, ref columnTracker, nodes);
                }
                else
                {
                    keepGoing = InstantiateMaze(latestMazeMap, theMaze.transform, wallPieces, cornerWallPieces, endWallPieces, floorPieces, exitPieces, startPieces, mazeTileWidthAndLength, defaultFloorThickness, removePlazaFloors, -1, ref rowTracker, ref columnTracker, nodes);
                }
            } while (keepGoing);

            if (addDetails)
            {
                if (mazeDimension == MazeDimension.TwoDimensional)
                {
                    rowTracker = 0;
                    columnTracker = 0;
                    indexTracker = 0;
                    keepGoing = true;
                    do
                    {
                        keepGoing = PlaceRandomTwoDimensionalDetails(latestMazeMap, twoDimensionalDetails, mazeTileWidthAndLength, theMaze.transform, -1, ref indexTracker, ref rowTracker, ref columnTracker);
                    } while (keepGoing);
                }
                else
                {
                    rowTracker = 0;
                    columnTracker = 0;
                    indexTracker = 0;
                    keepGoing = true;
                    do
                    {
                        keepGoing = PlaceRandomWallDetails(latestMazeMap, wallDetails, mazeTileWidthAndLength, theMaze.transform, -1, ref indexTracker, ref rowTracker, ref columnTracker); //this adds details randomly throughout the maze (attached to the walls) based on the settings in wallDetails
                    } while (keepGoing);

                    rowTracker = 0;
                    columnTracker = 0;
                    indexTracker = 0;
                    keepGoing = true;
                    do
                    {
                        keepGoing = PlaceRandomOtherDetails(latestMazeMap, otherDetails, mazeTileWidthAndLength, theMaze.transform, -1, ref indexTracker, ref rowTracker, ref columnTracker); //this adds details randomly throughout the maze based on the settings in otherDetails
                    } while (keepGoing);
                }
            }

            Maze tempMaze = theMaze.GetComponent<Maze>();
            tempMaze.mazeMap = latestMazeMap; //stores the maze information in a mazeMap located on the maze itself
            tempMaze.mazeNodes = nodes;

            return theMaze;
        }

        /// <summary>
        /// NOTE: Will not work in the editor if called - it is also recommended that you only call this again after the first maze is finished or have multiple generators in the scene, although this isn't necessary
        /// Generates a new maze over a number of frames using a coroutine. Will take a "snapshot" of the settings on this MazeGenerator at the time it is called for generation over the coming frames. Cannot return the generated GameObject due to the process involved.
        /// </summary>
        /// <param name="calculationsPerFrame">The maximum number of calculations that will be made per frame - higher numbers will generate faster, but hurt framerate</param>
        /// <returns></returns>
        public void GenerateNewMazeIncremental(int calculationsPerFrame)
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("GenerateNewMazeIncremental(int calculationsPerFrame) CANNOT be called when the application isn't running. It will not work as intended due to the nature of coroutines.");
            }
            else if (calculationsPerFrame < 2)
            {
                Debug.LogError("GenerateNewMazeIncremental was passed an integer value less than 2. A minimum of 2 calculations per frame is required to generate the maze due to some checks and balances.");
            }
            else
            {
                StartCoroutine(GeneratingIncrementally(calculationsPerFrame));
            }
        }

        /// <summary>
        /// A coroutine that generates a maze incrementally. Can be started again before finished.
        /// </summary>
        /// <param name="calculationsPerFrame">The maximum number of calculations that will be made per frame - higher numbers will generate faster, but hurt framerate</param>
        private IEnumerator GeneratingIncrementally(int calculationsPerFrame)
        {
            int incrementIndex; //used later to increment placement
            int incrementRow; //used later to increment placement and generation
            int incrementColumn; //used later to increment placement and generation
            bool keepGoing;
            MazeDimension stored_mazeDimension = mazeDimension;
            bool stored_useSeedValue = useSeedValue;
            int stored_seedValue = seedValue;
            int stored_mazeWidth = mazeWidth;
            int stored_mazeLength = mazeLength;
            Vector3 stored_mazePosition = mazePosition;
            bool stored_makeBraidMaze = makeBraidMaze;
            bool stored_makeFloorExit = makeFloorExit;
            bool stored_makeStart = makeStart;
            bool stored_makeUniqueTiles = makeUniqueTiles;
            FloorExitType stored_exitType = exitType;
            FloorExitType stored_startType = startType;
            bool stored_deleteOutsideWalls = deleteOutsideWalls;
            int stored_outsideWallPiecesToDelete = outsideWallPiecesToDelete;
            OutsideWallDeleteMode stored_outsideWallDeleteMode = outsideWallDeleteMode;
            bool stored_makePlazas = makePlazas;
            bool stored_removePlazaFloors = removePlazaFloors;
            int stored_numberOfPlazas = numberOfPlazas;
            int stored_plazaPlacementAttempts = plazaPlacementAttempts;

            PlazaPrefab[] stored_plazas = new PlazaPrefab[plazas.Length];
            plazas.CopyTo(stored_plazas, 0);

            float stored_mazeTileWidthAndLength = mazeTileWidthAndLength;
            float stored_defaultFloorThickness = defaultFloorThickness;
            float stored_floorZPlane = floorZPlane;

            WallPiece[] stored_wallPieces = new WallPiece[wallPieces.Length];
            wallPieces.CopyTo(stored_wallPieces, 0);

            WallPiece[] stored_cornerWallPieces = new WallPiece[cornerWallPieces.Length];
            cornerWallPieces.CopyTo(stored_cornerWallPieces, 0);

            WallPiece[] stored_endWallPieces = new WallPiece[endWallPieces.Length];
            endWallPieces.CopyTo(stored_endWallPieces, 0);

            WallPiece2D[] stored_wallPieces2D = new WallPiece2D[wallPieces2D.Length];
            wallPieces2D.CopyTo(stored_wallPieces2D, 0);

            WallPiece2D[] stored_cornerWallPieces2D = new WallPiece2D[cornerWallPieces2D.Length];
            cornerWallPieces2D.CopyTo(stored_cornerWallPieces2D, 0);

            WallPiece2D[] stored_endWallPieces2D = new WallPiece2D[endWallPieces2D.Length];
            endWallPieces2D.CopyTo(stored_endWallPieces2D, 0);

            FloorPiece[] stored_floorPieces = new FloorPiece[floorPieces.Length];
            floorPieces.CopyTo(stored_floorPieces, 0);

            GameObject[] stored_floorPieces2D = new GameObject[floorPieces2D.Length];
            floorPieces2D.CopyTo(stored_floorPieces2D, 0);

            GameObject[] stored_exitPieces = new GameObject[exitPieces.Length];
            exitPieces.CopyTo(stored_exitPieces, 0);

            GameObject[] stored_startPieces = new GameObject[startPieces.Length];
            startPieces.CopyTo(stored_startPieces, 0);

            UniqueTile[] stored_uniqueTiles = new UniqueTile[uniqueTiles.Length];
            uniqueTiles.CopyTo(stored_uniqueTiles, 0);

            bool stored_addDetails = addDetails;

            TwoDimensionalDetail[] stored_twoDimensionalDetails = new TwoDimensionalDetail[twoDimensionalDetails.Length];
            twoDimensionalDetails.CopyTo(stored_twoDimensionalDetails, 0);

            WallDetail[] stored_wallDetails = new WallDetail[wallDetails.Length];
            wallDetails.CopyTo(stored_wallDetails, 0);

            OtherDetail[] stored_otherDetails = new OtherDetail[otherDetails.Length];
            otherDetails.CopyTo(stored_otherDetails, 0);

            string stored_mazeName = mazeName;

            //Starts the new maze map off with the right values
            MazeMap stored_mazeMap = new MazeMap();
            //sets the width and height properties of the maze map to make sure they're current
            stored_mazeMap.mazeWidth = stored_mazeWidth;
            stored_mazeMap.mazeLength = stored_mazeLength;

            //creates a new map
            stored_mazeMap.map = new MazeMap.TileState[(stored_mazeMap.mazeWidth) * (stored_mazeMap.mazeLength)];
            for (int row = 0; row < stored_mazeMap.mazeLength; row++)
            {
                for (int column = 0; column < stored_mazeMap.mazeWidth; column++)
                {
                    if (row % 2 == 0 || column % 2 == 0)
                    {
                        //sets walls on every second square (looks like a grid)
                        stored_mazeMap.map[(row * stored_mazeMap.mazeWidth) + column] = MazeMap.TileState.wall;
                    }
                    else
                    {
                        //flags every empty space as unexplored
                        stored_mazeMap.map[(row * stored_mazeMap.mazeWidth) + column] = MazeMap.TileState.unexplored;
                    }
                }
            }
            Vector2Int stored_currentTile = new Vector2Int(1, 1); //sets the current tile to the bottom-left corner

            //if we're not using a pre-set seed value, it needs to be set now
            if (!stored_useSeedValue)
            {
                stored_seedValue = Random.Range(int.MinValue, int.MaxValue);
            }
            Random.InitState(stored_seedValue);

            keepGoing = true;
            //will keep generating until the maze is finished                  
            do
            {
                int counter = 0;
                do
                {
                    keepGoing = GetNextTile(stored_mazeMap, ref stored_currentTile);
                    counter++;
                } while (counter < calculationsPerFrame && keepGoing);
                yield return null;
            } while (keepGoing);

            if (stored_makeBraidMaze)
            {
                incrementRow = 0;
                incrementColumn = 0;
                keepGoing = true;
                do
                {
                    // erases all of the dead ends throughout the maze
                    keepGoing = EraseSomeDeadEnds(stored_mazeMap, calculationsPerFrame, ref incrementRow, ref incrementColumn);
                    yield return null;
                } while (keepGoing);
            }

            if (stored_deleteOutsideWalls)
            {
                EraseSomeOuterWalls(stored_mazeMap, stored_outsideWallPiecesToDelete, stored_outsideWallDeleteMode);
            }

            // delete mazes with the same name
            CheckDuplicateMazeNames(stored_mazeName);
            // make a new maze
            GameObject stored_theMaze = new GameObject(stored_mazeName, typeof(Maze));
            // move the maze to the correct position
            stored_theMaze.transform.position = stored_mazePosition;

            if (stored_makePlazas && stored_plazas.Length > 0)
            {
                int calculations = 0;
                for (int counter = 0; counter < stored_numberOfPlazas; counter++)
                {
                    bool validAvailableChoice = false;
                    foreach (PlazaPrefab plaza in stored_plazas)
                    {
                        if (plaza)
                        {
                            if (!plaza.fixedPosition || !plaza.placedOnce)
                            {
                                validAvailableChoice = true;
                                break;
                            }
                        }
                    }

                    if (validAvailableChoice)
                    {
                        PlazaPrefab chosenPlaza = stored_plazas[Random.Range(0, plazas.Length)];
                        while (chosenPlaza.fixedPosition && chosenPlaza.placedOnce)
                        {
                            chosenPlaza = plazas[Random.Range(0, plazas.Length)];
                            calculations++;
                            if (calculations >= calculationsPerFrame)
                            {
                                calculations = 0;
                                yield return null;
                            }
                        }
                        keepGoing = true;
                        int timesTested = 0;
                        do
                        {
                            keepGoing = SetPlaza(stored_mazeMap, chosenPlaza, stored_theMaze.transform, stored_mazeLength, stored_mazeWidth, stored_mazeDimension);
                            timesTested++;
                            calculations++;
                            if (calculations >= calculationsPerFrame)
                            {
                                calculations = 0;
                                yield return null;
                            }
                        } while (keepGoing && timesTested < stored_plazaPlacementAttempts);
                    }
                    else
                    {
                        break;
                    }
                }
                foreach (PlazaPrefab plaza in stored_plazas)
                {
                    plaza.placedOnce = false;
                }
            }

            if (stored_makeFloorExit)
            {
                //sets an exit point in the maze
                SetStartOrFinish(stored_mazeMap, stored_exitType, false);
            }

            if (stored_makeStart)
            {
                //sets a start point in the maze
                SetStartOrFinish(stored_mazeMap, stored_startType, true);
            }

            if (stored_makeUniqueTiles)
            {
                //sets unique tiles throughout the maze
                SetUniqueTiles(stored_mazeMap, stored_uniqueTiles);
            }

            incrementRow = 0;
            incrementColumn = 0;
            keepGoing = true;
            MazeNode[] nodes = new MazeNode[stored_mazeWidth * stored_mazeLength];
            do
            {
                if (mazeDimension == MazeDimension.TwoDimensional)
                {
                    keepGoing = InstantiateMaze2D(stored_mazeMap, stored_theMaze.transform, stored_wallPieces2D, stored_cornerWallPieces2D, stored_endWallPieces2D, stored_floorPieces2D, stored_exitPieces, stored_startPieces, stored_mazeTileWidthAndLength, stored_floorZPlane, stored_removePlazaFloors, calculationsPerFrame, ref incrementRow, ref incrementColumn, nodes);
                }
                else
                {
                    keepGoing = InstantiateMaze(stored_mazeMap, stored_theMaze.transform, stored_wallPieces, stored_cornerWallPieces, stored_endWallPieces, stored_floorPieces, stored_exitPieces, stored_startPieces, stored_mazeTileWidthAndLength, stored_defaultFloorThickness, stored_removePlazaFloors, calculationsPerFrame, ref incrementRow, ref incrementColumn, nodes);
                }
                yield return null;
            } while (keepGoing);

            if (stored_addDetails)
            {
                if (mazeDimension == MazeDimension.TwoDimensional)
                {
                    incrementRow = 0;
                    incrementColumn = 0;
                    incrementIndex = 0;
                    keepGoing = true;
                    do
                    {
                        keepGoing = PlaceRandomTwoDimensionalDetails(stored_mazeMap, stored_twoDimensionalDetails, stored_mazeTileWidthAndLength, stored_theMaze.transform, calculationsPerFrame, ref incrementIndex, ref incrementRow, ref incrementColumn);
                        yield return null;
                    } while (keepGoing);
                }
                else
                {
                    incrementRow = 0;
                    incrementColumn = 0;
                    incrementIndex = 0;
                    keepGoing = true;
                    do
                    {
                        keepGoing = PlaceRandomWallDetails(stored_mazeMap, stored_wallDetails, stored_mazeTileWidthAndLength, stored_theMaze.transform, calculationsPerFrame, ref incrementIndex, ref incrementRow, ref incrementColumn); //this adds details randomly throughout the maze (attached to the walls) based on the settings in wallDetails
                        yield return null;
                    } while (keepGoing);

                    incrementRow = 0;
                    incrementColumn = 0;
                    incrementIndex = 0;
                    keepGoing = true;
                    do
                    {
                        keepGoing = PlaceRandomOtherDetails(stored_mazeMap, stored_otherDetails, stored_mazeTileWidthAndLength, stored_theMaze.transform, calculationsPerFrame, ref incrementIndex, ref incrementRow, ref incrementColumn); //this adds details randomly throughout the maze based on the settings in otherDetails
                        yield return null;
                    } while (keepGoing);
                }
            }

            Maze tempMaze = stored_theMaze.GetComponent<Maze>();
            tempMaze.mazeMap = stored_mazeMap; //stores the maze information in a mazeMap located on the maze itself
            tempMaze.mazeNodes = nodes;
        }

        /// <summary>
        /// Clears the current map and starts a new one (editor generation only)
        /// </summary>
        private void StartMazeMap()
        {
            latestMazeMap = new MazeMap();
            //sets the width and height properties of the maze map to make sure they're current
            latestMazeMap.mazeWidth = mazeWidth;
            latestMazeMap.mazeLength = mazeLength;
            latestMazeMap.map = new MazeMap.TileState[(latestMazeMap.mazeWidth) * (latestMazeMap.mazeLength)]; //creates a new map
            for (int row = 0; row < latestMazeMap.mazeLength; row++)
            {
                for (int column = 0; column < latestMazeMap.mazeWidth; column++)
                {
                    if (row % 2 == 0 || column % 2 == 0)
                    {
                        latestMazeMap.map[(row * latestMazeMap.mazeWidth) + column] = MazeMap.TileState.wall; //sets walls on every second square (looks like a grid)
                    }
                    else
                    {
                        latestMazeMap.map[(row * latestMazeMap.mazeWidth) + column] = MazeMap.TileState.unexplored; //flags every empty space as unexplored
                    }
                }
            }
        }

        /// <summary>
        /// Finds a new, valid, tile to look at
        /// </summary>
        /// <returns>True if not finished, and false if the maze is finished generating</returns>
        private bool GetNextTile(MazeMap mazeMapToCheck, ref Vector2Int currentTileCoordinates)
        {
            //declares and initializes the values to meaningless states
            bool foundValidTile = false; //have we found a valid tile yet?
            bool checkingDeadEnds = false; //changes how a second look at surrounding tiles is processed (i.e. backtracking along path)
            MazeMap.TileState testResults = MazeMap.TileState.outOfBounds; //the results of looking in the above direction
            Vector2Int move = Vector2Int.zero; //if we've found a valid tile, the move we have to make to get there
            bool keepGoing = true; //assume that the generation process hasn't finished
            testResults = MazeMap.TileState.outOfBounds; //default the results to out of bounds

            //generate list of directions and shuffle it for more efficient random directions
            List<TestDirection> testDirections = new List<TestDirection> { TestDirection.left, TestDirection.up, TestDirection.right, TestDirection.down };
            for (int index = 0; index < testDirections.Count; index++)
            {
                int newIndex = Random.Range(0, 4);
                TestDirection temp = testDirections[newIndex];
                testDirections[newIndex] = testDirections[index];
                testDirections[index] = temp;
            }

            for (int directionIndex = 0; directionIndex < testDirections.Count && !foundValidTile; directionIndex++)
            {
                //choose offset from current tile based on given direction
                //when backtracking, we have to go one space at a time, not jump by twos
                switch(testDirections[directionIndex])
                {
                    case TestDirection.left: //one is left
                        move = checkingDeadEnds ? Vector2Int.left : Vector2Int.left * 2;
                        break;

                    case TestDirection.up: //two is up
                        move = checkingDeadEnds ? Vector2Int.up : Vector2Int.up * 2;
                        break;

                    case TestDirection.right: //three is right
                        move = checkingDeadEnds ? Vector2Int.right : Vector2Int.right * 2;
                        break;

                    case TestDirection.down: //four is down
                        move = checkingDeadEnds ? Vector2Int.down : Vector2Int.down * 2;
                        break;
                }

                testResults = CheckTile(move, mazeMapToCheck, currentTileCoordinates); //check the state of the tile in the given direction

                //the first round of checks needs to find a completely new, unexplored tile to make sure we don't have any "dead zones" in the maze that don't get developed
                if (!checkingDeadEnds)
                {
                    //if we've found an unexplored tile, we're done and can break down the wall between here and there as well as marking this tile as visited
                    if (testResults == MazeMap.TileState.unexplored)
                    {
                        foundValidTile = true;
                        mazeMapToCheck.map[((currentTileCoordinates.y) * mazeMapToCheck.mazeWidth) + (currentTileCoordinates.x)] = MazeMap.TileState.visitedOnce; //flags the current tile as visited
                        mazeMapToCheck.map[((currentTileCoordinates.y + move.y / 2) * mazeMapToCheck.mazeWidth) + (currentTileCoordinates.x + move.x / 2)] = MazeMap.TileState.brokenWall; //flags the wall you moved through as broken down
                        currentTileCoordinates += move; //moves the "cursor" to the new spot
                    }

                    //if we didn't find any unexplored tiles in any direction, we can start the backtracking process
                    if (directionIndex == testDirections.Count - 1 && !foundValidTile)
                    {
                        checkingDeadEnds = true;
                        directionIndex = -1;
                    }
                }
                //the second round of checks just needs to find a tile that's only been visited once, or a broken wall (this is backtracking)
                else
                {
                    if (testResults == MazeMap.TileState.visitedOnce || testResults == MazeMap.TileState.brokenWall)
                    {
                        foundValidTile = true;
                        if (mazeMapToCheck.map[((currentTileCoordinates.y) * mazeMapToCheck.mazeWidth) + (currentTileCoordinates.x)] == MazeMap.TileState.unexplored)
                        {
                            mazeMapToCheck.map[((currentTileCoordinates.y) * mazeMapToCheck.mazeWidth) + (currentTileCoordinates.x)] = MazeMap.TileState.deadEnd; //marks our dead ends in case we want to make a braid maze
                        }
                        else
                        {
                            mazeMapToCheck.map[((currentTileCoordinates.y) * mazeMapToCheck.mazeWidth) + (currentTileCoordinates.x)] = MazeMap.TileState.visitedTwice; //marks tiles we've visited twice so that we don't go back to them
                        }
                        currentTileCoordinates += move;
                    }

                    //if we've checked every direction and there aren't any valid tiles, the generation process has finished
                    if (directionIndex == testDirections.Count - 1 && !foundValidTile)
                    {
                        mazeMapToCheck.map[((currentTileCoordinates.y) * mazeMapToCheck.mazeWidth) + (currentTileCoordinates.x)] = MazeMap.TileState.deadEnd; //marks our dead ends in case we want to make a braid maze
                        keepGoing = false;
                    }
                }
            }
            return keepGoing;
        }

        /// <summary>
        /// Erases some dead ends if you want a braid maze. Does so over a number of frames influenced by the provided calculationsPerFrame variable
        /// </summary>
        /// <param name="mapToModify">The MazeMap being edited to remove dead ends</param>
        /// <param name="calculationsPerFrame">The number of calculations we can perform per frame. A value of 0 or lower means to continue until finished.</param>
        /// <param name="row">Passed by reference to keep track of position in looping through 2D array</param>
        /// <param name="column">Passed by reference to keep track of position in looping through 2D array</param>
        /// <returns>Returns true if it should be called again, returns false if it has finished</returns>
        private bool EraseSomeDeadEnds(MazeMap mapToModify, int calculationsPerFrame, ref int row, ref int column)
        {
            MazeMap.TileState floorResults = MazeMap.TileState.outOfBounds;
            MazeMap.TileState wallResults = MazeMap.TileState.outOfBounds;
            Vector2Int move = Vector2Int.zero;
            bool keepGoing = true;
            int calculationCounter = 0;

            //looks through the maze for all dead ends flagged during the generation process
            for (; row < mapToModify.mazeLength && (calculationCounter < calculationsPerFrame || calculationsPerFrame < 1); row++)
            {
                for (; column < mapToModify.mazeWidth && (calculationCounter < calculationsPerFrame || calculationsPerFrame < 1); column++)
                {
                    if (mapToModify.map[(row * mapToModify.mazeWidth) + column] == MazeMap.TileState.deadEnd)
                    {
                        Vector2Int currentTileCoordinates = new Vector2Int(column, row); //sets the current tile variable to the current tile (the dead end that's been found)
                        TestDirection testDirection = (TestDirection)1; //start testing at the lowest value (1 is left)
                        bool foundIt = false; //used to indicate when the wall that needs to be broken down has been found
                        while ((int)testDirection <= 4 && !foundIt)
                        {
                            switch (testDirection)
                            {
                                case TestDirection.left: //one is left
                                    move = Vector2Int.left;
                                    break;

                                case TestDirection.up: //two is up
                                    move = Vector2Int.up;
                                    break;

                                case TestDirection.right: //three is right
                                    move = Vector2Int.right;
                                    break;

                                case TestDirection.down: //four is down
                                    move = Vector2Int.down;
                                    break;
                            }
                            wallResults = CheckTile(move, mapToModify, currentTileCoordinates);
                            floorResults = CheckTile(move * 2, mapToModify, currentTileCoordinates);
                            
                            //if the tile we found is a 
                            if (IsFloor(floorResults) && IsWall(wallResults))
                            {
                                foundIt = true;
                                mapToModify.map[((currentTileCoordinates.y + move.y) * mapToModify.mazeWidth) + (currentTileCoordinates.x + move.x)] = MazeMap.TileState.brokenWall;
                            }
                            testDirection++;
                        }
                    }
                    calculationCounter++;
                }
                if (calculationCounter >= calculationsPerFrame && calculationsPerFrame > 0)
                {
                    column--;
                }
                else if (column >= mapToModify.mazeWidth)
                {
                    column = 0;
                }
            }
            if (calculationCounter >= calculationsPerFrame && calculationsPerFrame > 0)
            {
                row--;
            }
            else if (row >= mapToModify.mazeLength)
            {
                keepGoing = false;
            }
            return keepGoing;
        }

        /// <summary>
        /// Erases pieces of the outer wall. Presumably to be used as entrances and exits.
        /// </summary>
        /// <param name="mapToModify">The maze map we're modifying.</param>
        private void EraseSomeOuterWalls(MazeMap mapToModify, int numberToDelete, OutsideWallDeleteMode deleteMode)
        {
            // all of bottom except the bottom-left corner
            List<int> randomColBottom = new List<int>();
            for (int count = 1; count < mapToModify.mazeWidth; count++)
            {
                randomColBottom.Add(count);
            }
            // randomly permute possibilities
            for (int index = 0; index < randomColBottom.Count; index++)
            {
                int rnGesus = Random.Range(0, randomColBottom.Count);
                int temp = randomColBottom[index];
                randomColBottom[index] = randomColBottom[rnGesus];
                randomColBottom[rnGesus] = temp;
            }

            // all of top except the top-right corner
            List<int> randomColTop = new List<int>();
            for (int count = 0; count < mapToModify.mazeWidth - 1; count++)
            {
                randomColTop.Add(count);
            }
            // randomly permute possibilities
            for (int index = 0; index < randomColTop.Count; index++)
            {
                int rnGesus = Random.Range(0, randomColTop.Count);
                int temp = randomColTop[index];
                randomColTop[index] = randomColTop[rnGesus];
                randomColTop[rnGesus] = temp;
            }

            // all of left except the top-left corner
            List<int> randomRowLeft = new List<int>();
            for (int count = 0; count < mapToModify.mazeLength -1; count++)
            {
                randomRowLeft.Add(count);
            }
            // randomly permute possibilities
            for (int index = 0; index < randomRowLeft.Count; index++)
            {
                int rnGesus = Random.Range(0, randomRowLeft.Count);
                int temp = randomRowLeft[index];
                randomRowLeft[index] = randomRowLeft[rnGesus];
                randomRowLeft[rnGesus] = temp;
            }

            // all of right except the bottom-right corner
            List<int> randomRowRight = new List<int>();
            for (int count = 1; count < mapToModify.mazeLength; count++)
            {
                randomRowRight.Add(count);
            }
            // randomly permute possibilities
            for (int index = 0; index < randomRowRight.Count; index++)
            {
                int rnGesus = Random.Range(0, randomRowRight.Count);
                int temp = randomRowRight[index];
                randomRowRight[index] = randomRowRight[rnGesus];
                randomRowRight[rnGesus] = temp;
            }

            switch (deleteMode)
            {
                case OutsideWallDeleteMode.random:
                    for (int count = 0; count < numberToDelete; count++)
                    {
                        // 0 is left wall
                        // 1 is top wall
                        // 2 is right wall
                        // 3 is bottom wall
                        int rnGesus = Random.Range(0, 4);
                        switch(rnGesus)
                        {
                            case 0:
                                if (randomRowLeft.Count > 0)
                                {
                                    int leftRow = randomRowLeft[randomRowLeft.Count - 1];
                                    randomRowLeft.RemoveAt(randomRowLeft.Count - 1);
                                    mapToModify.map[(leftRow * mapToModify.mazeWidth) + 0] = MazeMap.TileState.brokenWall;
                                    // make sure this space is accessible
                                    if (IsWall(mapToModify.map[(leftRow * mapToModify.mazeWidth) + 1]))
                                    {
                                        mapToModify.map[(leftRow * mapToModify.mazeWidth) + 1] = MazeMap.TileState.brokenWall;
                                    }
                                }
                                else
                                {
                                    count--;
                                }
                                break;
                            case 1:
                                if (randomColTop.Count > 0)
                                {
                                    int topCol = randomColTop[randomColTop.Count - 1];
                                    randomColTop.RemoveAt(randomColTop.Count - 1);
                                    mapToModify.map[((mapToModify.mazeLength - 1) * mapToModify.mazeWidth) + topCol] = MazeMap.TileState.brokenWall;
                                    // make sure this space is accessible
                                    if (IsWall(mapToModify.map[((mapToModify.mazeLength - 2) * mapToModify.mazeWidth) + topCol]))
                                    {
                                        mapToModify.map[((mapToModify.mazeLength - 2) * mapToModify.mazeWidth) + topCol] = MazeMap.TileState.brokenWall;
                                    }
                                }
                                else
                                {
                                    count--;
                                }
                                break;
                            case 2:
                                if (randomRowRight.Count > 0)
                                {
                                    int rightRow = randomRowRight[randomRowRight.Count - 1];
                                    randomRowRight.RemoveAt(randomRowRight.Count - 1);
                                    mapToModify.map[(rightRow * mapToModify.mazeWidth) + mapToModify.mazeWidth-1] = MazeMap.TileState.brokenWall;
                                    // make sure this space is accessible
                                    if (IsWall(mapToModify.map[(rightRow * mapToModify.mazeWidth) + mapToModify.mazeWidth - 2]))
                                    {
                                        mapToModify.map[(rightRow * mapToModify.mazeWidth) + mapToModify.mazeWidth - 2] = MazeMap.TileState.brokenWall;
                                    }
                                }
                                else
                                {
                                    count--;
                                }
                                break;
                            case 3:
                                if (randomColBottom.Count > 0)
                                {
                                    int bottomCol = randomColBottom[randomColBottom.Count - 1];
                                    randomColBottom.RemoveAt(randomColBottom.Count - 1);
                                    mapToModify.map[(0 * mapToModify.mazeWidth) + bottomCol] = MazeMap.TileState.brokenWall;
                                    // make sure this space is accessible
                                    if (IsWall(mapToModify.map[(1 * mapToModify.mazeWidth) + bottomCol]))
                                    {
                                        mapToModify.map[(1 * mapToModify.mazeWidth) + bottomCol] = MazeMap.TileState.brokenWall;
                                    }
                                }
                                else
                                {
                                    count--;
                                }
                                break;
                        }
                    }
                    break;
                case OutsideWallDeleteMode.opposite:
                    for (int count = 0; count < numberToDelete; count += 2)
                    {
                        int rnGesus = Random.Range(0, 2);
                        switch(rnGesus)
                        {
                            // break random, opposite walls on the left and right
                            case 0:
                                if (randomRowRight.Count > 0 && randomRowLeft.Count > 0)
                                {
                                    int leftRow = randomRowLeft[randomRowLeft.Count - 1];
                                    randomRowLeft.RemoveAt(randomRowLeft.Count - 1);
                                    mapToModify.map[(leftRow * mapToModify.mazeWidth) + 0] = MazeMap.TileState.brokenWall;
                                    // make sure this space is accessible
                                    if (IsWall(mapToModify.map[(leftRow * mapToModify.mazeWidth) + 1]))
                                    {
                                        mapToModify.map[(leftRow * mapToModify.mazeWidth) + 1] = MazeMap.TileState.brokenWall;
                                    }

                                    int rightRow = randomRowRight[randomRowRight.Count - 1];
                                    randomRowRight.RemoveAt(randomRowRight.Count - 1);
                                    mapToModify.map[(rightRow * mapToModify.mazeWidth) + mapToModify.mazeWidth-1] = MazeMap.TileState.brokenWall;
                                    // make sure this space is accessible
                                    if (IsWall(mapToModify.map[(rightRow * mapToModify.mazeWidth) + mapToModify.mazeWidth - 2]))
                                    {
                                        mapToModify.map[(rightRow * mapToModify.mazeWidth) + mapToModify.mazeWidth - 2] = MazeMap.TileState.brokenWall;
                                    }
                                }
                                else
                                {
                                    // we don't have any walls to delete here, so override the randomness
                                    if (randomColTop.Count > 0 && randomColBottom.Count > 0)
                                    {
                                        goto case 1;
                                    }
                                    else
                                    {
                                        Debug.LogWarning("This is weird.");
                                        return;
                                    }
                                }
                                break;
                            // break random, opposite walls on the top and bottom
                            case 1:
                                if (randomColTop.Count > 0 && randomColBottom.Count > 0)
                                {
                                    int topCol = randomColTop[randomColTop.Count - 1];
                                    randomColTop.RemoveAt(randomColTop.Count - 1);
                                    mapToModify.map[((mapToModify.mazeLength - 1) * mapToModify.mazeWidth) + topCol] = MazeMap.TileState.brokenWall;
                                    // make sure this space is accessible
                                    if (IsWall(mapToModify.map[((mapToModify.mazeLength - 2) * mapToModify.mazeWidth) + topCol]))
                                    {
                                        mapToModify.map[((mapToModify.mazeLength - 2) * mapToModify.mazeWidth) + topCol] = MazeMap.TileState.brokenWall;
                                    }

                                    int bottomCol = randomColBottom[randomColBottom.Count - 1];
                                    mapToModify.map[(0 * mapToModify.mazeWidth) + bottomCol] = MazeMap.TileState.brokenWall;
                                    randomColBottom.RemoveAt(randomColBottom.Count - 1);
                                    // make sure this space is accessible
                                    if (IsWall(mapToModify.map[(1 * mapToModify.mazeWidth) + bottomCol]))
                                    {
                                        mapToModify.map[(1 * mapToModify.mazeWidth) + bottomCol] = MazeMap.TileState.brokenWall;
                                    }
                                }
                                else
                                {
                                    // we don't have any walls to delete here, so override the randomness
                                    if (randomRowLeft.Count > 0 && randomRowRight.Count > 0)
                                    {
                                        goto case 0;
                                    }
                                    else
                                    {
                                        Debug.LogWarning("This is weird.");
                                        return;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case OutsideWallDeleteMode.symmetric:
                    for (int count = 0; count < numberToDelete; count += 2)
                    {
                        // 0 is left/right
                        // 1 is top/bottom
                        int rnGesus = Random.Range(0, 2);
                        switch(rnGesus)
                        {
                            // break symmetrically opposite walls on the left and right
                            case 0:
                                if (randomRowLeft.Count > 0 && randomRowRight.Count > 0)
                                {
                                    int leftRow = randomRowLeft[randomRowLeft.Count - 1];
                                    randomRowLeft.RemoveAt(randomRowLeft.Count - 1);
                                    mapToModify.map[(leftRow * mapToModify.mazeWidth) + 0] = MazeMap.TileState.brokenWall;
                                    // make sure this space is accessible
                                    if (IsWall(mapToModify.map[(leftRow * mapToModify.mazeWidth) + 1]))
                                    {
                                        mapToModify.map[(leftRow * mapToModify.mazeWidth) + 1] = MazeMap.TileState.brokenWall;
                                    }

                                    int rightRow = mapToModify.mazeLength - 1 - leftRow;
                                    randomRowRight.Remove(rightRow);
                                    mapToModify.map[(rightRow * mapToModify.mazeWidth) + mapToModify.mazeWidth - 1] = MazeMap.TileState.brokenWall;
                                    // make sure this space is accessible
                                    if (IsWall(mapToModify.map[(rightRow * mapToModify.mazeWidth) + mapToModify.mazeWidth - 2]))
                                    {
                                        mapToModify.map[(rightRow * mapToModify.mazeWidth) + mapToModify.mazeWidth - 2] = MazeMap.TileState.brokenWall;
                                    }
                                }
                                else
                                {
                                    // we don't have any walls to delete here, so override the randomness
                                    if (randomColBottom.Count > 0 && randomColTop.Count > 0)
                                    {
                                        goto case 1;
                                    }
                                    else
                                    {
                                        Debug.LogWarning("This is weird.");
                                        return;
                                    }
                                }
                                break;
                            // break symmetrically opposite walls on the top and bottom
                            case 1:
                                if (randomColTop.Count > 0 && randomColBottom.Count > 0)
                                {
                                    int topCol = randomColTop[randomColTop.Count - 1];
                                    randomColTop.RemoveAt(randomColTop.Count - 1);
                                    mapToModify.map[((mapToModify.mazeLength - 1) * mapToModify.mazeWidth) + topCol] = MazeMap.TileState.brokenWall;
                                    // make sure this space is accessible
                                    if (IsWall(mapToModify.map[((mapToModify.mazeLength - 2) * mapToModify.mazeWidth) + topCol]))
                                    {
                                        mapToModify.map[((mapToModify.mazeLength - 2) * mapToModify.mazeWidth) + topCol] = MazeMap.TileState.brokenWall;
                                    }

                                    int bottomCol = mapToModify.mazeWidth - 1 - topCol;
                                    randomColBottom.Remove(bottomCol);
                                    mapToModify.map[(0 * mapToModify.mazeWidth) + bottomCol] = MazeMap.TileState.brokenWall;
                                    // make sure this space is accessible
                                    if (IsWall(mapToModify.map[(1 * mapToModify.mazeWidth) + bottomCol]))
                                    {
                                        mapToModify.map[(1 * mapToModify.mazeWidth) + bottomCol] = MazeMap.TileState.brokenWall;
                                    }
                                }
                                else
                                {
                                    // we don't have any walls to delete here, so override the randomness
                                    if (randomRowLeft.Count > 0 && randomRowRight.Count > 0)
                                    {
                                        goto case 0;
                                    }
                                    else
                                    {
                                        Debug.LogWarning("This is weird.");
                                        return;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Finds all wall pieces that are at the junctions of wall segments and labels them as such.
        /// </summary>
        /// <param name="mapToModify">The maze map we're modifying.</param>
        /// <param name="calculationsPerFrame">The number of calculations we can perform per frame. A value of 0 or lower means to continue until finished.</param>
        /// <param name="row">Passed by reference to keep track of position in looping through 2D array</param>
        /// <param name="column">Passed by reference to keep track of position in looping through 2D array</param>
        /// <returns>Returns true if it should be called again, returns false if it has finished</returns>
        private bool FindCornerWalls(MazeMap mapToModify, int calculationsPerFrame, ref int row, ref int column)
        {
            bool keepGoing = true;
            int calculationCounter = 0;

            //looks through the maze for all dead ends flagged during the generation process
            for (; row < mapToModify.mazeLength && (calculationCounter < calculationsPerFrame || calculationsPerFrame < 1); row++)
            {
                for (; column < mapToModify.mazeWidth && (calculationCounter < calculationsPerFrame || calculationsPerFrame < 1); column++)
                {
                    if (mapToModify.map[(row * mapToModify.mazeWidth) + column] == MazeMap.TileState.wall)
                    {
                        Vector2Int currentTileCoordinates = new Vector2Int(column, row);
                        // storing state of all surrounding tiles
                        MazeMap.TileState left = CheckTile(Vector2Int.left, mapToModify, currentTileCoordinates);
                        MazeMap.TileState right = CheckTile(Vector2Int.right, mapToModify, currentTileCoordinates);
                        MazeMap.TileState up = CheckTile(Vector2Int.up, mapToModify, currentTileCoordinates);
                        MazeMap.TileState down = CheckTile(Vector2Int.down, mapToModify, currentTileCoordinates);
                        // any tile that has a left or right neighbour AND an up or down neighbour is considered a corner wall
                        // if it does not have left and right OR up and down neighbour (which would be a t-intersection or cross-intersection)
                        if ((IsWall(left) ^ IsWall(right)) && (IsWall(up) ^ IsWall(down)))
                        {
                            mapToModify.map[(row * mapToModify.mazeWidth) + column] = MazeMap.TileState.cornerWall;
                        }
                    }
                    calculationCounter++;
                }
                if (calculationCounter >= calculationsPerFrame && calculationsPerFrame > 0)
                {
                    column--;
                }
                else if (column >= mapToModify.mazeWidth)
                {
                    column = 0;
                }
            }
            if (calculationCounter >= calculationsPerFrame && calculationsPerFrame > 0)
            {
                row--;
            }
            else if (row >= mapToModify.mazeLength)
            {
                keepGoing = false;
            }
            return keepGoing;
        }

        /// <summary>
        /// Finds all wall pieces that are at the ends of wall segments and labels them as such.
        /// </summary>
        /// <param name="mapToModify">The maze map we're modifying.</param>
        /// <param name="calculationsPerFrame">The number of calculations we can perform per frame. A value of 0 or lower means to continue until finished.</param>
        /// <param name="row">Passed by reference to keep track of position in looping through 2D array</param>
        /// <param name="column">Passed by reference to keep track of position in looping through 2D array</param>
        /// <returns>Returns true if it should be called again, returns false if it has finished</returns>
        private bool FindEndWalls(MazeMap mapToModify, int calculationsPerFrame, ref int row, ref int column)
        {
            bool keepGoing = true;
            int calculationCounter = 0;

            //looks through the maze for all dead ends flagged during the generation process
            for (; row < mapToModify.mazeLength && (calculationCounter < calculationsPerFrame || calculationsPerFrame < 1); row++)
            {
                for (; column < mapToModify.mazeWidth && (calculationCounter < calculationsPerFrame || calculationsPerFrame < 1); column++)
                {
                    if (mapToModify.map[(row * mapToModify.mazeWidth) + column] == MazeMap.TileState.wall)
                    {
                        Vector2Int currentTileCoordinates = new Vector2Int(column, row);
                        //storing state of all surrounding tiles
                        MazeMap.TileState left = CheckTile(Vector2Int.left, mapToModify, currentTileCoordinates);
                        MazeMap.TileState right = CheckTile(Vector2Int.right, mapToModify, currentTileCoordinates);
                        MazeMap.TileState up = CheckTile(Vector2Int.up, mapToModify, currentTileCoordinates);
                        MazeMap.TileState down = CheckTile(Vector2Int.down, mapToModify, currentTileCoordinates);
                        //tally up neighbouring walls
                        int neighbours = 0;
                        MazeMap.TileState newState = MazeMap.TileState.wall;
                        if (IsWall(up))
                        {
                            newState = MazeMap.TileState.endWallDown;
                            neighbours++;
                        }
                        if (IsWall(right))
                        {
                            newState = MazeMap.TileState.endWallLeft;
                            neighbours++;
                        }
                        if (IsWall(down))
                        {
                            newState = MazeMap.TileState.endWallUp;
                            neighbours++;
                        }
                        if (IsWall(left))
                        {
                            newState = MazeMap.TileState.endWallRight;
                            neighbours++;
                        }
                        //any wall that has 0 or 1 neighbours is considered an end wall
                        if (neighbours <= 1)
                        {
                            mapToModify.map[(row * mapToModify.mazeWidth) + column] = newState;
                        }
                    }
                    calculationCounter++;
                }
                if (calculationCounter >= calculationsPerFrame && calculationsPerFrame > 0)
                {
                    column--;
                }
                else if (column >= mapToModify.mazeWidth)
                {
                    column = 0;
                }
            }
            if (calculationCounter >= calculationsPerFrame && calculationsPerFrame > 0)
            {
                row--;
            }
            else if (row >= mapToModify.mazeLength)
            {
                keepGoing = false;
            }
            return keepGoing;
        }

        /// <summary>
        /// Checks the tile in the specified direction for it's state
        /// </summary>
        /// <returns>The state of the tile</returns>
        /// <param name="move">The vector 2 representing the place to check in relation to the current position</param>
        private MazeMap.TileState CheckTile(Vector2Int move, MazeMap mapToCheck, Vector2Int currentTileCoordinates)
        {
            //if we've moved out of the bounds of the maze, return out of bounds, otherwise return the tile information
            if (currentTileCoordinates.x + move.x < 0 || currentTileCoordinates.y + move.y < 0)
            {
                return MazeMap.TileState.outOfBounds;
            }
            else if (currentTileCoordinates.x + move.x >= mapToCheck.mazeWidth || currentTileCoordinates.y + move.y >= mapToCheck.mazeLength)
            {
                return MazeMap.TileState.outOfBounds;
            }
            else
            {
                return mapToCheck.map[((currentTileCoordinates.y + move.y) * mapToCheck.mazeWidth) + (currentTileCoordinates.x + move.x)];
            }
        }

        /// <summary>
        /// Finds tiles to clear out in order to create space for user specified plazas. Also instantiates said plazas.
        /// </summary>
        /// <param name="mapToModify">The MazeMap being edited.</param>
        /// <param name="plaza">The object containing all plaza data and the plaza prefab.</param>
        /// <param name="rootMazeTransform">The maze that this plaza will be attached to.</param>
        /// <param name="mazeLength">The length of the maze.</param>
        /// <param name="mazeWidth">The width of the maze.</param>
        /// <param name="mazeDimension">Whether the maze is 2D or 3D</param>
        /// <returns></returns>
        private bool SetPlaza(MazeMap mapToModify, PlazaPrefab plaza, Transform rootMazeTransform, int mazeLength, int mazeWidth, MazeDimension mazeDimension)
        {
            if (!plaza.fixedPosition)
            {
                plaza.xPosition = Random.Range(1, mazeWidth - plaza.width);
                plaza.zPosition = Random.Range(1, mazeLength - plaza.length);
            }

            Vector2Int startingTile = new Vector2Int(plaza.xPosition, plaza.zPosition);
            for (int row = 0; row < plaza.length; row++)
            {
                for (int column = 0; column < plaza.width; column++)
                {
                    if (mapToModify.map[((startingTile.y + row) * mapToModify.mazeWidth) + startingTile.x + column] == MazeMap.TileState.plaza)
                    {
                        return true; //returning true is an indicator to keep going
                    }
                }
            }

            for (int row = 0; row < plaza.length; row++)
            {
                for (int column = 0; column < plaza.width; column++)
                {
                    mapToModify.map[((startingTile.y + row) * mapToModify.mazeWidth) + startingTile.x + column] = MazeMap.TileState.plaza;
                }
            }

            GameObject tempPlaza = Instantiate(plaza.gameObject);
            tempPlaza.transform.parent = rootMazeTransform;
            if (mazeDimension == MazeDimension.TwoDimensional)
            {
                tempPlaza.transform.localPosition = new Vector3(plaza.xPosition * mazeTileWidthAndLength, plaza.zPosition * mazeTileWidthAndLength, -plaza.height);
            }
            else
            {
                tempPlaza.transform.localPosition = new Vector3(plaza.xPosition * mazeTileWidthAndLength, plaza.height, plaza.zPosition * mazeTileWidthAndLength);
            }
            plaza.placedOnce = true;

            return false; //returning false is an indicator to stop
        }

        /// <summary>
        /// Sets a valid finish tile somewhere in the maze (based on a user drop-down menu selection)
        /// </summary>
        private void SetStartOrFinish(MazeMap mazeMap, FloorExitType typeOfExit, bool start)
        {
            List<Vector2Int> possibleCoords = new List<Vector2Int>();

            for (int yCoord = 0; yCoord < mazeMap.mazeLength; yCoord++)
            {
                for (int xCoord = 0; xCoord < mazeMap.mazeWidth; xCoord++)
                {
                    possibleCoords.Add(new Vector2Int(xCoord, yCoord));
                }
            }

            for (int index = 0; index < possibleCoords.Count; index++)
            {
                Vector2Int temp = possibleCoords[index];
                int randomIndex = Random.Range(0, possibleCoords.Count);
                possibleCoords[index] = possibleCoords[randomIndex];
                possibleCoords[randomIndex] = temp;
            }

            bool found = false;
            for (int coordIndex = 0; coordIndex < possibleCoords.Count && !found; coordIndex++)
            {
                found = TestUniqueTile(possibleCoords[coordIndex], (UniqueTile.Placement)typeOfExit, mazeMap);
                if (found)
                {
                    mazeMap.map[(possibleCoords[coordIndex].y * mazeMap.mazeWidth) + possibleCoords[coordIndex].x] = start ? MazeMap.TileState.start : MazeMap.TileState.finish; //mark this space as being used for a unique tile
                }
            }
            if (!found)
            {
                Debug.LogWarning("One of the start/end tiles could not be placed anywhere on the map. This could be a result of the current seed, the size of the map, or the tile settings.");
            }
        }

        /// <summary>
        /// Will scatter the given unique tiles randomly throughout the given maze
        /// </summary>
        /// <param name="mazeMap">The maze map to modify</param>
        /// <param name="tilesToPlace">The tiles to place</param>
        private void SetUniqueTiles(MazeMap mazeMap, UniqueTile[] tilesToPlace)
        {
            List<Vector2Int> possibleCoords = new List<Vector2Int>();

            // Get all coordinates in the maze
            for (int yCoord = 0; yCoord < mazeMap.mazeLength; yCoord++)
            {
                for (int xCoord = 0; xCoord < mazeMap.mazeWidth; xCoord++)
                {
                    possibleCoords.Add(new Vector2Int(xCoord, yCoord));
                }
            }

            // Shuffle the coordinates
            for (int index = 0; index < possibleCoords.Count; index++)
            {
                Vector2Int temp = possibleCoords[index];
                int randomIndex = Random.Range(0, possibleCoords.Count);
                possibleCoords[index] = possibleCoords[randomIndex];
                possibleCoords[randomIndex] = temp;
            }

            // Place each unique tile somewhere in the maze
            foreach (UniqueTile tile in tilesToPlace)
            {
                // Iterate through the shuffled coordinates and try to find somewhere to put the unique tile
                bool found = false;
                for (int coordIndex = 0; coordIndex < possibleCoords.Count && !found; coordIndex++)
                {
                    found = TestUniqueTile(possibleCoords[coordIndex], tile.placement, mazeMap);
                    if (found)
                    {
                        //add the tile (with it's coordinates) to the dictionary so we can instantiate it in the right place later
                        uniqueTileDict[possibleCoords[coordIndex]] = tile;
                        //mark this space as being used for a unique tile
                        mazeMap.map[(possibleCoords[coordIndex].y * mazeMap.mazeWidth) + possibleCoords[coordIndex].x] = MazeMap.TileState.unique;
                        //this space is already being used by a unique tile, so we can't use it again
                        possibleCoords.RemoveAt(coordIndex);
                    }
                }

                // There was nowhere available to put the unique tile, so skip it and log a warning
                if (!found)
                {
                    Debug.LogWarning(string.Format("One of the unique tiles [{0}] could not be placed anywhere on the map. This could be a result of the current seed, the size of the map, or the unique tile settings.", tile.uniqueTileName));
                }
            }
        }

        /// <summary>
        /// Tests a location to make sure that it's a valid place to position a unique tile
        /// </summary>
        private bool TestUniqueTile(Vector2Int coord, UniqueTile.Placement placement, MazeMap mapToTest)
        {
            // Can only be placed in limited spaces (don't want to overwrite some of the things that are true under "IsFloor"
            if (mapToTest.map[(coord.y * mapToTest.mazeWidth) + coord.x] == MazeMap.TileState.brokenWall ||
                mapToTest.map[(coord.y * mapToTest.mazeWidth) + coord.x] == MazeMap.TileState.visitedOnce ||
                mapToTest.map[(coord.y * mapToTest.mazeWidth) + coord.x] == MazeMap.TileState.visitedTwice ||
                mapToTest.map[(coord.y * mapToTest.mazeWidth) + coord.x] == MazeMap.TileState.deadEnd ||
                mapToTest.map[(coord.y * mapToTest.mazeWidth) + coord.x] == MazeMap.TileState.unexplored)
            {
                switch(placement)
                {
                    // If this unique tile needs to be placed in the center of the maze, exclude spaces outside of the center
                    case UniqueTile.Placement.center:
                        if (coord.x > mapToTest.mazeWidth / 3 && coord.x < (mapToTest.mazeWidth / 3) * 2)
                        {
                            if (coord.y > mapToTest.mazeLength / 3 && coord.y < (mapToTest.mazeLength / 3) * 2)
                            {
                                return true;
                            }
                        }
                        break;
                    // If this unique tile needs to be placed in the outside of the maze, exclude spaces in the center
                    case UniqueTile.Placement.outside:
                        if (coord.x < mapToTest.mazeWidth / 3 || coord.x > (mapToTest.mazeWidth / 3) * 2)
                        {
                            if (coord.y < mapToTest.mazeLength / 3 || coord.y > (mapToTest.mazeLength / 3) * 2)
                            {
                                return true;
                            }
                        }
                        break;
                    default:
                        return true;
                }

                // Given coordinate failed the positioning test
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Instantiates all basic parts of the 3D maze (floor/walls)
        /// </summary>
        /// <param name="mazeMap">The map to read from for instantiation</param>
        /// <param name="mazeTransform">The parent the pieces will be tied to</param>
        /// <param name="walls">The pool of wall pieces to pull from</param>
        /// <param name="cornerWalls">The pool of wall pieces to pull from for corners</param>
        /// <param name="endWalls">The pool of wall pieces to pull from for ends</param>
        /// <param name="floors">The pool of floor pieces to pull from</param>
        /// <param name="exits">The pool of exits to pull from</param>
        /// <param name="tileWidthAndLength">The width/length of every tile in the maze</param>
        /// <param name="defaultFloorThickness">The default thickness of floor pieces</param>
        /// <param name="instantiationsPerFrame">The number of instantiations to perform every frame. A value of 0 or less will result the entire maze being instantiated in one frame.</param>
        /// <param name="row">The current row of the map we're reading</param>
        /// <param name="column">The current column of the map we're reading</param>
        /// <param name="mazeNodes">An empty 2D array of MazeNode objects - used to persistently keep track of nodes for linkage.</param>
        /// <returns>Returns true if it should be called again, returns false if it has finished</returns>
        private bool InstantiateMaze(MazeMap mazeMap, Transform mazeTransform, WallPiece[] walls, WallPiece[] cornerWalls, WallPiece[] endWalls, FloorPiece[] floors, GameObject[] exits, GameObject[] starts, float tileWidthAndLength, float defaultFloorThickness, bool removePlazaFloors, int instantiationsPerFrame, ref int row, ref int column, MazeNode[] mazeNodes)
        {
            int instantiationCounter = 0;
            bool keepGoing = true;
            for (; row < mazeMap.mazeLength && (instantiationCounter < instantiationsPerFrame || instantiationsPerFrame < 1); row++)
            {
                for (; column < mazeMap.mazeWidth && (instantiationCounter < instantiationsPerFrame || instantiationsPerFrame < 1); column++)
                {
                    int rNGesus;
                    GameObject tempHolder;
                    switch (mazeMap.map[(row * mazeMap.mazeWidth) + column])
                    {
                        case MazeMap.TileState.wall:
                            do
                            {
                                rNGesus = Random.Range(0, walls.Length);
                            } while (!walls[rNGesus].wallPrefab);
                            tempHolder = Instantiate(walls[rNGesus].wallPrefab, new Vector3(column * tileWidthAndLength, walls[rNGesus].wallHeight / 2f, row * tileWidthAndLength) + mazeTransform.position, mazeTransform.rotation);
                            tempHolder.transform.parent = mazeTransform;
                            goto default;

                        case MazeMap.TileState.cornerWall:
                            do
                            {
                                rNGesus = Random.Range(0, cornerWalls.Length);
                            } while (!cornerWalls[rNGesus].wallPrefab);
                            tempHolder = Instantiate(cornerWalls[rNGesus].wallPrefab, new Vector3(column * tileWidthAndLength, cornerWalls[rNGesus].wallHeight / 2f, row * tileWidthAndLength) + mazeTransform.position, mazeTransform.rotation);
                            tempHolder.transform.parent = mazeTransform;
                            goto default;

                        case MazeMap.TileState.endWallUp:
                            do
                            {
                                rNGesus = Random.Range(0, endWalls.Length);
                            } while (!endWalls[rNGesus].wallPrefab);
                            tempHolder = Instantiate(endWalls[rNGesus].wallPrefab, new Vector3(column * tileWidthAndLength, endWalls[rNGesus].wallHeight / 2f, row * tileWidthAndLength) + mazeTransform.position, mazeTransform.rotation);
                            tempHolder.transform.parent = mazeTransform;
                            goto default;

                        case MazeMap.TileState.endWallRight:
                            do
                            {
                                rNGesus = Random.Range(0, endWalls.Length);
                            } while (!endWalls[rNGesus].wallPrefab);
                            tempHolder = Instantiate(endWalls[rNGesus].wallPrefab, new Vector3(column * tileWidthAndLength, endWalls[rNGesus].wallHeight / 2f, row * tileWidthAndLength) + mazeTransform.position, mazeTransform.rotation);
                            tempHolder.transform.Rotate(transform.up, 90f);
                            tempHolder.transform.parent = mazeTransform;
                            goto default;

                        case MazeMap.TileState.endWallDown:
                            do
                            {
                                rNGesus = Random.Range(0, endWalls.Length);
                            } while (!endWalls[rNGesus].wallPrefab);
                            tempHolder = Instantiate(endWalls[rNGesus].wallPrefab, new Vector3(column * tileWidthAndLength, endWalls[rNGesus].wallHeight / 2f, row * tileWidthAndLength) + mazeTransform.position, mazeTransform.rotation);
                            tempHolder.transform.Rotate(transform.up, 180f);
                            tempHolder.transform.parent = mazeTransform;
                            goto default;

                        case MazeMap.TileState.endWallLeft:
                            do
                            {
                                rNGesus = Random.Range(0, endWalls.Length);
                            } while (!endWalls[rNGesus].wallPrefab);
                            tempHolder = Instantiate(endWalls[rNGesus].wallPrefab, new Vector3(column * tileWidthAndLength, endWalls[rNGesus].wallHeight / 2f, row * tileWidthAndLength) + mazeTransform.position, mazeTransform.rotation);
                            tempHolder.transform.Rotate(transform.up, 270f);
                            tempHolder.transform.parent = mazeTransform;
                            goto default;

                        case MazeMap.TileState.finish:
                            rNGesus = Random.Range(0, exits.Length);
                            if (exits[rNGesus])
                            {
                                tempHolder = Instantiate(exits[rNGesus], new Vector3(column * tileWidthAndLength, -(defaultFloorThickness / 2f), row * tileWidthAndLength) + mazeTransform.position, mazeTransform.rotation);
                                tempHolder.transform.parent = mazeTransform;
                                LinkNodes(tempHolder, mazeNodes, mazeMap, row, column);
                            }
                            instantiationCounter++;
                            break;

                        case MazeMap.TileState.start:
                            rNGesus = Random.Range(0, starts.Length);
                            if (starts[rNGesus])
                            {
                                tempHolder = Instantiate(starts[rNGesus], new Vector3(column * tileWidthAndLength, -(defaultFloorThickness / 2f), row * tileWidthAndLength) + mazeTransform.position, mazeTransform.rotation);
                                tempHolder.transform.parent = mazeTransform;
                                LinkNodes(tempHolder, mazeNodes, mazeMap, row, column);
                            }
                            instantiationCounter++;
                            break;

                        case MazeMap.TileState.unique:
                            UniqueTile thisTile = uniqueTileDict[new Vector2Int(column, row)];
                            rNGesus = Random.Range(0, thisTile.tileVariations.Length);
                            if (thisTile.tileVariations[rNGesus])
                            {
                                tempHolder = Instantiate(thisTile.tileVariations[rNGesus], new Vector3(column * tileWidthAndLength, -(defaultFloorThickness / 2f), row * tileWidthAndLength) + mazeTransform.position, mazeTransform.rotation);
                                tempHolder.transform.parent = mazeTransform;
                                tempHolder.gameObject.name = string.IsNullOrEmpty(thisTile.uniqueTileName) ? tempHolder.gameObject.name : thisTile.uniqueTileName;
                                LinkNodes(tempHolder, mazeNodes, mazeMap, row, column);
                            }
                            instantiationCounter++;
                            break;

                        case MazeMap.TileState.plaza:
                            if (removePlazaFloors)
                            {
                                break;
                            }
                            else
                            {
                                goto default;
                            }

                        default:
                            do
                            {
                                rNGesus = Random.Range(0, floors.Length);
                            } while (!floors[rNGesus].floorPrefab);
                            tempHolder = Instantiate(floors[rNGesus].floorPrefab, new Vector3(column * tileWidthAndLength, -(floors[rNGesus].floorThickness / 2f), row * tileWidthAndLength) + mazeTransform.position, mazeTransform.rotation);
                            tempHolder.transform.parent = mazeTransform;
                            LinkNodes(tempHolder, mazeNodes, mazeMap, row, column);
                            instantiationCounter++;
                            break;
                    }
                }
                if (instantiationCounter >= instantiationsPerFrame && instantiationsPerFrame > 0)
                {
                    column--;
                }
                else if (column >= mazeMap.mazeWidth)
                {
                    column = 0;
                }
            }
            if (instantiationCounter >= instantiationsPerFrame && instantiationsPerFrame > 0)
            {
                row--;
            }
            else if (row >= mazeMap.mazeLength)
            {
                keepGoing = false;
            }
            return keepGoing;
        }

        /// <summary>
        /// Instantiates all basic parts of the @D maze (floor/walls)
        /// </summary>
        /// <param name="mazeMap">The map to read from for instantiation</param>
        /// <param name="mazeTransform">The parent the pieces will be tied to</param>
        /// <param name="walls">The pool of wall pieces to pull from</param>
        /// <param name="cornerWalls">The pool of wall pieces to pull from for corners</param>
        /// <param name="endWalls">The pool of wall pieces to pull from for ends</param>
        /// <param name="floors">The pool of floor pieces to pull from</param>
        /// <param name="exits">The pool of exits to pull from</param>
        /// <param name="tileWidthAndLength">The width/length of every tile in the maze</param>
        /// <param name="floorZPlane">The plane that all floor tiles will occupy</param>
        /// <param name="instantiationsPerFrame">The number of instantiations to perform every frame. A value of 0 or less will result the entire maze being instantiated in one frame.</param>
        /// <param name="row">The current row of the map we're reading</param>
        /// <param name="column">The current column of the map we're reading</param>
        /// <param name="mazeNodes">An empty 2D array of MazeNode objects - used to persistently keep track of nodes for linkage.</param>
        /// <returns>Returns true if it should be called again, returns false if it has finished</returns>
        private bool InstantiateMaze2D(MazeMap mazeMap, Transform mazeTransform, WallPiece2D[] walls, WallPiece2D[] cornerWalls, WallPiece2D[] endWalls, GameObject[] floors, GameObject[] exits, GameObject[] starts, float tileWidthAndLength, float floorZPlane, bool removePlazaFloors, int instantiationsPerFrame, ref int row, ref int column, MazeNode[] mazeNodes)
        {
            int instantiationCounter = 0;
            bool keepGoing = true;
            for (; row < mazeMap.mazeLength && (instantiationCounter < instantiationsPerFrame || instantiationsPerFrame < 1); row++)
            {
                for (; column < mazeMap.mazeWidth && (instantiationCounter < instantiationsPerFrame || instantiationsPerFrame < 1); column++)
                {
                    int rNGesus;
                    GameObject tempHolder;
                    switch (mazeMap.map[(row * mazeMap.mazeWidth) + column])
                    {
                        case MazeMap.TileState.wall:
                            do
                            {
                                rNGesus = Random.Range(0, walls.Length);
                            } while (!walls[rNGesus].wallPrefab);
                            tempHolder = Instantiate(walls[rNGesus].wallPrefab, new Vector3(column * tileWidthAndLength, row * tileWidthAndLength, walls[rNGesus].wallZPlane) + mazeTransform.position, mazeTransform.rotation);
                            tempHolder.transform.parent = mazeTransform;
                            goto default;

                        case MazeMap.TileState.cornerWall:
                            do
                            {
                                rNGesus = Random.Range(0, cornerWalls.Length);
                            } while (!cornerWalls[rNGesus].wallPrefab);
                            tempHolder = Instantiate(cornerWalls[rNGesus].wallPrefab, new Vector3(column * tileWidthAndLength, row * tileWidthAndLength, cornerWalls[rNGesus].wallZPlane) + mazeTransform.position, mazeTransform.rotation);
                            tempHolder.transform.parent = mazeTransform;
                            goto default;

                        case MazeMap.TileState.endWallUp:
                            do
                            {
                                rNGesus = Random.Range(0, endWalls.Length);
                            } while (!endWalls[rNGesus].wallPrefab);
                            tempHolder = Instantiate(endWalls[rNGesus].wallPrefab, new Vector3(column * tileWidthAndLength, row * tileWidthAndLength, endWalls[rNGesus].wallZPlane) + mazeTransform.position, mazeTransform.rotation);
                            tempHolder.transform.parent = mazeTransform;
                            goto default;

                        case MazeMap.TileState.endWallRight:
                            do
                            {
                                rNGesus = Random.Range(0, endWalls.Length);
                            } while (!endWalls[rNGesus].wallPrefab);
                            tempHolder = Instantiate(endWalls[rNGesus].wallPrefab, new Vector3(column * tileWidthAndLength, row * tileWidthAndLength, endWalls[rNGesus].wallZPlane) + mazeTransform.position, mazeTransform.rotation);
                            tempHolder.transform.Rotate(transform.forward, 90f);
                            tempHolder.transform.parent = mazeTransform;
                            goto default;

                        case MazeMap.TileState.endWallDown:
                            do
                            {
                                rNGesus = Random.Range(0, endWalls.Length);
                            } while (!endWalls[rNGesus].wallPrefab);
                            tempHolder = Instantiate(endWalls[rNGesus].wallPrefab, new Vector3(column * tileWidthAndLength, row * tileWidthAndLength, endWalls[rNGesus].wallZPlane) + mazeTransform.position, mazeTransform.rotation);
                            tempHolder.transform.Rotate(transform.forward, 180f);
                            tempHolder.transform.parent = mazeTransform;
                            goto default;

                        case MazeMap.TileState.endWallLeft:
                            do
                            {
                                rNGesus = Random.Range(0, endWalls.Length);
                            } while (!endWalls[rNGesus].wallPrefab);
                            tempHolder = Instantiate(endWalls[rNGesus].wallPrefab, new Vector3(column * tileWidthAndLength, row * tileWidthAndLength, endWalls[rNGesus].wallZPlane) + mazeTransform.position, mazeTransform.rotation);
                            tempHolder.transform.Rotate(transform.forward, 270f);
                            tempHolder.transform.parent = mazeTransform;
                            goto default;

                        case MazeMap.TileState.finish:
                            rNGesus = Random.Range(0, exits.Length);
                            if (exits[rNGesus])
                            {
                                tempHolder = Instantiate(exits[rNGesus], new Vector3(column * tileWidthAndLength, row * tileWidthAndLength, floorZPlane) + mazeTransform.position, mazeTransform.rotation);
                                tempHolder.transform.parent = mazeTransform;
                                LinkNodes(tempHolder, mazeNodes, mazeMap, row, column);
                            }
                            instantiationCounter++;
                            break;

                        case MazeMap.TileState.start:
                            rNGesus = Random.Range(0, starts.Length);
                            if (starts[rNGesus])
                            {
                                tempHolder = Instantiate(starts[rNGesus], new Vector3(column * tileWidthAndLength, row * tileWidthAndLength, floorZPlane) + mazeTransform.position, mazeTransform.rotation);
                                tempHolder.transform.parent = mazeTransform;
                                LinkNodes(tempHolder, mazeNodes, mazeMap, row, column);
                            }
                            instantiationCounter++;
                            break;

                        case MazeMap.TileState.unique:
                            UniqueTile thisTile = uniqueTileDict[new Vector2Int(column, row)];
                            rNGesus = Random.Range(0, thisTile.tileVariations.Length);
                            if (thisTile.tileVariations[rNGesus])
                            {
                                tempHolder = Instantiate(thisTile.tileVariations[rNGesus], new Vector3(column * tileWidthAndLength, row * tileWidthAndLength, floorZPlane) + mazeTransform.position, mazeTransform.rotation);
                                tempHolder.transform.parent = mazeTransform;
                                tempHolder.gameObject.name = string.IsNullOrEmpty(thisTile.uniqueTileName) ? tempHolder.gameObject.name : thisTile.uniqueTileName;
                                LinkNodes(tempHolder, mazeNodes, mazeMap, row, column);
                            }
                            instantiationCounter++;
                            break;

                        case MazeMap.TileState.plaza:
                            if (removePlazaFloors)
                            {
                                break;
                            }
                            else
                            {
                                goto default;
                            }

                        default:
                            do
                            {
                                rNGesus = Random.Range(0, floors.Length);
                            } while (!floors[rNGesus]);
                            tempHolder = Instantiate(floors[rNGesus], new Vector3(column * tileWidthAndLength, row * tileWidthAndLength, floorZPlane) + mazeTransform.position, mazeTransform.rotation);
                            tempHolder.transform.parent = mazeTransform;
                            LinkNodes(tempHolder, mazeNodes, mazeMap, row, column);
                            instantiationCounter++;
                            break;
                    }
                }
                if (instantiationCounter >= instantiationsPerFrame && instantiationsPerFrame > 0)
                {
                    column--;
                }
                else if (column >= mazeMap.mazeWidth)
                {
                    column = 0;
                }
            }
            if (instantiationCounter >= instantiationsPerFrame && instantiationsPerFrame > 0)
            {
                row--;
            }
            else if (row >= mazeMap.mazeLength)
            {
                keepGoing = false;
            }
            return keepGoing;
        }

        /// <summary>
        /// Adds random details to the walls throughout the maze
        /// </summary>
        /// <param name="mapToModify">The map that the details will be added to</param>
        /// <param name="tileWidthAndLength">The length and width of each tile</param>
        /// <param name="mazeTransform">The root transform of the maze these details are being added to</param>
        /// <param name="instantiationsPerFrame"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool PlaceRandomWallDetails(MazeMap mapToModify, WallDetail[] objectsForWalls, float tileWidthAndLength, Transform mazeTransform, int instantiationsPerFrame, ref int detailIndex, ref int row, ref int column)
        {
            int instantiationCounter = 0;
            bool keepGoing = true;
            for (; detailIndex < wallDetails.Length && (instantiationCounter < instantiationsPerFrame || instantiationsPerFrame < 1); detailIndex++)
            {
                MazeMap.TileState testResults = MazeMap.TileState.outOfBounds;
                Vector2Int move = Vector2Int.zero;
                //search through the maze until you find a wall since these details will only be placed on walls
                for (; row < mapToModify.mazeLength && (instantiationCounter < instantiationsPerFrame || instantiationsPerFrame < 1); row++)
                {
                    for (; column < mapToModify.mazeWidth && (instantiationCounter < instantiationsPerFrame || instantiationsPerFrame < 1); column++)
                    {
                        if (mapToModify.map[(row * mapToModify.mazeWidth) + column] == MazeMap.TileState.wall)
                        {
                            //RNGesus determines if the object will be placed on this wall or not depending on your frequency value
                            if (Random.Range(0f, 1f) <= wallDetails[detailIndex].frequency)
                            {
                                Vector2Int currentTileCoordinates = new Vector2Int(column, row);
                                TestDirection testDirection = (TestDirection)1;
                                Quaternion detailRotation = mazeTransform.rotation;
                                Vector3 detailZDirection = Vector3.zero;
                                Vector3 detailXDirection = Vector3.zero;

                                //test every direction from the wall so that you only place the detail on faces of the wall piece that can be seen
                                //if you've found a valid placement, the rotation (z-out relative to the normal of the wall face) is stored, as well
                                //as the randomly selected vertical/horizontal position
                                while ((int)testDirection <= 4)
                                {
                                    switch (testDirection)
                                    {
                                        case TestDirection.left: //one is left
                                            move = Vector2Int.left;
                                            detailRotation = Quaternion.LookRotation(-mazeTransform.right, mazeTransform.up);
                                            detailZDirection = -mazeTransform.right;
                                            detailXDirection = -mazeTransform.forward;
                                            break;

                                        case TestDirection.up: //two is up
                                            move = Vector2Int.up;
                                            detailRotation = Quaternion.LookRotation(mazeTransform.forward, mazeTransform.up);
                                            detailZDirection = mazeTransform.forward;
                                            detailXDirection = -mazeTransform.right;
                                            break;

                                        case TestDirection.right: //three is right
                                            move = Vector2Int.right;
                                            detailRotation = Quaternion.LookRotation(mazeTransform.right, mazeTransform.up);
                                            detailZDirection = mazeTransform.right;
                                            detailXDirection = mazeTransform.forward;
                                            break;

                                        case TestDirection.down: //four is down
                                            move = Vector2Int.down;
                                            detailRotation = Quaternion.LookRotation(-mazeTransform.forward, mazeTransform.up);
                                            detailZDirection = -mazeTransform.forward;
                                            detailXDirection = mazeTransform.right;
                                            break;
                                    }
                                    testResults = CheckTile(move, mapToModify, currentTileCoordinates);
                                    if (!IsWall(testResults) && testResults != MazeMap.TileState.outOfBounds)
                                    {
                                        GameObject tempDetail = Instantiate(wallDetails[detailIndex].detailPrefab, new Vector3(column * tileWidthAndLength, Random.Range(wallDetails[detailIndex].minHeight, wallDetails[detailIndex].maxHeight), row * mazeTileWidthAndLength) + mazeTransform.position + ((mazeTileWidthAndLength / 2f) + wallDetails[detailIndex].thickness / 2f) * detailZDirection + Random.Range(wallDetails[detailIndex].minHorizontalOffset, wallDetails[detailIndex].maxHorizontalOffset) * detailXDirection, detailRotation);
                                        tempDetail.transform.parent = mazeTransform;
                                        tempDetail.name = "Detail(" + wallDetails[detailIndex].detailName + ")";
                                    }
                                    testDirection++;
                                }
                            }
                        }
                        instantiationCounter++;
                    }
                    if (instantiationCounter >= instantiationsPerFrame && instantiationsPerFrame > 0)
                    {
                        column--;
                    }
                    else if (column >= mapToModify.mazeWidth)
                    {
                        column = 0;
                    }
                }
                if (instantiationCounter >= instantiationsPerFrame && instantiationsPerFrame > 0)
                {
                    row--;
                }
                else if (row >= mapToModify.mazeLength)
                {
                    row = 0;
                }
            }
            if (instantiationCounter >= instantiationsPerFrame && instantiationsPerFrame > 0)
            {
                detailIndex--;
            }
            else if (detailIndex >= wallDetails.Length)
            {
                keepGoing = false;
            }
            return keepGoing;
        }

        /// <summary>
        /// Places random details throughout the maze
        /// </summary>
        /// <param name="mapToModify">The maze map that will store information about where the details are placed</param>
        /// <param name="otherObjects">The array of details that will be pulled from to place the objects</param>
        /// <param name="tileWidthAndLength">The length and width of the tiles (used to calculate positioning)</param>
        /// <param name="mazeTransform">The transform that these details will need to be parented to</param>
        /// <param name="instantiationsPerFrame">The number of these objects that will be instantiated every frame until this is done</param>
        /// <param name="detailIndex">Used to keep track of what's been instantiated</param>
        /// <param name="row">Used to keep track of where things have been instantiated</param>
        /// <param name="column">Used to keep track of where things have been instantiated</param>
        /// <returns>True if it isn't finished and false if it is</returns>
        private bool PlaceRandomOtherDetails(MazeMap mapToModify, OtherDetail[] otherObjects, float tileWidthAndLength, Transform mazeTransform, int instantiationsPerFrame, ref int detailIndex, ref int row, ref int column)
        {
            int instantiationCounter = 0;
            bool keepGoing = true;
            for (; detailIndex < otherObjects.Length && (instantiationCounter < instantiationsPerFrame || instantiationsPerFrame < 1); detailIndex++)
            {
                //search through the maze until you find a floor since details will be placed on tiles that aren't wall pieces or finish squares
                for (; row < mapToModify.mazeLength && (instantiationCounter < instantiationsPerFrame || instantiationsPerFrame < 1); row++)
                {
                    for (; column < mapToModify.mazeWidth && (instantiationCounter < instantiationsPerFrame || instantiationsPerFrame < 1); column++)
                    {
                        Vector2Int currentTileCoordinates = new Vector2Int(column, row); //sets the current tile to our current coordinates
                        MazeMap.TileState testResults = CheckTile(Vector2Int.zero, mapToModify, currentTileCoordinates); //we're just checking the tile we're on
                        if (!IsWall(testResults) && testResults != MazeMap.TileState.finish && testResults != MazeMap.TileState.start && testResults != MazeMap.TileState.plaza) //if the tile isn't a wall, the finish square, or a plaza we can procede
                        {
                            if (Random.Range(0f, 1f) <= otherObjects[detailIndex].frequency)//RNGesus determines if the object will be placed on this tile or not depending on your frequency value
                            {
                                GameObject tempDetail = Instantiate(otherObjects[detailIndex].detailPrefab, new Vector3(column * tileWidthAndLength + Random.Range(otherObjects[detailIndex].minXOffset, otherObjects[detailIndex].maxXOffset), Random.Range(otherObjects[detailIndex].minHeight, otherObjects[detailIndex].maxHeight), row * mazeTileWidthAndLength + Random.Range(otherObjects[detailIndex].minZOffset, otherObjects[detailIndex].maxZOffset)) + mazeTransform.position, mazeTransform.rotation);
                                tempDetail.transform.Rotate(transform.up, Random.Range(0f, 360f));
                                tempDetail.transform.parent = mazeTransform;
                                tempDetail.name = "Detail(" + otherObjects[detailIndex].detailName + ")";
                            }
                        }
                        instantiationCounter++;
                    }
                    if (instantiationCounter >= instantiationsPerFrame && instantiationsPerFrame > 0)
                    {
                        column--;
                    }
                    else if (column >= mapToModify.mazeWidth)
                    {
                        column = 0;
                    }
                }
                if (instantiationCounter >= instantiationsPerFrame && instantiationsPerFrame > 0)
                {
                    row--;
                }
                else if (row >= mapToModify.mazeLength)
                {
                    row = 0;
                }
            }
            if (instantiationCounter >= instantiationsPerFrame && instantiationsPerFrame > 0)
            {
                detailIndex--;
            }
            else if (detailIndex >= otherObjects.Length)
            {
                keepGoing = false;
            }
            return keepGoing;
        }

        /// <summary>
        /// Places random two dimensional details throughout the maze
        /// </summary>
        /// <param name="mapToModify">The maze map that will store information about where the details are placed</param>
        /// <param name="twoDimensionalObjects">The array of details that will be pulled from to place the objects</param>
        /// <param name="tileWidthAndLength">The length and width of the tiles (used to calculate positioning)</param>
        /// <param name="mazeTransform">The transform that these details will need to be parented to</param>
        /// <param name="instantiationsPerFrame">The number of these objects that will be instantiated every frame until this is done</param>
        /// <param name="detailIndex">Used to keep track of what's been instantiated</param>
        /// <param name="row">Used to keep track of where things have been instantiated</param>
        /// <param name="column">Used to keep track of where things have been instantiated</param>
        /// <returns>True if it isn't finished and false if it is</returns>
        private bool PlaceRandomTwoDimensionalDetails(MazeMap mapToModify, TwoDimensionalDetail[] twoDimensionalObjects, float tileWidthAndLength, Transform mazeTransform, int instantiationsPerFrame, ref int detailIndex, ref int row, ref int column)
        {
            int instantiationCounter = 0;
            bool keepGoing = true;
            for (; detailIndex < twoDimensionalObjects.Length; detailIndex++)
            {
                //search through the maze until you find a floor since details will be placed on tiles that aren't wall pieces or finish squares
                for (; row < mapToModify.mazeLength && (instantiationCounter < instantiationsPerFrame || instantiationsPerFrame < 1); row++)
                {
                    for (; column < mapToModify.mazeWidth && (instantiationCounter < instantiationsPerFrame || instantiationsPerFrame < 1); column++)
                    {
                        //sets the current tile to our current coordinates
                        Vector2Int currentTileCoordinates = new Vector2Int(column, row);
                        //we're just checking the tile we're on
                        MazeMap.TileState testResults = CheckTile(Vector2Int.zero, mapToModify, currentTileCoordinates);
                        //if the tile isn't a wall, the finish square, or a plaza, we can procede
                        if (!IsWall(testResults) && testResults != MazeMap.TileState.finish && testResults != MazeMap.TileState.start && testResults != MazeMap.TileState.plaza)
                        {
                            //RNGesus determines if the object will be placed on this tile or not depending on your frequency value
                            if (Random.Range(0f, 1f) <= twoDimensionalObjects[detailIndex].frequency)
                            {
                                GameObject tempDetail = (GameObject)Instantiate(twoDimensionalObjects[detailIndex].detailPrefab, new Vector3(column * tileWidthAndLength + Random.Range(twoDimensionalObjects[detailIndex].minXOffset, twoDimensionalObjects[detailIndex].maxXOffset), row * mazeTileWidthAndLength + Random.Range(twoDimensionalObjects[detailIndex].minYOffset, twoDimensionalObjects[detailIndex].maxYOffset), twoDimensionalObjects[detailIndex].zPlane) + mazeTransform.position, mazeTransform.rotation);
                                tempDetail.transform.Rotate(transform.up, Random.Range(0f, 360f));
                                tempDetail.transform.parent = mazeTransform;
                                tempDetail.name = "Detail(" + twoDimensionalObjects[detailIndex].detailName + ")";
                            }
                        }
                        instantiationCounter++;
                    }
                    if (instantiationCounter >= instantiationsPerFrame && instantiationsPerFrame > 0)
                    {
                        column--;
                    }
                    else if (column >= mapToModify.mazeWidth)
                    {
                        column = 0;
                    }
                }
                if (instantiationCounter >= instantiationsPerFrame && instantiationsPerFrame > 0)
                {
                    row--;
                }
                else if (row >= mapToModify.mazeLength)
                {
                    row = 0;
                }
            }
            if (instantiationCounter >= instantiationsPerFrame && instantiationsPerFrame > 0)
            {
                detailIndex--;
            }
            else if (detailIndex >= twoDimensionalObjects.Length)
            {
                keepGoing = false;
            }
            return keepGoing;
        }

        /// <summary>
        /// Used to link all nodes together during maze instantiation
        /// </summary>
        /// <param name="nodeHost">The object that will hold the node component (generally a floor piece since there is a floor piece at every tile)</param>
        /// <param name="mazeNodes">The 2D array that persistently holds information about all nodes being linked</param>
        /// <param name="mazeMap">The map of the maze being generated</param>
        /// <param name="currentRow">The current y coordinate in the maze</param>
        /// <param name="currentColumn">The current x coordinate in the maze</param>
        private void LinkNodes(GameObject nodeHost, MazeNode[] mazeNodes, MazeMap mazeMap, int currentRow, int currentColumn)
        {
            MazeNode node;
            node = nodeHost.AddComponent<MazeNode>(); //adds the MazeNode component to our host
            node.tileType = mazeMap.map[(currentRow * mazeMap.mazeWidth) + currentColumn]; //stores the Tile information (will make pathfinding easier)
            node.coordinates = new Vector2Int(currentColumn, currentRow); //stores the coordinates of this tile (might be useful for pathfinding or other applications)
            mazeNodes[(currentRow * mazeMap.mazeWidth) + currentColumn] = node; //persistently stores information about this new node so that it can be linked to
            //creates all possible links to neighbours
            if (currentColumn > 0)
            {
                if (mazeNodes[(currentRow * mazeMap.mazeWidth) + currentColumn - 1])
                {
                    mazeNodes[(currentRow * mazeMap.mazeWidth) + currentColumn - 1].AddConnectionRight(node);
                }
            }
            if (currentRow > 0)
            {
                if (mazeNodes[((currentRow - 1) * mazeMap.mazeWidth) + currentColumn])
                {
                    mazeNodes[((currentRow - 1) * mazeMap.mazeWidth) + currentColumn].AddConnectionUp(node);
                }
            }
        }

        /// <summary>
        /// Checks for existing mazes with the proposed name and deletes them to facilitate "overwriting". Used when the application is playing.
        /// </summary>
        /// <param name="nameToCheck">The name to be checked against</param>
        private void CheckDuplicateMazeNames(string nameToCheck)
        {
            if (Application.isPlaying)
            {
                Maze[] mazes = FindObjectsOfType<Maze>();
                foreach (Maze maze in mazes)
                {
                    if (maze.gameObject.name.Equals(nameToCheck))
                    {
                        Destroy(maze.gameObject);
                    }
                }
            }
            else
            {
                Debug.LogError("CheckDuplicateMazeNames(string nameToCheck) was called from the editor - this is not allowed as seperate editor logic handles this when the game isn't running.");
            }
        }

        /// <summary>
        /// Determines if there should not be a wall on the tile that has the provided state
        /// </summary>
        /// <param name="state">The state of the tile</param>
        /// <returns>True if there shouldn't be a wall on this tile, false otherwise</returns>
        public static bool IsFloor(MazeMap.TileState state)
        {
            return (state == MazeMap.TileState.brokenWall || state == MazeMap.TileState.deadEnd || state == MazeMap.TileState.finish || 
                    state == MazeMap.TileState.plaza || state == MazeMap.TileState.unexplored || state == MazeMap.TileState.visitedOnce || 
                    state == MazeMap.TileState.visitedTwice || state == MazeMap.TileState.start || state == MazeMap.TileState.unique);
        }

        /// <summary>
        /// Determines if there should be a wall on a tile that has the provided state
        /// </summary>
        /// <param name="state">The state of the tile</param>
        /// <returns>True if their should be a wall on this tile, false otherwise</returns>
        public static bool IsWall(MazeMap.TileState state)
        {
            return (state == MazeMap.TileState.cornerWall || state == MazeMap.TileState.endWallUp || state == MazeMap.TileState.endWallRight ||
                    state == MazeMap.TileState.endWallDown || state == MazeMap.TileState.endWallLeft || state == MazeMap.TileState.wall);
        }
    }
}

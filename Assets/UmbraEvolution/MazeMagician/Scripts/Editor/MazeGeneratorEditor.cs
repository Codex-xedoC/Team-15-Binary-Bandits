//Name: Robert MacGillivray
//Date: Jun.10.2015
//File: MazeGeneratorEditor.cs
//Purpose: To modify the inspector panel of the MazeGenerator

//Last Updated: Jul.01.2022 by Robert MacGillivray

using UnityEngine;
using UnityEditor;

namespace UmbraEvolution.UmbraMazeMagician
{
    /// <summary>
    /// Makes the maze generator pretty
    /// </summary>
    [CustomEditor(typeof(MazeGenerator))]
    public class MazeGeneratorEditor : Editor
    {
        /// <summary>
        /// Adds a menu item in the toolbar that adds a MazeGenerator to the open scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("Tools/UmbraEvolution/Umbra Maze Magician/Create Maze Generator")]
        static void CreateMazeGenerator()
        {
            GameObject mazeGenerator = new GameObject("UmbraMazeMagician", typeof(MazeGenerator));
            Undo.RegisterCreatedObjectUndo(mazeGenerator, "Created Maze Generator");
            Selection.activeObject = mazeGenerator;
        }

        public override void OnInspectorGUI()
        {
            bool hasAllPlazas = true;
            bool hasAllWalls = true;
            bool hasAllFloors = true;
            bool hasAllWallDetails = true;
            bool hasAllOtherDetails = true;
            bool hasAllTwoDimensionalDetails = true;

            serializedObject.Update();

            MazeGenerator mazeGenerator = (MazeGenerator)target;

            EditorGUILayout.Space();

            #region General Settings Stuff
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mazeDimension"));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("showDimensionsGizmo"));
            if (mazeGenerator.showDimensionsGizmo)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("dimensionsGizmoColour"));
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");
            GUIContent useSeedValueContent = new GUIContent("Custom Seed Value?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useSeedValue"), useSeedValueContent);
            if (mazeGenerator.useSeedValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("seedValue"));
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("seedValue"));
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mazeWidth"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mazeLength"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mazePosition"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mazeTileWidthAndLength"));

            if (mazeGenerator.mazeDimension == MazeGenerator.MazeDimension.TwoDimensional)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultWallZPlane"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("floorZPlane"));
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultWallHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultFloorThickness"));
            }
            EditorGUILayout.EndVertical();

            if (mazeGenerator.mazeDimension == MazeGenerator.MazeDimension.TwoDimensional)
            {
                EditorGUILayout.HelpBox("Note that every wall piece, floor piece, and detail prefab should have the pivot point in its center with the axes oriented the same as the world axes", MessageType.Info, true);

            }
            else
            {
                EditorGUILayout.HelpBox("Note that every wall piece, wall detail, floor piece, and other detail should have the pivot point oriented as follows: The pivot point should be centered, the y-axis should be pointing upwards, and both the z/x axes should be perpendicular to two surfaces and parallel to two others (basically the pivot should be oriented the same way as Unity's primitive objects). Also note that in the case of wall details, the object will be oriented so that its z-axis points out from the wall when it is placed.", MessageType.Info, true);
            }
            #endregion

            #region Wall Piece Stuff
            EditorGUILayout.BeginVertical("Box");
            SerializedProperty wallPiecesProperty;
            //Normal Wall Pieces
            if (mazeGenerator.mazeDimension == MazeGenerator.MazeDimension.TwoDimensional)
            {
                wallPiecesProperty = serializedObject.FindProperty("wallPieces2D");
            }
            else
            {
                wallPiecesProperty = serializedObject.FindProperty("wallPieces");
            }
            GUIContent wallPiecesContent = new GUIContent(wallPiecesProperty.displayName, wallPiecesProperty.tooltip);
            SerializedProperty numberOfWallPiecesProperty = serializedObject.FindProperty("numberOfWallPieces");
            SerializedProperty showWallPieces = serializedObject.FindProperty("showWallPieces");
            if (mazeGenerator.mazeDimension == MazeGenerator.MazeDimension.TwoDimensional)
            {
                DisplayWallsFormatted2D(wallPiecesContent, showWallPieces, wallPiecesProperty, numberOfWallPiecesProperty);
                foreach (WallPiece2D wallPiece in mazeGenerator.wallPieces2D)
                {
                    if (!wallPiece.wallPrefab)
                    {
                        hasAllWalls = false;
                        break;
                    }
                }
            }
            else
            {
                DisplayWallsFormatted(wallPiecesContent, showWallPieces, wallPiecesProperty, numberOfWallPiecesProperty);
                foreach (WallPiece wallPiece in mazeGenerator.wallPieces)
                {
                    if (!wallPiece.wallPrefab)
                    {
                        hasAllWalls = false;
                        break;
                    }
                }
            }

            //Corner Wall Pieces
            SerializedProperty differentCornersProperty = serializedObject.FindProperty("differentCorners");
            EditorGUILayout.PropertyField(differentCornersProperty);
            if (differentCornersProperty.boolValue)
            {
                if (mazeGenerator.mazeDimension == MazeGenerator.MazeDimension.TwoDimensional)
                {
                    wallPiecesProperty = serializedObject.FindProperty("cornerWallPieces2D");
                }
                else
                {
                    wallPiecesProperty = serializedObject.FindProperty("cornerWallPieces");
                }
                wallPiecesContent = new GUIContent(wallPiecesProperty.displayName, wallPiecesProperty.tooltip);
                numberOfWallPiecesProperty = serializedObject.FindProperty("numberOfCornerWallPieces");
                showWallPieces = serializedObject.FindProperty("showCornerWallPieces");
                if (mazeGenerator.mazeDimension == MazeGenerator.MazeDimension.TwoDimensional)
                {
                    DisplayWallsFormatted2D(wallPiecesContent, showWallPieces, wallPiecesProperty, numberOfWallPiecesProperty);
                    foreach (WallPiece2D wallPiece in mazeGenerator.cornerWallPieces2D)
                    {
                        if (!wallPiece.wallPrefab)
                        {
                            hasAllWalls = false;
                            break;
                        }
                    }
                }
                else
                {
                    DisplayWallsFormatted(wallPiecesContent, showWallPieces, wallPiecesProperty, numberOfWallPiecesProperty);
                    foreach (WallPiece wallPiece in mazeGenerator.cornerWallPieces)
                    {
                        if (!wallPiece.wallPrefab)
                        {
                            hasAllWalls = false;
                            break;
                        }
                    }
                }
            }

            //End Wall Pieces
            SerializedProperty differentEndsProperty = serializedObject.FindProperty("differentEnds");
            EditorGUILayout.PropertyField(differentEndsProperty);
            if (differentEndsProperty.boolValue)
            {
                if (mazeGenerator.mazeDimension == MazeGenerator.MazeDimension.TwoDimensional)
                {
                    wallPiecesProperty = serializedObject.FindProperty("endWallPieces2D");
                }
                else
                {
                    wallPiecesProperty = serializedObject.FindProperty("endWallPieces");
                }
                wallPiecesContent = new GUIContent(wallPiecesProperty.displayName, wallPiecesProperty.tooltip);
                numberOfWallPiecesProperty = serializedObject.FindProperty("numberOfEndWallPieces");
                showWallPieces = serializedObject.FindProperty("showEndWallPieces");
                if (mazeGenerator.mazeDimension == MazeGenerator.MazeDimension.TwoDimensional)
                {
                    DisplayWallsFormatted2D(wallPiecesContent, showWallPieces, wallPiecesProperty, numberOfWallPiecesProperty);
                    foreach (WallPiece2D wallPiece in mazeGenerator.endWallPieces2D)
                    {
                        if (!wallPiece.wallPrefab)
                        {
                            hasAllWalls = false;
                            break;
                        }
                    }
                }
                else
                {
                    DisplayWallsFormatted(wallPiecesContent, showWallPieces, wallPiecesProperty, numberOfWallPiecesProperty);
                    foreach (WallPiece wallPiece in mazeGenerator.endWallPieces)
                    {
                        if (!wallPiece.wallPrefab)
                        {
                            hasAllWalls = false;
                            break;
                        }
                    }
                }
            }
            
            if (!hasAllWalls)
            {
                EditorGUILayout.HelpBox("You need to have all wall pieces filled in to generate a maze!", MessageType.Warning, true);
            }
            EditorGUILayout.EndVertical();
            #endregion

            #region Floor Piece Stuff
            SerializedProperty floorPiecesProperty;
            if (mazeGenerator.mazeDimension == MazeGenerator.MazeDimension.TwoDimensional)
            {
                floorPiecesProperty = serializedObject.FindProperty("floorPieces2D");
            }
            else
            {
                floorPiecesProperty = serializedObject.FindProperty("floorPieces");
            }
            GUIContent floorPiecesContent = new GUIContent(floorPiecesProperty.displayName, floorPiecesProperty.tooltip);
            SerializedProperty numberOfFloorPiecesProperty = serializedObject.FindProperty("numberOfFloorPieces");
            SerializedProperty showFloorPieces = serializedObject.FindProperty("showFloorPieces");
            if (mazeGenerator.mazeDimension == MazeGenerator.MazeDimension.TwoDimensional)
            {
                DisplayGameObjectArrayFormatted(floorPiecesContent, showFloorPieces, floorPiecesProperty, numberOfFloorPiecesProperty);
                foreach (GameObject floorPiece in mazeGenerator.floorPieces2D)
                {
                    if (!floorPiece)
                    {
                        hasAllFloors = false;
                        break;
                    }
                }
            }
            else
            {
                DisplayFloorsFormatted(floorPiecesContent, showFloorPieces, floorPiecesProperty, numberOfFloorPiecesProperty);
                foreach (FloorPiece floorPiece in mazeGenerator.floorPieces)
                {
                    if (!floorPiece.floorPrefab)
                    {
                        hasAllFloors = false;
                        break;
                    }
                }
            }
            if (!hasAllFloors)
            {
                EditorGUILayout.HelpBox("You need to have all floor pieces filled in to generate or load a maze!", MessageType.Warning, true);
            }
            #endregion

            #region Plaza Stuff
            EditorGUILayout.BeginVertical("Box");
            GUIContent makePlazasContent = new GUIContent("Make Plazas?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("makePlazas"), makePlazasContent);

            if (mazeGenerator.makePlazas)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("numberOfPlazas"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("plazaPlacementAttempts"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("removePlazaFloors"));
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("plazas"), true);
                EditorGUI.indentLevel -= 2;

                foreach (PlazaPrefab plaza in mazeGenerator.plazas)
                {
                    if (!plaza)
                    {
                        hasAllPlazas = false;
                    }
                }
                if (!hasAllPlazas)
                {
                    EditorGUILayout.HelpBox("You need to have all plazas filled in to generate a maze!", MessageType.Warning, true);
                }
            }
            EditorGUILayout.EndVertical();
            #endregion

            #region Detail Stuff
            EditorGUILayout.BeginVertical("Box");
            GUIContent addDetailsContent = new GUIContent("Add Details?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("addDetails"), addDetailsContent);
            if (mazeGenerator.addDetails)
            {
                EditorGUILayout.HelpBox("It is important that the proper values are entered for each detail you want scattered throughout the maze. If the following are not filled out correctly, you will end up with floating, clipping, or misaligned objects. Read the tooltips for more detailed information.", MessageType.Info, true);
                EditorGUI.indentLevel++;

                if (mazeGenerator.mazeDimension == MazeGenerator.MazeDimension.TwoDimensional)
                {
                    EditorGUILayout.BeginVertical("Box");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("twoDimensionalDetails"), true);
                    foreach (TwoDimensionalDetail twoDimensionalDetail in mazeGenerator.twoDimensionalDetails)
                    {
                        if (!twoDimensionalDetail.detailPrefab)
                        {
                            hasAllTwoDimensionalDetails = false;
                            break;
                        }
                    }
                    if (!hasAllTwoDimensionalDetails)
                    {
                        EditorGUILayout.HelpBox("You need to have all two dimensional detail prefabs filled in to generate a maze!", MessageType.Warning, true);
                    }
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.BeginVertical("Box");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("wallDetails"), true);
                    foreach (WallDetail wallDetail in mazeGenerator.wallDetails)
                    {
                        if (!wallDetail.detailPrefab)
                        {
                            hasAllWallDetails = false;
                            break;
                        }
                    }
                    if (!hasAllWallDetails)
                    {
                        EditorGUILayout.HelpBox("You need to have all wall detail prefabs filled in to generate a maze!", MessageType.Warning, true);
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical("Box");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("otherDetails"), true);
                    foreach (OtherDetail otherDetail in mazeGenerator.otherDetails)
                    {
                        if (!otherDetail.detailPrefab)
                        {
                            hasAllOtherDetails = false;
                            break;
                        }
                    }
                    if (!hasAllOtherDetails)
                    {
                        EditorGUILayout.HelpBox("You need to have all other detail prefabs filled in to generate a maze!", MessageType.Warning, true);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
            #endregion

            #region Floor Exit Stuff
            EditorGUILayout.BeginVertical("Box");
            SerializedProperty makeFloorExitProperty = serializedObject.FindProperty("makeFloorExit");
            GUIContent makeFloorExitContent = new GUIContent("Make Floor Exit?", makeFloorExitProperty.tooltip);
            EditorGUILayout.PropertyField(makeFloorExitProperty, makeFloorExitContent);
            if (makeFloorExitProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("exitType"));
                EditorGUI.indentLevel--;
                SerializedProperty exitPiecesProperty = serializedObject.FindProperty("exitPieces");
                GUIContent exitPiecesContent = new GUIContent("Exit Pieces", exitPiecesProperty.tooltip);
                SerializedProperty numberOfExitPiecesProperty = serializedObject.FindProperty("numberOfExitPieces");
                SerializedProperty showExitPiecesProperty = serializedObject.FindProperty("showExitPieces");
                DisplayGameObjectArrayFormatted(exitPiecesContent, showExitPiecesProperty, exitPiecesProperty, numberOfExitPiecesProperty);
            }
            EditorGUILayout.EndVertical();
            #endregion

            #region Floor Start Stuff
            EditorGUILayout.BeginVertical("Box");
            SerializedProperty makeFloorStartProperty = serializedObject.FindProperty("makeStart");
            GUIContent makeFloorStartContent = new GUIContent("Make Start?", makeFloorExitProperty.tooltip);
            EditorGUILayout.PropertyField(makeFloorStartProperty, makeFloorStartContent);
            if (makeFloorStartProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("startType"));
                EditorGUI.indentLevel--;
                SerializedProperty startPiecesProperty = serializedObject.FindProperty("startPieces");
                GUIContent startPiecesContent = new GUIContent("Start Pieces", startPiecesProperty.tooltip);
                SerializedProperty numberOfStartPiecesProperty = serializedObject.FindProperty("numberOfStartPieces");
                SerializedProperty showStartPiecesProperty = serializedObject.FindProperty("showStartPieces");
                DisplayGameObjectArrayFormatted(startPiecesContent, showStartPiecesProperty, startPiecesProperty, numberOfStartPiecesProperty);
            }
            EditorGUILayout.EndVertical();
            #endregion

            #region Outer Wall Deletion Stuff
            EditorGUILayout.BeginVertical("Box");
            SerializedProperty deleteOutsideWallsProperty = serializedObject.FindProperty("deleteOutsideWalls");
            GUIContent deleteOutsideWallContent = new GUIContent("Delete Outside Wall Pieces?", deleteOutsideWallsProperty.tooltip);
            EditorGUILayout.PropertyField(deleteOutsideWallsProperty, deleteOutsideWallContent);
            if (deleteOutsideWallsProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("outsideWallDeleteMode"));
                EditorGUI.indentLevel--;
                SerializedProperty outsideWallPiecesToDeleteProperty = serializedObject.FindProperty("outsideWallPiecesToDelete");
                GUIContent outsideWallPiecesToDeleteContent = new GUIContent("Number of Pieces to Delete", outsideWallPiecesToDeleteProperty.tooltip);
                EditorGUILayout.PropertyField(outsideWallPiecesToDeleteProperty, outsideWallPiecesToDeleteContent);
            }
            EditorGUILayout.EndVertical();
            # endregion

            #region Unique Tile Stuff
            EditorGUILayout.BeginVertical("Box");
            SerializedProperty makeUniqueTilesProperty = serializedObject.FindProperty("makeUniqueTiles");
            GUIContent makeUniqueTilesContent = new GUIContent("Make Unique Tiles?", makeUniqueTilesProperty.tooltip);
            EditorGUILayout.PropertyField(makeUniqueTilesProperty, makeUniqueTilesContent);
            if (makeUniqueTilesProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("uniqueTiles"), true);
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
            #endregion

            #region Braid Maze Stuff
            EditorGUILayout.BeginVertical("Box");
            GUIContent makeBraidMazeContent = new GUIContent("Make Braid Maze?");
            SerializedProperty makeBraidMazeProperty = serializedObject.FindProperty("makeBraidMaze");
            EditorGUILayout.PropertyField(makeBraidMazeProperty, makeBraidMazeContent);
            if (makeBraidMazeProperty.boolValue)
            {
                EditorGUILayout.HelpBox("This maze will have all dead ends removed.", MessageType.Info);
            }
            EditorGUILayout.EndVertical();
            #endregion

            #region Maze Name Stuff
            EditorGUILayout.BeginVertical("Box");
            GUIContent mazeNameContent = new GUIContent("Maze Name");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mazeName"), mazeNameContent);
            EditorGUILayout.EndVertical();
            #endregion

            #region Allow Undo Stuff
            EditorGUILayout.BeginVertical("Box");
            GUIContent allowEditorUndoContent = new GUIContent("Allow Undo in Editor?");
            SerializedProperty allowEditorUndoProperty = serializedObject.FindProperty("allowEditorUndo");
            EditorGUILayout.PropertyField(allowEditorUndoProperty, allowEditorUndoContent);
            EditorGUILayout.EndVertical();
            if (allowEditorUndoProperty.boolValue)
            {
                EditorGUILayout.HelpBox("Warning: Enabling this feature will allow you to undo generation actions in the Unity Editor, but will also drasticly affect the amount of RAM allocated to Unity if large mazes are being stored in Unity's built-in undo system. This is not advised unless you're just playing around with small mazes or feel like you REALLY need it.", MessageType.Warning, true);
            }
            #endregion

            EditorGUILayout.Space();

            if (mazeGenerator.gameObject.IsSceneObject())
            { 
                if (mazeGenerator.wallPieces.Length > 0 && mazeGenerator.floorPieces.Length > 0 && hasAllPlazas && hasAllWalls && hasAllFloors && hasAllWallDetails && hasAllOtherDetails)
                {
                    GUIContent generateMazeButtonContent = new GUIContent("Generate New Maze", "This button will destroy the current maze and generate a new one in its place.");
                    if (GUILayout.Button(generateMazeButtonContent))
                    {
                        DestroyOldMazeInEditor(mazeGenerator);
                        if (mazeGenerator.allowEditorUndo)
                        {
                            Undo.RegisterCreatedObjectUndo(mazeGenerator.GenerateNewMaze(), "Generate New Maze");
                        }
                        else
                        {
                            mazeGenerator.GenerateNewMaze();
                        } 
                    }
                }
                else
                {
                    if (mazeGenerator.wallPieces.Length == 0)
                    {
                        EditorGUILayout.HelpBox("You don't have any wall pieces specified. At least one is required to generate a maze.", MessageType.Error, true);
                    }

                    if (mazeGenerator.floorPieces.Length == 0)
                    {
                        EditorGUILayout.HelpBox("You don't have any floor pieces specified. At least one is required to generate a maze.", MessageType.Error, true);
                    }

                    if (!hasAllWalls)
                    {
                        EditorGUILayout.HelpBox("Some of your wall piece entries are null. They must all be filled to generate a maze.", MessageType.Error, true);
                    }

                    if (!hasAllFloors)
                    {
                        EditorGUILayout.HelpBox("Some of your floor piece entries are null. They must all be filled to generate a maze.", MessageType.Error, true);
                    }

                    if (!hasAllPlazas)
                    {
                        EditorGUILayout.HelpBox("Some of your plaza entries are null. They must all be filled to generate a maze.", MessageType.Error, true);
                    }

                    if (!hasAllWallDetails)
                    {
                        EditorGUILayout.HelpBox("Some of your wall detail entries are null. They must all be filled to generate a maze.", MessageType.Error, true);
                    }

                    if (!hasAllOtherDetails)
                    {
                        EditorGUILayout.HelpBox("Some of your floor detail entries are null. They must all be filled to generate a maze.", MessageType.Error, true);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DestroyOldMazeInEditor(MazeGenerator theMazeGenerator)
        {
            Maze[] mazes = FindObjectsOfType<Maze>();
            foreach (Maze maze in mazes)
            {
                if (maze.gameObject.name.Equals(theMazeGenerator.mazeName))
                {
                    if (theMazeGenerator.allowEditorUndo)
                    {
                        Undo.DestroyObjectImmediate(maze.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(maze.gameObject);
                    }
                }
            }
        }

        private void DisplayWallsFormatted(GUIContent wallPiecesContent, SerializedProperty showWallPieces, SerializedProperty wallPiecesProperty, SerializedProperty numberOfWallPiecesProperty)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUI.indentLevel++;
            showWallPieces.boolValue = EditorGUILayout.Foldout(showWallPieces.boolValue, wallPiecesContent);
            if (showWallPieces.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 1.1f));
                GUIContent numberOfWallPiecesContent = new GUIContent("Number of Pieces", numberOfWallPiecesProperty.tooltip);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(numberOfWallPiecesProperty, numberOfWallPiecesContent);
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("+"))
                {
                    numberOfWallPiecesProperty.intValue++;
                }
                if (GUILayout.Button("-"))
                {
                    if (numberOfWallPiecesProperty.intValue > 1)
                    {
                        numberOfWallPiecesProperty.intValue--;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
                while (wallPiecesProperty.arraySize != numberOfWallPiecesProperty.intValue)
                {
                    if (wallPiecesProperty.arraySize < numberOfWallPiecesProperty.intValue)
                    {
                        wallPiecesProperty.InsertArrayElementAtIndex(wallPiecesProperty.arraySize);
                        wallPiecesProperty.GetArrayElementAtIndex(wallPiecesProperty.arraySize - 1).FindPropertyRelative("wallPrefab").objectReferenceValue = null;
                        wallPiecesProperty.GetArrayElementAtIndex(wallPiecesProperty.arraySize - 1).FindPropertyRelative("wallHeight").floatValue = serializedObject.FindProperty("defaultWallHeight").floatValue;
                    }
                    else
                    {
                        wallPiecesProperty.DeleteArrayElementAtIndex(wallPiecesProperty.arraySize - 1);
                    }
                }
                int layoutTracker = 0;
                for (int index = 0; index < wallPiecesProperty.arraySize; index++)
                {
                    if (layoutTracker % 2 == 0)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 1.1f));
                        EditorGUILayout.Space();
                    }
                    EditorGUILayout.BeginVertical();
                    float elementWidth = 130f; //130 is default
                    if (wallPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("wallPrefab").objectReferenceValue)
                    {
                        Texture2D wallPreview;
                        do
                        {
                            wallPreview = AssetPreview.GetAssetPreview(wallPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("wallPrefab").objectReferenceValue);
                        } while (!wallPreview);
                        GUILayout.Label(wallPreview);
                        elementWidth = wallPreview.width;
                    }
                    EditorGUI.indentLevel--;
                    wallPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("wallPrefab").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField(wallPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("wallPrefab").objectReferenceValue, typeof(GameObject), false, GUILayout.Width(elementWidth));
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Height", GUILayout.Width(elementWidth * 0.5f));
                    wallPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("wallHeight").floatValue = EditorGUILayout.FloatField(wallPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("wallHeight").floatValue, GUILayout.Width(elementWidth * 0.5f));
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel++;
                    EditorGUILayout.EndVertical();
                    if (layoutTracker % 2 == 1)
                    {
                        EditorGUILayout.EndHorizontal();
                        if (index != wallPiecesProperty.arraySize - 1)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                        }
                    }
                    layoutTracker++;
                }
                if (wallPiecesProperty.arraySize % 2 == 1)
                {
                    EditorGUILayout.LabelField("", GUILayout.MaxWidth(Screen.width / 2.4f));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DisplayWallsFormatted2D(GUIContent wallPiecesContent, SerializedProperty showWallPieces, SerializedProperty wallPiecesProperty, SerializedProperty numberOfWallPiecesProperty)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUI.indentLevel++;
            showWallPieces.boolValue = EditorGUILayout.Foldout(showWallPieces.boolValue, wallPiecesContent);
            if (showWallPieces.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 1.1f));
                GUIContent numberOfWallPiecesContent = new GUIContent("Number of Pieces", numberOfWallPiecesProperty.tooltip);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(numberOfWallPiecesProperty, numberOfWallPiecesContent);
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("+"))
                {
                    numberOfWallPiecesProperty.intValue++;
                }
                if (GUILayout.Button("-"))
                {
                    if (numberOfWallPiecesProperty.intValue > 1)
                    {
                        numberOfWallPiecesProperty.intValue--;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
                while (wallPiecesProperty.arraySize != numberOfWallPiecesProperty.intValue)
                {
                    if (wallPiecesProperty.arraySize < numberOfWallPiecesProperty.intValue)
                    {
                        wallPiecesProperty.InsertArrayElementAtIndex(wallPiecesProperty.arraySize);
                        wallPiecesProperty.GetArrayElementAtIndex(wallPiecesProperty.arraySize - 1).FindPropertyRelative("wallPrefab").objectReferenceValue = null;
                        wallPiecesProperty.GetArrayElementAtIndex(wallPiecesProperty.arraySize - 1).FindPropertyRelative("wallZPlane").floatValue = serializedObject.FindProperty("defaultWallZPlane").floatValue;
                    }
                    else
                    {
                        wallPiecesProperty.DeleteArrayElementAtIndex(wallPiecesProperty.arraySize - 1);
                    }
                }
                int layoutTracker = 0;
                for (int index = 0; index < wallPiecesProperty.arraySize; index++)
                {
                    if (layoutTracker % 2 == 0)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 1.1f));
                        EditorGUILayout.Space();
                    }
                    EditorGUILayout.BeginVertical();
                    float elementWidth = 130f; //130 is default
                    if (wallPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("wallPrefab").objectReferenceValue)
                    {
                        Texture2D wallPreview;
                        do
                        {
                            wallPreview = AssetPreview.GetAssetPreview(wallPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("wallPrefab").objectReferenceValue);
                        } while (!wallPreview);
                        GUILayout.Label(wallPreview);
                        elementWidth = wallPreview.width;
                    }
                    EditorGUI.indentLevel--;
                    wallPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("wallPrefab").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField(wallPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("wallPrefab").objectReferenceValue, typeof(GameObject), false, GUILayout.Width(elementWidth));
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Z-Plane", GUILayout.Width(elementWidth * 0.5f));
                    wallPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("wallZPlane").floatValue = EditorGUILayout.FloatField(wallPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("wallZPlane").floatValue, GUILayout.Width(elementWidth * 0.5f));
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel++;
                    EditorGUILayout.EndVertical();
                    if (layoutTracker % 2 == 1)
                    {
                        EditorGUILayout.EndHorizontal();
                        if (index != wallPiecesProperty.arraySize - 1)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                        }
                    }
                    layoutTracker++;
                }
                if (wallPiecesProperty.arraySize % 2 == 1)
                {
                    EditorGUILayout.LabelField("", GUILayout.MaxWidth(Screen.width / 2.4f));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DisplayFloorsFormatted(GUIContent floorPiecesContent, SerializedProperty showFloorPieces, SerializedProperty floorPiecesProperty, SerializedProperty numberOfFloorPiecesProperty)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUI.indentLevel++;
            showFloorPieces.boolValue = EditorGUILayout.Foldout(showFloorPieces.boolValue, floorPiecesContent);
            if (showFloorPieces.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 1.1f));
                GUIContent numberOfFloorPiecesContent = new GUIContent("Number of Pieces", numberOfFloorPiecesProperty.tooltip);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(numberOfFloorPiecesProperty, numberOfFloorPiecesContent);
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("+"))
                {
                    numberOfFloorPiecesProperty.intValue++;
                }
                if (GUILayout.Button("-"))
                {
                    if (numberOfFloorPiecesProperty.intValue > 1)
                    {
                        numberOfFloorPiecesProperty.intValue--;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
                while (floorPiecesProperty.arraySize != numberOfFloorPiecesProperty.intValue)
                {
                    if (floorPiecesProperty.arraySize < numberOfFloorPiecesProperty.intValue)
                    {
                        floorPiecesProperty.InsertArrayElementAtIndex(floorPiecesProperty.arraySize);
                        floorPiecesProperty.GetArrayElementAtIndex(floorPiecesProperty.arraySize - 1).FindPropertyRelative("floorPrefab").objectReferenceValue = null;
                        floorPiecesProperty.GetArrayElementAtIndex(floorPiecesProperty.arraySize - 1).FindPropertyRelative("floorThickness").floatValue = serializedObject.FindProperty("defaultFloorThickness").floatValue;
                    }
                    else
                    {
                        floorPiecesProperty.DeleteArrayElementAtIndex(floorPiecesProperty.arraySize - 1);
                    }
                }
                int layoutTracker = 0;
                for (int index = 0; index < floorPiecesProperty.arraySize; index++)
                {
                    if (layoutTracker % 2 == 0)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 1.1f));
                        EditorGUILayout.Space();
                    }
                    EditorGUILayout.BeginVertical();
                    float elementWidth = 130f; //130 is default
                    if (floorPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("floorPrefab").objectReferenceValue)
                    {
                        Texture2D floorPreview;
                        do
                        {
                            floorPreview = AssetPreview.GetAssetPreview(floorPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("floorPrefab").objectReferenceValue);
                        } while (!floorPreview);
                        GUILayout.Label(floorPreview);
                        elementWidth = floorPreview.width;
                    }
                    EditorGUI.indentLevel--;
                    floorPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("floorPrefab").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField(floorPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("floorPrefab").objectReferenceValue, typeof(GameObject), false, GUILayout.Width(elementWidth));
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Thickness", GUILayout.Width(elementWidth * 0.5f));
                    floorPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("floorThickness").floatValue = EditorGUILayout.FloatField(floorPiecesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("floorThickness").floatValue, GUILayout.Width(elementWidth * 0.5f));
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel++;
                    EditorGUILayout.EndVertical();
                    if (layoutTracker % 2 == 1)
                    {
                        EditorGUILayout.EndHorizontal();
                        if (index != floorPiecesProperty.arraySize - 1)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                        }
                    }
                    layoutTracker++;
                }
                if (floorPiecesProperty.arraySize % 2 == 1)
                {
                    EditorGUILayout.LabelField("", GUILayout.MaxWidth(Screen.width / 2.4f));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DisplayGameObjectArrayFormatted(GUIContent arrayContent, SerializedProperty showItemsProperty, SerializedProperty arrayProperty, SerializedProperty numberOfItemsProperty)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUI.indentLevel++;
            showItemsProperty.boolValue = EditorGUILayout.Foldout(showItemsProperty.boolValue, arrayContent);
            if (showItemsProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 1.1f));
                GUIContent numberOfFloorPiecesContent = new GUIContent("Number of Items", numberOfItemsProperty.tooltip);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(numberOfItemsProperty, numberOfFloorPiecesContent);
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("+"))
                {
                    numberOfItemsProperty.intValue++;
                }
                if (GUILayout.Button("-"))
                {
                    if (numberOfItemsProperty.intValue > 1)
                    {
                        numberOfItemsProperty.intValue--;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
                while (arrayProperty.arraySize != numberOfItemsProperty.intValue)
                {
                    if (arrayProperty.arraySize < numberOfItemsProperty.intValue)
                    {
                        arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
                        arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1).objectReferenceValue = null;
                    }
                    else
                    {
                        arrayProperty.DeleteArrayElementAtIndex(arrayProperty.arraySize - 1);
                    }
                }
                int layoutTracker = 0;
                for (int index = 0; index < arrayProperty.arraySize; index++)
                {
                    if (layoutTracker % 2 == 0)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 1.1f));
                        EditorGUILayout.Space();
                    }
                    EditorGUILayout.BeginVertical();
                    float elementWidth = 130f; //130 is default
                    if (arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue)
                    {
                        Texture2D floorPreview;
                        do
                        {
                            floorPreview = AssetPreview.GetAssetPreview(arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue);
                        } while (!floorPreview);
                        GUILayout.Label(floorPreview);
                        elementWidth = floorPreview.width;
                    }
                    EditorGUI.indentLevel--;
                    arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue = (GameObject)EditorGUILayout.ObjectField(arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue, typeof(GameObject), false, GUILayout.Width(elementWidth));
                    EditorGUI.indentLevel++;
                    EditorGUILayout.EndVertical();
                    if (layoutTracker % 2 == 1)
                    {
                        EditorGUILayout.EndHorizontal();
                        if (index != arrayProperty.arraySize - 1)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                        }
                    }
                    layoutTracker++;
                }
                if (arrayProperty.arraySize % 2 == 1)
                {
                    EditorGUILayout.LabelField("", GUILayout.MaxWidth(Screen.width / 2.4f));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }
}
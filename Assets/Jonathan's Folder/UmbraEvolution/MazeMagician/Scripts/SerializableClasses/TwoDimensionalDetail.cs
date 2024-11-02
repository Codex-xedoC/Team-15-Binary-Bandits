//Name: Robert MacGillivray
//File: TwoDimensionalDetail.cs
//Date: Apr.03.2016
//Purpose: To store information about two dimensional details scattered throughout a 2D maze

//Last Updated: Jul.01.2022 by Robert MacGillivray

using UnityEngine;

namespace UmbraEvolution.UmbraMazeMagician
{
    [System.Serializable]
    public class TwoDimensionalDetail
    {
        [Tooltip("Feel free to name the detail. This name will be used during instantiation.")]
        public string detailName;
        [Tooltip("The prefab to be placed. Must be oriented so that the pivot is in the center, and the z-axis points outward relative to where the detail would be placed on the maze wall.")]
        public GameObject detailPrefab;
        [Tooltip("The height of the object along its y-axis. Used to make sure that it is placed within appropriate bounds of the floor tile")]
        public float height;
        [Tooltip("How wide the object is along its x-axis. Used to make sure that it is placed within appropriate bounds of the floor tile")]
        public float width;
        [Tooltip("The z-value of this detail when scattered throughout the maze")]
        public float zPlane;
        [Tooltip("The minimum offset along the x-axis of the maze relative to the center of a tile.")]
        public float minXOffset;
        [Tooltip("The maximum offset along the x-axis of the maze relative to the center of a tile.")]
        public float maxXOffset;
        [Tooltip("The minimum offset along the y-axis of the maze relative to the center of a tile.")]
        public float minYOffset;
        [Tooltip("The maximum offset along the y-axis of the maze relative to the center of a tile.")]
        public float maxYOffset;
        [Tooltip("The frequency with which this item will generally appear on a tile. 0 is never, 1 is always.")]
        [Range(0f, 1f)]
        public float frequency;

        public TwoDimensionalDetail(TwoDimensionalDetail detailToCopy)
        {
            detailName = detailToCopy.detailName;
            detailPrefab = detailToCopy.detailPrefab;
            height = detailToCopy.height;
            width = detailToCopy.width;
            zPlane = detailToCopy.zPlane;
            minXOffset = detailToCopy.minXOffset;
            maxXOffset = detailToCopy.maxXOffset;
            minYOffset = detailToCopy.minYOffset;
            maxYOffset = detailToCopy.maxYOffset;
            frequency = detailToCopy.frequency;
        }
    }
}

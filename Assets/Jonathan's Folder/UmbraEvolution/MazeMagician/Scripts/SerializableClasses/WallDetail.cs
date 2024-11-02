//Name: Robert MacGillivray
//Date: Jun.20.2015
//File: WallDetail.cs
//Purpose: A class to hold information about details that are placed on the walls of the maze

//Last Updated: Jul.01.2022 by Robert MacGillivray

using UnityEngine;

namespace UmbraEvolution.UmbraMazeMagician
{
    [System.Serializable]
    public class WallDetail
    {
        [Tooltip("Feel free to name the detail. This name will be used during instantiation.")]
        public string detailName;
        [Tooltip("The prefab to be placed. Must be oriented so that the pivot is in the center, and the z-axis points outward relative to where the detail would be placed on the maze wall.")]
        public GameObject detailPrefab;
        [Tooltip("How thick the object is along its z-axis. Used to place it properly, assuming that your pivot point is centered and oriented correctly.")]
        public float thickness;
        [Tooltip("The height of the object along its y-axis. Used to make sure that it is placed within appropriate vertical bounds of the wall face.")]
        public float height;
        [Tooltip("How wide the object is along its x-axis. Used to make sure that it is placed within appropriate horizontal bounds of the wall face.")]
        public float width;
        [Tooltip("The minimum height from the floor that this detail will be placed.")]
        public float minHeight;
        [Tooltip("The maximum height from the floor that this detail will be placed.")]
        public float maxHeight;
        [Tooltip("The leftmost distance from the center of the wall that the detail can be placed.")]
        public float minHorizontalOffset;
        [Tooltip("The rightmost distance from the center of the wall that the detail can be placed.")]
        public float maxHorizontalOffset;
        [Tooltip("The frequency with which this item will generally appear on a wall. 0 is never, 1 is always.")]
        [Range(0f, 1f)]
        public float frequency;

        public WallDetail(WallDetail detailToCopy)
        {
            detailName = detailToCopy.detailName;
            detailPrefab = detailToCopy.detailPrefab;
            thickness = detailToCopy.thickness;
            height = detailToCopy.height;
            width = detailToCopy.width;
            minHeight = detailToCopy.minHeight;
            maxHeight = detailToCopy.maxHeight;
            minHorizontalOffset = detailToCopy.minHorizontalOffset;
            maxHorizontalOffset = detailToCopy.maxHorizontalOffset;
            frequency = detailToCopy.frequency;
        }
    }
}

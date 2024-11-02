//Name: Robert MacGillivray
//Date: Jun.20.2015
//File: OtherDetail.cs
//Purpose: A class to hold information about details that are placed throughout the maze

//Last Updated: Jul.01.2022 by Robert MacGillivray

using UnityEngine;

namespace UmbraEvolution.UmbraMazeMagician
{
    [System.Serializable]
    public class OtherDetail
    {
        [Tooltip("Feel free to name the detail. This name will be used during instantiation.")]
        public string detailName;
        [Tooltip("The prefab to be placed. Must be oriented so that the pivot is in the center, and the z-axis points outward relative to where the detail would be placed on the maze wall.")]
        public GameObject detailPrefab;
        [Tooltip("How thick the object is along its z-axis. Used to make sure that it is placed within appropriate bounds of the floor tile")]
        public float thickness;
        [Tooltip("The height of the object along its y-axis. Used to make sure that it does not clip into the floor")]
        public float height;
        [Tooltip("How wide the object is along its x-axis. Used to make sure that it is placed within appropriate bounds of the floor tile")]
        public float width;
        [Tooltip("The minimum height from the floor that this detail will be placed.")]
        public float minHeight;
        [Tooltip("The maximum height from the floor that this detail will be placed.")]
        public float maxHeight;
        [Tooltip("The minimum offset along the x-axis of the maze relative to the center of a tile.")]
        public float minXOffset;
        [Tooltip("The maximum offset along the x-axis of the maze relative to the center of a tile.")]
        public float maxXOffset;
        [Tooltip("The minimum offset along the z-axis of the maze relative to the center of a tile.")]
        public float minZOffset;
        [Tooltip("The maximum offset along the z-axis of the maze relative to the center of a tile.")]
        public float maxZOffset;
        [Tooltip("The frequency with which this item will generally appear on a tile. 0 is never, 1 is always.")]
        [Range(0f, 1f)]
        public float frequency;

        public OtherDetail(OtherDetail detailToCopy)
        {
            detailName = detailToCopy.detailName;
            detailPrefab = detailToCopy.detailPrefab;
            thickness = detailToCopy.thickness;
            height = detailToCopy.height;
            width = detailToCopy.width;
            minHeight = detailToCopy.minHeight;
            maxHeight = detailToCopy.maxHeight;
            minXOffset = detailToCopy.minXOffset;
            maxXOffset = detailToCopy.maxXOffset;
            minZOffset = detailToCopy.minZOffset;
            maxZOffset = detailToCopy.maxZOffset;
            frequency = detailToCopy.frequency;
        }
    }
}

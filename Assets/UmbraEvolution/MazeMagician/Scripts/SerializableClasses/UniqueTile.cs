//Name: Robert MacGillivray
//File: UniqueTile.cs
//Date: Sept.30.2017
//Purpose: To hold information about unique tiles

//Last Updated: Jul.01.2022 by Robert MacGillivray

using UnityEngine;

namespace UmbraEvolution.UmbraMazeMagician
{
    [System.Serializable]
    public class UniqueTile
    {
        [Tooltip("Feel free to name the unique tile. This name will be used during instantiation.")]
        public string uniqueTileName;
        [Tooltip("One of these prefabs will replace a floor piece somewhere in the maze (like the exit)")]
        public GameObject[] tileVariations;
        public enum Placement
        {
            random,
            center,
            outside
        }
        [Tooltip("Center: tile will be located in roughly the center third of the maze.\nOutside: tile will be placed in the ring around that center\nRandom: tile can be anywhere.")]
        public Placement placement;
    }
}

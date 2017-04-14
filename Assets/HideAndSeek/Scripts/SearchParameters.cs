using UnityEngine;


namespace HideAndSeek
{
    /// <summary>
    /// Defines the parameters which will be used to find a path across a section of the map
    /// </summary>
    public class SearchParameters
    {
        public Vector2 StartLocation { get; set; }

        public Vector2 EndLocation { get; set; }
        
        public bool[,] Map { get; set; }

        public SearchParameters(Vector2 startLocation, Vector2 endLocation, bool[,] map)
        {
            this.StartLocation = startLocation;
            this.EndLocation = endLocation;
            this.Map = map;
        }
    }
}

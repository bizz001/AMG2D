namespace AMG2D.Configuration.Enum
{
    /// <summary>
    /// Enum that specifies where to position external objects.
    /// </summary>
    public enum EObjectPosition
    {
        /// <summary>
        /// External objects will be placed anywhere on the map.
        /// </summary>
        Any = 0,

        /// <summary>
        /// External objects will be placed only in the air.
        /// </summary>
        Air,

        /// <summary>
        /// External objects will be placed only on the ground.
        /// </summary>
        OnGround,

        /// <summary>
        /// External objects will be placed inside the ground.
        /// </summary>
        Soil,

        /// <summary>
        /// External objects will be placed inside caves.
        /// </summary>
        Cave
    }
}
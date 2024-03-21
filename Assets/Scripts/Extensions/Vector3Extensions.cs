namespace Extensions
{
    public static class Vector3Extensions
    {
        // Convert UnityEngine.Vector3 to System.Numerics.Vector3
        public static System.Numerics.Vector3 ToSystemNumeric(this UnityEngine.Vector3 vector)
        {
            return new System.Numerics.Vector3(vector.x, vector.y, vector.z);
        }

        // Convert System.Numerics.Vector3 to UnityEngine.Vector3
        public static UnityEngine.Vector3 ToUnity(this System.Numerics.Vector3 vector)
        {
            return new UnityEngine.Vector3(vector.X, vector.Y, vector.Z);
        }
    }

}

namespace LambdaTheDev.SimpleJwt.Net.Utils
{
    // Some info about this SimpleJwt build
    public static class SimpleJwtInfo
    {
        // True, if uses experimental features
        public static bool IsExperimental
        {
            get
            {
#if SIMPLE_JWT_EXPERIMENTAL
                return true;
#endif
                return false;
            }
        }
    }
}
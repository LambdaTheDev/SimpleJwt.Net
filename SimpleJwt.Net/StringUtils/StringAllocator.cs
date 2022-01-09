using System.Reflection;

namespace LambdaTheDev.SimpleJwt.Net.StringUtils
{
    // MANUAL STRING ALLOCATOR
    // WARNING: THIS IS EXPERIMENTAL FEATURE, CONSIDERED BY C# RUNTIME AS ILLEGAL!
    //  HASH STRINGS ARE USUALLY UNIQUE, SO THERE ARE SMALL CHANCES THAT STRING WILL BE
    //  CACHED BY C#, BUT THERE IS ALWAYS A RISK OF APPLICATION CRASH!
    // 
    // Use on your own responsibility
    public static class StringAllocator
    {
#if SIMPLE_JWT_EXPERIMENTAL
        private static readonly MethodInfo FastAllocateStringMethod;
        private static readonly object[] Arguments = new object[1];
        
        static StringAllocator()
        {
            FastAllocateStringMethod = typeof(string).GetMethod("FastAllocateString", BindingFlags.Static | BindingFlags.NonPublic);
        }
#endif
        
        // Returns allocated string instance, or null if feature is disabled
        public static string Allocate(int size)
        {
#pragma warning disable 162
#if SIMPLE_JWT_EXPERIMENTAL

            Arguments[0] = size;
            string allocated = (string) FastAllocateStringMethod.Invoke(null, Arguments);
            return allocated;

#endif

            return null;
#pragma warning restore
        }
    }
}
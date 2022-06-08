using System;

namespace LambdaTheDev.SimpleJwt.Net.Utils
{
    // Delegate used to pass-by-reference to avoid struct copying
    internal delegate void ActionRef<T>(ref T item);
    
    // Wraps method to try-finally statement without struct boxing
    internal static class ValueUsing<T> where T : IDisposable
    {
        // Invokes action method, with provided value & calls dispose after invocation
        public static void Use(ref T value, ActionRef<T> action)
        {
            try
            {
                action.Invoke(ref value);
            }
            finally
            {
                value.Dispose();
            }
        }
    }
}
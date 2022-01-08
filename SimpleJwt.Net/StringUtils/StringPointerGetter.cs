using System;
using System.Reflection;

namespace LambdaTheDev.SimpleJwt.Net.StringUtils
{
    // This class allows to return first char of string, for example
    //  to use it in unsafe code. 
    public static class StringPointerGetter
    {
#if SIMPLE_JWT_EXPERIMENTAL
#if !NET
        // Mono framework workaround for first string getter
        private static readonly FieldInfo FirstCharField;

        static StringPointerGetter()
        {
            FirstCharField = typeof(string).GetField("_firstChar", BindingFlags.Instance | BindingFlags.NonPublic);
            if (FirstCharField == null)
            {
                throw new Exception("Could not find a _firstChar field used by Mono pointer getter!");
            }
        }
        
#endif
#endif
        
        // Returns true & char on success, false & empty char on failure
        public static bool TryGetFor(string input, out char value)
        {
#if SIMPLE_JWT_EXPERIMENTAL
#if NET
            // .NET simplified it. Mono - hasn't
            value = input.GetPinnableReference();
            return true;
#else

            object result = FirstCharField.GetValue(input);
            if (result == null)
            {
                value = default;
                return false;
            }

            value = (char) result;
            return true;

#endif
#endif
        }
    }
}
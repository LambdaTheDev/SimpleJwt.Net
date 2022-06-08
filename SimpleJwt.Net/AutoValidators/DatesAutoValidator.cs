using System;
using LambdaTheDev.SimpleJwt.Net.Claims;

namespace LambdaTheDev.SimpleJwt.Net.AutoValidators
{
    // IAT, NBF, & EXP claims validator
    public sealed class DatesAutoValidator : JwtAutoValidator
    {
        public override object Feedback { get; }
        

        public override bool Validate(object tokenPayload)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            if (tokenPayload is IIssuedAtClaim iat)
            {
                // Check if token isn't issued in future
                if (iat.Iat > now) return false;
            }

            if (tokenPayload is INotBeforeClaim nbf)
            {
                // Check if not trying to use it too early
                if (nbf.Nbf > now) return false;
            }

            if (tokenPayload is IExpirationClaim exp)
            {
                // Check if it's not expired
                if (exp.Exp < now) return false;
            }

            return true;
        }
    }
}
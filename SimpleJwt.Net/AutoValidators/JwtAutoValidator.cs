namespace LambdaTheDev.SimpleJwt.Net.AutoValidators
{
    // Base class for JWT token validation components
    public abstract class JwtAutoValidator
    {
        // Last validation feedback
        public abstract object Feedback { get; }
        
        // Returns true, if token passes validation. Otherwise - false
        public abstract bool Validate(object tokenPayload);
    }
}
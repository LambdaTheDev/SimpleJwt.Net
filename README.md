# SimpleJwt.Net
Welcome! This library allows you to create & validate your custom Json Web Tokens easily & performant!

# Why SimpleJwt.Net instead of other libraries?
- My main goal was to reduce runtime memory allocations wherever possible. In most places I pre-allocate char/byte array and re-use it.
- I simplified token generation - to add JWT claims, all you need is to implement simple interfaces.
- Claims IAT, NBF, and EXP are automatically validated.
- Commented code. If you do not understand something, just read method/class comments.

# Creating & validating token
- Define a token type & implement claims interfaces:
```cs
public class MyToken : ISubjectClaim<int>, IExpirationClaim
{
    public int Sub { get; set; }
    public DateTime Exp { get; set; }
    public string MyCustomClaim { get; set; }
}
```
- Make an instance of SimpleJsonWebToken class
```cs
var algorithm = new Hmac256(); // Or other supported hashing algorithm instance
var sjwt = new SimpleJsonWebTokens(algorithm);
```
- Make instance of a token type & generate token
```cs
var algorithm = new Hmac256();
var sjwt = new SimpleJsonWebTokens(algorithm);

MyToken tokenPayload = new MyToken
{
    Sub = 1234,
    Exp = DateTimeOffset.UtcNow.AddHours(3).DateTime, // Token will be valid for 3 hours 
    MyCustomClaim = "Example", // Or any other custom value you want
};

string token = sjwt.Generate<MyToken>(tokenPayload);
```
- Validate incoming token
```cs
string token = ...; // Get the token, for example from HTTP header

try
{
    // That method throws InvalidTokenException if token is:
    // a) Issued in future (IAT claim > DateTime.UtcNow)
    // b) Used before it's allowed (NBF claim > DateTime.UtcNow)
    // c) Expired (EXP claim <= DateTime.UtcNow)

    MyToken tokenPayload = sjwt.Validate<T>(token); 
    
    // Don't forget to validate your custom claims!
    if(tokenPayload.MyCustomClaim != "Example")
    {
        // Here do action when your custom claim fails.
        // Although it's suggested to throw an InvalidTokenException, with
        // a CustomClaimFailed cause, to handle token failure only in catch block.
        throw new InvalidTokenException(JwtFailureCause.CustomClaimFailed);
    }

    // From that moment token is marked as valid.
}
catch(InvalidTokenException e)
{
    // Something is wrong with token. You can check what:
    JwtFailureCause cause = e.Cause;
    // Inform rest of your application about JWT rejection casue.
}
```
# What are currently supported claims & how to use them?
Currently, all claims from JWT RFC are implemented as interfaces. But, let's divide them into 2 parts:

a) Auto-validated claims:
- IExpirationClaim (exp)
- IIssuedAtClaim (iat)
- INotBeforeClaim (nbf)

b) Manually-validated claims:
- IAudienceClaim<T> - (aud)
- IIssuerClaim<T> - (iss)
- IJwtIdClaim - (jti)
- ISubjectClaim<T> - (sub)

But, what's with those generics? It's for your convenience! You can provide a type that represents your 
audience, issuer or subject. This is used to get rid of pointless string conversions.

# I want to contribute, what now?
If you want to contribute, all you need is to submit Pull Request (do one PR per new feature). Although, please
do your best to make least allocations possible in your code - better pre-allocate when you can.

# Third-party attributions
Base64Encoder - https://github.com/ramonsmits/Base64Encoder - All I did is reducing runtime allocations.
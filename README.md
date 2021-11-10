# SimpleJwt.Net
It's a C# JWT library, with simplified (at least for me) usage. Right now, it contains following claims:
- Iss
- Sub
- Iat
- Exp

But it's planned to support all of claims defined in RFC.

# Why use this?
That library makes you very easy & performant creation of your own claims!

# How to use it?
- First, you need to create an IJwtAlgorithm object (right now, only supported algorithm is HS256 with a fixed secret key).
- Next, create instances of `JwtGenerator` and `JwtValidator`. In constructor, provide chosen IJwtAlgorithm instance and issuer name (to provide ISS claim checks automatically).
- Next, create your JWT struct & implement IJwtBasicPayload interface, so it looks like that:
```cs
    public struct MyJwt : IJwtBasicPayload
    {
        // Implementation of IJwtBasicPayload
        public string Iss { get; set; }
        public string Sub { get; set; }
        public DateTime Iat { get; set; }
        public DateTime Exp { get; set; }
        
        // Your custom claims
        public string Secret { get; set; }
        public int Nonce { get; set; }
        // etc...
    }
```
- After you have struct ready with your custom claims (or without them, your choice), prepare to JWT generation. First make an instance of your JWT payload struct mentioned before. Fill out *ONLY YOUR CUSTOM CLAIMS*! Then, define expiration date (must be a UTC DateTime instance), and the subject (for example user ID), pass it to `_jwtGenerator.Generate<MyJwt>(myJwtInstance, expirationDateTime, subjectName)` & get your JWT!
- To validate your token, wrap the `_jwtValidator.Validate<MyJwt>(token)` method into try-catch block. If everything is fine, that method will return an instance of MyJwt with all data filled out in generator (remember to eventually validate your custom claims), or throw an JwtException. To get a cause, why token got rejected, just grab the `jwtException.Cause` value (in /Exceptions/JwtFailureCause.cs you can read more about what each enum value means).
- Enjoy!

# Are you planning to extend it's functionalities?
I plan to implement the IJwtAdvancedPayload system, with all claims defined in RFC. But, this is right a small project, just for my needs, it does not contain many functionalities. But if more people will start using that tool, I will spend more time on it and add new claims more quickly & more hashing algorithms.
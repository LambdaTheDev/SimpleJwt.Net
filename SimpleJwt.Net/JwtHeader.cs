namespace SimpleJwt.Net
{
    // QOL struct to wrap JWT header
    internal struct JwtHeader
    {
        public string Typ { get; set; } // Type of token
        public string Alg { get; set; } // Picked algorithm
    }
}
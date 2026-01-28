namespace Application.Exceptions;

public class OrbiteOneException : Exception
{
    public int StatusCode { get; }

    public OrbiteOneException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}

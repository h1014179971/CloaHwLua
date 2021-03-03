public class SingletonException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SingletonException"/> class.
    /// </summary>
    /// <param name="msg">Message.</param>
    public SingletonException(string msg)
        : base(msg)
    {
    }
}

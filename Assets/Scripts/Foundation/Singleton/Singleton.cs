/// <summary>
/// Generic C# singleton.
/// </summary>
public abstract class Singleton<T> where T : class, new()
{

    /// <summary>
    /// The m_ instance.
    /// </summary>
    protected static T _Instance = null;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static T Instance
    {
        get
        {
            if (null == _Instance)
            {
                _Instance = new T();
            }
            return _Instance;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XHEngine.Singleton`1"/> class.
    /// </summary>
    protected Singleton()
    {
        if (null != _Instance)
            throw new SingletonException("This " + (typeof(T)).ToString() + " Singleton Instance is not null !!!");
        //Init();
    }


    /// <summary></summary>
    /// Init this Singleton.
    /// </summary>
    public virtual void Init() { }
    public static void Release()
    {
        _Instance = default(T);
    }
}

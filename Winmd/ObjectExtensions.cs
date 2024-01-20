namespace Winmd;

public static class ObjectExtensions
{
    public static T Tap<T>(this T instance, Action<T> action)
    {
        action.Invoke(instance);
        return instance;
    }
}

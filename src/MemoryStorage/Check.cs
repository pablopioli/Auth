namespace MemoryStorage;

internal static class Check
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> when obj is null.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <param name="name">The name of the object parameter.</param>
    /// <exception cref="ArgumentNullException"/>
    internal static void NotNull(object obj, string name)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(name);
        }
    }
}

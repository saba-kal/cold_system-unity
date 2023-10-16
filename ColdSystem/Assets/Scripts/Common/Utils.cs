public static class Utils
{
    public static T GetValueOrNull<T>(this T[] list, int index) where T : class
    {
        if (index < 0 || index >= list.Length)
        {
            return null;
        }

        return list[index];
    }
}

namespace Rikuiki_Simpson
{
    internal static class Extension
    {
        public static string TrimSpecies(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            var array = text.Split("亜目");
            text = array[0];
            array = text.Split("sp");
            text = array[0];
            return text;
        }
    }
}

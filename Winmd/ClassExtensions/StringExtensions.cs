namespace Winmd.ClassExtensions;

internal static class StringExtensions
{
    extension(string self)
    {
        public string StripEnd(string suffix) => self.EndsWith(suffix) ? self[..^suffix.Length] : self;
    }
}

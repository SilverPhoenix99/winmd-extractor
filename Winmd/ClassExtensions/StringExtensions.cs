﻿namespace Winmd.ClassExtensions;

static class StringExtensions
{
    public static string StripEnd(this string self, string suffix) =>
        self.EndsWith(suffix) ? self[..^suffix.Length] : self;
}

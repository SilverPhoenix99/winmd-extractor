using JetBrains.Annotations;

namespace Winmd.Model;

[Flags]
internal enum Architecture
{
    [UsedImplicitly] X86   = 1,
    [UsedImplicitly] X64   = 2,
    [UsedImplicitly] Arm64 = 4
}

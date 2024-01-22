namespace Winmd.Model;

enum ModelType
{
    Apis,      // class Apis - the list of available constants and functions
    // Attribute - not used
    Callback,
    Enum,
    Interface, // COM interfaces?
    Struct,    // struct or union
    Typedef,
    Object,    // generic fallback
}

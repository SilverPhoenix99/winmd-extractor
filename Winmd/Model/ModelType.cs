namespace Winmd.Model;

enum ModelType
{
    Interface, // COM interfaces?
    Enum,
    Callback,
    Struct,   // struct or union
    Typedef,
    Object    // class Apis - the list of available functions
    // Attribute - not used
}

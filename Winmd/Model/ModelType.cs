namespace Winmd.Model;

enum ModelType
{
    Apis,      // class Apis - the list of available constants and functions
    // Attribute - not used
    Callback,
    Constant,  // defines or consts
    Enum,
    Function,  // C functions
    Interface, // COM interfaces?
    Struct,
    Typedef,
    Union,
    Object,    // generic fallback
}

namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public enum Operator
    {
        OP_EQUAL,
        OP_NOT_EQUAL,
        OP_LESS,
        OP_LESS_EQUAL,
        OP_GREATER,
        OP_GREATER_EQUAL,
        OP_AND,
        OP_OR,
        OP_NOT,
        OP_NEGATE,
        OP_ADD,
        OP_SUB,
        OP_MUL,
        OP_DIV,
        OP_MOD,
        OP_SHIFT_LEFT,
        OP_SHIFT_RIGHT,
        OP_ASSIGN,
        OP_ASSIGN_ADD,
        OP_ASSIGN_SUB,
        OP_ASSIGN_MUL,
        OP_ASSIGN_DIV,
        OP_ASSIGN_MOD,
        OP_ASSIGN_SHIFT_LEFT,
        OP_ASSIGN_SHIFT_RIGHT,
        OP_ASSIGN_BIT_AND,
        OP_ASSIGN_BIT_OR,
        OP_ASSIGN_BIT_XOR,
        OP_BIT_AND,
        OP_BIT_OR,
        OP_BIT_XOR,
        OP_BIT_INVERT,
        OP_INCREMENT,
        OP_DECREMENT,
        OP_SELECT_IF,
        OP_SELECT_ELSE, //used only internally, then only IF appears with 3 arguments
        OP_POST_INCREMENT,
        OP_POST_DECREMENT,
        OP_CALL,
        OP_CONSTRUCT,
        OP_STRUCT,
        OP_INDEX,
        OP_EMPTY,
        OP_MAX
    }
}

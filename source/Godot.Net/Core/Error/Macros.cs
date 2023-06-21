namespace Godot.Net.Core.Error;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Godot.Net.Core.IO;

using OS = OS.OS;


public record ErrorHandlerList(object UserData, ErrorHandlerFunc ErrorHandler);

public static class Macros
{
    public delegate void ErrorHandlerFunc(object userData, string function, string file, int line, string error, string message, bool editorNotify, ErrorHandlerType type);

    public enum ErrorHandlerType
    {
        ERR_HANDLER_ERROR,
        ERR_HANDLER_WARNING,
        ERR_HANDLER_SCRIPT,
        ERR_HANDLER_SHADER,
    };

    private static readonly LinkedList<ErrorHandlerList> errorHandlerList = new();
    private static readonly object                       locker           = new();

    private static void ErrPrintIndexError(string function, string file, int line, long index, long size, string message = "", bool fatal = false)
    {
        var err = $"{(fatal ? "FATAL: " : "")} + Index {index} = {index} is out of bounds ({size} = {size}).";

        ErrPrintError(function, file, line, err, message, default, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool ERR_FAIL_COND(
        bool                                                 condition,
        string                                               message    = "",
        [CallerArgumentExpression(nameof(condition))] string expression = "",
        [CallerMemberName]                            string caller     = "",
        [CallerFilePath]                              string file       = "",
        [CallerLineNumber]                            int    line       = 0
    )
    {
        if (condition)
        {
            ErrPrintError(caller, file, line, $"Condition '{expression}' is true.", message, default, default);

            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static T ERR_FAIL_V<T>(
        T                         value,
        string                    message = "",
        [CallerMemberName] string caller  = "",
        [CallerFilePath]   string file    = "",
        [CallerLineNumber] int    line    = 0
    )
    {
        ErrPrintError(caller, file, line, $"Method/function failed. Returning: {value}", message, default, default);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void ASSERT_NOT_NULL<T>([NotNull] T? value)
    {
        if (value == null)
        {
            throw new NullReferenceException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_BREAK(
        bool                                                 condition,
        [CallerArgumentExpression(nameof(condition))] string expression = "",
        [CallerMemberName]                            string caller     = "",
        [CallerFilePath]                              string file       = "",
        [CallerLineNumber]                            int    line       = 0
    )
    {
        if (condition)
        {
            ErrPrintError(caller, file, line, $"Condition \"{expression}\" is true. Breaking.", default, default);
        }

        return condition;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void CRASH_COND_MSG(
        bool                                                 condition,
        string                                               message,
        [CallerArgumentExpression(nameof(condition))] string expression = "",
        [CallerMemberName]                            string caller     = "",
        [CallerFilePath]                              string file       = "",
        [CallerLineNumber]                            int    line       = 0
    )
    {
        if (condition)
        {
            ErrPrintError(caller, file, line, "FATAL: Condition \"" + expression + "\" is true.", message, default, default);
            throw new Exception("Program terminated abruptly");
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_CONTINUE(
        bool                                                 condition,
        [CallerArgumentExpression(nameof(condition))] string expression = "",
        [CallerMemberName]                            string caller     = "",
        [CallerFilePath]                              string file       = "",
        [CallerLineNumber]                            int    line       = 0
    )
    {
        if (condition)
        {
            ErrPrintError(caller, file, line, $"Condition '{expression}' is true. Continuing.", default, default);
        }

        return condition;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_CONTINUE_MSG(
        bool                                                 condition,
        string                                               message,
        [CallerArgumentExpression(nameof(condition))] string expression = "",
        [CallerMemberName]                            string caller     = "",
        [CallerFilePath]                              string file       = "",
        [CallerLineNumber]                            int    line       = 0
    )
    {
        if (condition)
        {
            ErrPrintError(caller, file, line, $"Condition '{expression}' is true. Continuing.", message, default, default);
        }

        return condition;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void ERR_FAIL(
        [CallerMemberName] string caller  = "",
        [CallerFilePath]   string file    = "",
        [CallerLineNumber] int    line    = 0
    ) => ErrPrintError(caller, file, line, $"Method/function failed.", "", default, default);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_COND(
        bool                                                 condition,
        [CallerArgumentExpression(nameof(condition))] string expression = "",
        [CallerMemberName]                            string caller     = "",
        [CallerFilePath]                              string file       = "",
        [CallerLineNumber]                            int    line       = 0
    ) => ERR_FAIL_COND(condition, "", expression, caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_COND_MSG(
        bool                                                 condition,
        string                                               message,
        [CallerArgumentExpression(nameof(condition))] string expression = "",
        [CallerMemberName]                            string caller     = "",
        [CallerFilePath]                              string file       = "",
        [CallerLineNumber]                            int    line       = 0
    ) => ERR_FAIL_COND(condition, message, expression, caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_COND_V(
        bool                                                 condition,
        [CallerArgumentExpression(nameof(condition))] string expression = "",
        [CallerMemberName]                            string caller     = "",
        [CallerFilePath]                              string file       = "",
        [CallerLineNumber]                            int    line       = 0
    ) => ERR_FAIL_COND(condition, "", expression, caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_COND_EDMSG(
        bool                                                 condition,
        string                                               message,
        [CallerArgumentExpression(nameof(condition))] string expression = "",
        [CallerMemberName]                            string caller     = "",
        [CallerFilePath]                              string file       = "",
        [CallerLineNumber]                            int    line       = 0
    )
    {
        if (condition)
        {
            ErrPrintError(caller, file, line, $"Condition '{expression}' is true.", message, true, default);
        }

        return condition;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_COND_V_MSG(
        bool                                                 condition,
        string                                               message,
        [CallerArgumentExpression(nameof(condition))] string expression = "",
        [CallerMemberName]                            string caller     = "",
        [CallerFilePath]                              string file       = "",
        [CallerLineNumber]                            int    line       = 0
    ) => ERR_FAIL_COND(condition, message, expression, caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_INDEX(
        int                       index,
        int                       size,
        [CallerMemberName] string caller = "",
        [CallerFilePath]   string file   = "",
        [CallerLineNumber] int    line   = 0
    ) => ERR_FAIL_INDEX_MSG(index, size, "", caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_INDEX<T>(
        T                         index,
        T                         size,
        [CallerMemberName] string caller = "",
        [CallerFilePath]   string file   = "",
        [CallerLineNumber] int    line   = 0
    ) where T : Enum => ERR_FAIL_INDEX((int)(object)index, (int)(object)size, caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_INDEX_MSG(
        int                       index,
        int                       size,
        string                    message,
        [CallerMemberName] string caller = "",
        [CallerFilePath]   string file   = "",
        [CallerLineNumber] int    line   = 0
    )
    {
        if (index < 0 || index >= size)
        {
            ErrPrintIndexError(caller, file, line, index, size, message);

            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_INDEX_MSG<T>(
        T                         index,
        T                         size,
        string                    message,
        [CallerMemberName] string caller = "",
        [CallerFilePath]   string file   = "",
        [CallerLineNumber] int    line   = 0
    ) where T : Enum => ERR_FAIL_INDEX_MSG((int)(object)index, (int)(object)size, message, caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_INDEX_V(
        int                       index,
        int                       size,
        [CallerMemberName] string caller = "",
        [CallerFilePath]   string file   = "",
        [CallerLineNumber] int    line   = 0
    ) => ERR_FAIL_INDEX(index, size, caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_INDEX_V<T>(
        T                         index,
        T                         size,
        [CallerMemberName] string caller = "",
        [CallerFilePath]   string file   = "",
        [CallerLineNumber] int    line   = 0
    ) where T : Enum => ERR_FAIL_INDEX(index, size, caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_MSG(
        string                    message,
        [CallerMemberName] string caller = "",
        [CallerFilePath]   string file   = "",
        [CallerLineNumber] int    line   = 0
    )
    {
        ErrPrintError(caller, file, line, "Method/function failed.", message, default, default);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_NULL<T>(
        [NotNullWhen(false)] T?     value,
        [CallerMemberName]   string caller = "",
        [CallerFilePath]     string file   = "",
        [CallerLineNumber]   int    line   = 0
    )
    {
        if (value == null)
        {
            ErrPrintError(caller, file, line, $"Parameter \"{value}\" is null.", default, default);

            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_NULL_V(
        object? value,
        [CallerMemberName] string caller = "",
        [CallerFilePath]   string file   = "",
        [CallerLineNumber] int    line   = 0
    ) => ERR_FAIL_NULL(value, caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T ERR_FAIL_V<T>(
        T                         value,
        [CallerMemberName] string caller = "",
        [CallerFilePath]   string file   = "",
        [CallerLineNumber] int    line   = 0
    ) => ERR_FAIL_V(value, "", caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T ERR_FAIL_V_MSG<T>(
        T                         value,
        string                    message,
        [CallerMemberName] string caller     = "",
        [CallerFilePath]   string file       = "",
        [CallerLineNumber] int    line       = 0
    ) => ERR_FAIL_V(value, message, caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void ERR_PRINT(
        string                    message,
        [CallerMemberName] string caller     = "",
        [CallerFilePath]   string file       = "",
        [CallerLineNumber] int    line       = 0
    ) => ErrPrintError(caller, file, line, "", message, default, default);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void ERR_PRINT_ONCE(
        string                    message,
        ref bool                  firstPrint,
        [CallerMemberName] string caller     = "",
        [CallerFilePath]   string file       = "",
        [CallerLineNumber] int    line       = 0
    )
    {
        if (firstPrint)
        {
            ErrPrintError(caller, file, line, "", message, default, default);

            firstPrint = false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void WARN_PRINT(
        string                    message,
        [CallerMemberName] string caller = "",
        [CallerFilePath]   string file   = "",
        [CallerLineNumber] int    line   = 0
    ) => ErrPrintError(caller, file, line, message, false, ErrorHandlerType.ERR_HANDLER_WARNING);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void WARN_PRINT_ONCE(
        string                    message,
        ref bool                  firstPrint,
        [CallerMemberName] string caller     = "",
        [CallerFilePath]   string file       = "",
        [CallerLineNumber] int    line       = 0
    )
    {
        if (firstPrint)
        {
            ErrPrintError(caller, file, line, message, false, ErrorHandlerType.ERR_HANDLER_WARNING);

            firstPrint = false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void WARN_PRINT_ONCE_ED(
        string                    message,
        ref bool                  firstPrint,
        [CallerMemberName] string caller     = "",
        [CallerFilePath]   string file       = "",
        [CallerLineNumber] int    line       = 0
    )
    {
        if (firstPrint)
        {
            ErrPrintError(caller, file, line, "", message, default, ErrorHandlerType.ERR_HANDLER_WARNING);

            firstPrint = false;
        }
    }

    public static void AddErrorHandler(ErrorHandlerList item)
    {
        lock(locker)
        {
            if (errorHandlerList.Contains(item))
            {
                errorHandlerList.Remove(item);
            }

            errorHandlerList.AddLast(item);
        }
    }

    public static void ErrPrintError(string function, string file, int line, string error, bool editorNotify, ErrorHandlerType type) =>
        ErrPrintError(function, file, line, error, "", editorNotify, type);

    public static void ErrPrintError(string function, string file, int line, string error, string message, bool editorNotify, ErrorHandlerType type)
    {
        if (OS.IsInitialized)
        {
            OS.Singleton.PrintError(function, file, line, error, message, editorNotify, (Logger.ErrorType)type);
        }
        else
        {
            var details = !string.IsNullOrEmpty(message) ? message : error;

            Console.WriteLine($"ERROR: {details}\n   at: {function} ({file}:{line})\n");
        }

        lock(locker)
        {
            foreach (var item in errorHandlerList)
            {
                item.ErrorHandler.Invoke(item.UserData, function, file, line, error, message, editorNotify, type);
            }
        }
    }
}

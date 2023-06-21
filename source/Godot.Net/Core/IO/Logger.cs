namespace Godot.Net.Core.IO;

#pragma warning disable IDE0052 // TODO - Remove

public abstract class Logger
{
    public enum ErrorType
    {
        ERR_ERROR,
        ERR_WARNING,
        ERR_SCRIPT,
        ERR_SHADER
    };

    private static void ERR_PRINT(string message) => throw new NotImplementedException();

    protected static bool FlushStdoutOnPrint { get; set; } = true;

    protected static bool ShouldLog(bool error) =>
        (!error || CoreGlobals.PrintErrorEnabled) && (error || CoreGlobals.PrintLineEnabled);

    public abstract void Logv(string message, bool error);

    public virtual void LogError(string function, string file, int line, string code, string rationale, bool editorNotify, ErrorType type)
    {
        if (ShouldLog(true))
        {
            var errType = "ERROR";
            switch (type)
            {
                case ErrorType.ERR_ERROR:
                    errType = "ERROR";
                    break;
                case ErrorType.ERR_WARNING:
                    errType = "WARNING";
                    break;
                case ErrorType.ERR_SCRIPT:
                    errType = "SCRIPT ERROR";
                    break;
                case ErrorType.ERR_SHADER:
                    errType = "SHADER ERROR";
                    break;
                default:
                    ERR_PRINT("Unknown error type");
                    break;
            }

            var errDetails = string.IsNullOrEmpty(rationale)
                ? rationale
                : code;

            if (editorNotify)
            {
                this.LogfError($"{errType}: {errDetails}\n");
            }
            else
            {
                this.LogfError($"USER {errType}: {errDetails}\n");
            }

            this.LogfError($"   at: {function} ({file}:{line})\n");
        }
    }

    public virtual void LogfError(string message)
    {
        if (ShouldLog(true))
        {
            this.Logv(message, true);
        }
    }
}

public class CompositeLogger : Logger
{
    private readonly List<Logger> loggers;

    public CompositeLogger(List<Logger> loggers) => this.loggers = loggers;
    public override void Logv(string message, bool error) => throw new NotImplementedException();
}

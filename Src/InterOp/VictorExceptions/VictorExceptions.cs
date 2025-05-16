using System;
using VictorBaseDotNET.Src.utils;

namespace VictorExceptions;
public class VictorException : Exception
{
    public ErrorCode Code { get; }

    public VictorException(ErrorCode code) : base() => Code = code;
    public VictorException(string message) : base(message) { }
    public VictorException(string message,ErrorCode code) : base(message) => Code = code;

}

using System;
using VictorBaseDotNET.Src.utils;

namespace VictorExceptions;
public class VictorException : Exception
{
    protected ErrorCode Code { get; }

    public VictorException(ErrorCode code) : base() => Code = code;
    public VictorException(string message) : base(message) { }
    public VictorException(ErrorCode code, string message) : base(message) => Code = code;

}

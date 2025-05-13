using System;
using VictorBaseDotNET.Src.utils;

namespace VictorExceptions;
public class VictorException : Exception
{
    public ErrorCode Code { get; }

    public VictorException(ErrorCode code, string message) : base(message)
    {
        Code = code;
    }
}

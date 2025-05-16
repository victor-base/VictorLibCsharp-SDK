
using System;
using System.Collections.Generic;
using VictorBaseDotNET.Src.utils;

namespace VictorSnapshots;
[Serializable]
public class VictorIndexSnapshot
{
    public ushort Dimensions { get; set; }
    public IndexType IndexType { get; set; }
    public DistanceMethod Method { get; set; }
    public List<VectorEntry> Vectors { get; set; } = new();
}

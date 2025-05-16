using System;

[Serializable]
public class VectorEntry
{
    public ulong Id { get; set; }
    public float[] Vector { get; set; } = [];
}

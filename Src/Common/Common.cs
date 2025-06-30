using System.Runtime.InteropServices;
namespace Src.Common
{
    /// <summary>
    /// Estructura que representa un resultado de búsqueda
    /// </summary>
   
    [StructLayout(LayoutKind.Sequential)]
    
    public struct MatchResult
    {
        public int Id;
        public float Distance;
    }

}
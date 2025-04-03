// Copyright (C) 2025 Iván E. Rodríguez. Contact: ivanrwcm25@gmail.com
//
// This file is part of the Victor Base binding for .NET.
// The original Victor Base core project was created by Emiliano Billi (email: emiliano.billi@gmail.com).
//
// This program is free software: you can redistribute it and/or modify  
// it under the terms of the GNU General Public License as published  
// by the Free Software Foundation, either version 3 of the License,  
// or (at your option) any later version.  
//
// This program is distributed in the hope that it will be useful,  
// but WITHOUT ANY WARRANTY; without even the implied warranty of  
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the  
// GNU General Public License for more details.  
//
// You should have received a copy of the GNU General Public License  
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Runtime.InteropServices;
namespace Src.Common
{
    /// <summary>
    /// Estructura que representa un resultado de búsqueda. Mapea con la struct del core.
    /// </summary>
   
    [StructLayout(LayoutKind.Sequential)]
    
    public struct MatchResult
    {
        public int Id;
        public float Distance;
    }

}
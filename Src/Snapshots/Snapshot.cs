/*
 * Victor Base SDK - Developed by Iván E. Rodriguez
 * Based on the vector database core created by Emiliano A. Billi.
 * 
 * Copyright (C) 2025 Iván E. Rodriguez
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 *
 */
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

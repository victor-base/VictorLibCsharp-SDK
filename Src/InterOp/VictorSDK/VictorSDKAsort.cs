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
using VictorBaseDotNET.Src.Common;
using VictorBaseDotNET.Src.utils;
using VictorExceptions;

namespace Victor;

public partial class VictorSDK : IDisposable
{
    /// <summary>
    /// Inicializa una nueva estructura ASort con un heap de tamaño <paramref name="n"/> y método de comparación <paramref name="method"/>.
    /// </summary>
    /// <param name="n">Tamaño del heap interno (cantidad máxima de resultados a mantener).</param>
    /// <param name="method">Método de comparación (ver documentación nativa para opciones).</param>
    /// <returns>Una instancia de <see cref="Asort"/> inicializada.</returns>
    public PublicAsort InitAsort(int n, int method)
    {

        Asort asortInstance = new();
        int result = _native.init_asort(ref asortInstance, n, method);
        if (result != 0) throw new Exception($"init_asort failed with code {result}");

        return StructMapper.MapToPublic(StructMapper.MapFromInterop(asortInstance));
    }

    /// <summary>
    /// Inserta múltiples resultados de coincidencia en la estructura ASort.
    /// Reemplaza elementos si encuentra mejores matches.
    /// </summary>
    /// <param name="asort">Referencia a la instancia ASort.</param>
    /// <param name="inputs">Arreglo de resultados a insertar.</param>
    public void UpdateAsort(ref PublicAsort asort, MatchResult[] inputs)
    {
        if (inputs == null || inputs.Length == 0) throw new VictorException("inputs must be non-empty");

        var internalAsort = StructMapper.MapToInterop(StructMapper.MapToInternal(asort));
        var internalInputs = new InternalMatchResult[inputs.Length];

        for (int i = 0; i < inputs.Length; i++)
        {
            internalInputs[i] = new InternalMatchResult
            {
                Distance = inputs[i].Distance,
                Label = inputs[i].Label // 👈 asegúrate que MatchResult tiene Label, no Id
            };
        }

        int result = _native.as_update(ref internalAsort, internalInputs, inputs.Length);

        if (result != 0) throw new Exception($"as_update failed with code {result}");

        asort = StructMapper.MapToPublic(StructMapper.MapFromInterop(internalAsort));
    }





    /// <summary>
    /// Finaliza el contexto de ASort. Si se provee un arreglo de salida, extrae los mejores resultados en orden aproximado.
    /// Si <paramref name="extractResults"/> es falso, solo libera memoria.
    /// </summary>
    /// <param name="asort">Referencia al contexto ASort.</param>
    /// <param name="n">Máximo número de resultados a extraer.</param>
    /// <param name="extractResults">Si es verdadero, extrae resultados. Si es falso, solo libera recursos.</param>
    /// <returns>Arreglo con los mejores resultados, o arreglo vacío si solo se liberó memoria.</returns>
    public MatchResult[] CloseAsort(ref PublicAsort asort, int n, bool extractResults = true)
    {
        Asort internalAsort = StructMapper.MapToInterop(StructMapper.MapToInternal(asort));

        InternalMatchResult[] internalOutputs = extractResults ? new InternalMatchResult[n] : null;
        int result = _native.as_close(ref internalAsort, internalOutputs, n);

        if (result < 0) throw new VictorException($"as_close failed with code {result}");

        asort = StructMapper.MapToPublic(StructMapper.MapFromInterop(internalAsort)); // ✅ simplificado

        if (!extractResults || result == 0) return [];

        MatchResult[] outputs = new MatchResult[result];

        for (int i = 0; i < result; i++)
        {
            outputs[i] = new MatchResult
            {
                Distance = internalOutputs[i].Distance,
                Label = internalOutputs[i].Label
            };
        }

        return outputs;
    }


}
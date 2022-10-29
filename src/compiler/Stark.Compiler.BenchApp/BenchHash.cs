// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using BenchmarkDotNet.Attributes;
using Stark.Compiler.Helpers;

namespace Stark.Compiler.BenchApp;

/// <summary>
/// Small benchmark to decide until which size I can use FNV-1A vs System.HashCode.
///
/// Verdict: If the size is lower than 28, FNV-1A is better
///
/// BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1098/21H2)
/// AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
/// .NET SDK=7.0.100-rc.1.22431.12
///   [Host]     : .NET 7.0.0 (7.0.22.42610), X64 RyuJIT AVX2
///   DefaultJob : .NET 7.0.0 (7.0.22.42610), X64 RyuJIT AVX2
/// 
/// |     Method |     data |      Mean |    Error |   StdDev |
/// |----------- |----------|----------:|---------:|---------:|
/// |   HashCode | Byte[16] | 6.808 ns | 0.0465 ns | 0.0412 ns |
/// |   HashCode | Byte[17] | 7.148 ns | 0.0106 ns | 0.0094 ns |
/// |   HashCode | Byte[18] | 8.608 ns | 0.0282 ns | 0.0264 ns |
/// |   HashCode | Byte[19] | 9.855 ns | 0.0314 ns | 0.0294 ns |
/// |   HashCode | Byte[20] | 7.133 ns | 0.0273 ns | 0.0256 ns |
/// |   HashCode | Byte[21] | 8.601 ns | 0.0136 ns | 0.0127 ns |
/// |   HashCode | Byte[22] | 9.961 ns | 0.0292 ns | 0.0273 ns |
/// |   HashCode | Byte[23] | 13.877 ns | 0.1031 ns | 0.0964 ns |
/// |   HashCode | Byte[24] |  8.698 ns | 0.0287 ns | 0.0255 ns |
/// |   HashCode | Byte[25] | 10.795 ns | 0.0276 ns | 0.0259 ns |
/// |   HashCode | Byte[26] | 13.96 ns | 0.051 ns | 0.045 ns |
/// |   HashCode | Byte[27] | 14.87 ns | 0.041 ns | 0.038 ns |
/// |   HashCode | Byte[28] |  9.885 ns | 0.0236 ns | 0.0210 ns |
/// |   HashCode | Byte[29] | 13.954 ns | 0.0289 ns | 0.0270 ns |
/// |   HashCode | Byte[30] | 14.974 ns | 0.0755 ns | 0.0707 ns |
/// |   HashCode | Byte[31] | 15.703 ns | 0.0645 ns | 0.0572 ns |
/// |   HashCode | Byte[32] |  9.337 ns | 0.0337 ns | 0.0315 ns |
/// |   HashCode | Byte[33] | 12.929 ns | 0.0977 ns | 0.0914 ns |
/// | HashFNV1A | Byte[16] | 6.542 ns | 0.0260 ns | 0.0243 ns |
/// | HashFNV1A | Byte[17] | 6.923 ns | 0.0141 ns | 0.0131 ns |
/// | HashFNV1A | Byte[18] | 7.264 ns | 0.0210 ns | 0.0197 ns |
/// | HashFNV1A | Byte[19] | 7.707 ns | 0.0208 ns | 0.0194 ns |
/// | HashFNV1A | Byte[20] | 9.034 ns | 0.0209 ns | 0.0196 ns |
/// | HashFNV1A | Byte[21] | 8.679 ns | 0.0223 ns | 0.0209 ns |
/// | HashFNV1A | Byte[22] | 9.279 ns | 0.0218 ns | 0.0193 ns |
/// | HashFNV1A | Byte[23] | 9.876 ns | 0.0563 ns | 0.0499 ns |
/// | HashFNV1A | Byte[24] | 10.428 ns | 0.0357 ns | 0.0334 ns |
/// | HashFNV1A | Byte[25] | 11.074 ns | 0.0385 ns | 0.0360 ns |
/// | HashFNV1A | Byte[26] | 11.90 ns | 0.022 ns | 0.021 ns |
/// | HashFNV1A | Byte[27] | 12.79 ns | 0.020 ns | 0.016 ns |
/// | HashFNV1A | Byte[28] | 13.585 ns | 0.0348 ns | 0.0325 ns |
/// | HashFNV1A | Byte[29] | 14.412 ns | 0.0326 ns | 0.0305 ns |
/// | HashFNV1A | Byte[30] | 16.614 ns | 0.0682 ns | 0.0638 ns |
/// | HashFNV1A | Byte[31] | 15.993 ns | 0.0677 ns | 0.0600 ns |
/// | HashFNV1A | Byte[32] | 16.388 ns | 0.0562 ns | 0.0498 ns |
/// | HashFNV1A | Byte[33] | 16.879 ns | 0.0397 ns | 0.0371 ns |
/// </summary>
public class BenchHash
{
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int HashCode(byte[] data)
    {
        var hash = new HashCode();
        hash.AddBytes(data);
        return hash.ToHashCode();
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int HashFNV1A(byte[] data)
    {
        return HashHelper.Hash(data);
    }

    public IEnumerable<byte[]> Data()
    {
        //for (int i = 1; i < 36; i++)
        //{
        //    yield return Enumerable.Range(0, i).Select(x => (byte)x).ToArray();
        //}
        yield return Enumerable.Range(0, 64).Select(x => (byte)x).ToArray();
        yield return Enumerable.Range(0, 80).Select(x => (byte)x).ToArray();
        yield return Enumerable.Range(0, 96).Select(x => (byte)x).ToArray();
        //yield return Enumerable.Range(0, 64).Select(x => (byte)x).ToArray();
    }
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using StarkPlatform.Reflection.Internal;

namespace StarkPlatform.Reflection.Metadata.Ecma335
{
    internal readonly struct UserStringHeap
    {
        internal readonly MemoryBlock Block;

        public UserStringHeap(MemoryBlock block)
        {
            this.Block = block;
        }

        internal string GetString(UserStringHandle handle)
        {
            int offset, size;
            if (!Block.PeekHeapValueOffsetAndSize(handle.GetHeapOffset(), out offset, out size))
            {
                return string.Empty;
            }

            // Spec: Furthermore, there is an additional terminal byte (so all byte counts are odd, not even). 
            // The size in the blob header is the length of the string in bytes + 1.
            return Block.PeekUtf8(offset, size - 1);
        }

        internal unsafe BlobReader GetStringAsBlobReader(UserStringHandle handle)
        {

            int offset, size;
            var result = Block.PeekHeapValueOffsetAndSize(handle.GetHeapOffset(), out offset, out size);
            Debug.Assert(result);
            return new BlobReader(Block.Pointer + offset, size - 1);
        }

        internal UserStringHandle GetNextHandle(UserStringHandle handle)
        {
            int offset, size;
            if (!Block.PeekHeapValueOffsetAndSize(handle.GetHeapOffset(), out offset, out size))
            {
                return default(UserStringHandle);
            }

            int nextIndex = offset + size;
            if (nextIndex >= Block.Length)
            {
                return default(UserStringHandle);
            }

            return UserStringHandle.FromOffset(nextIndex);
        }
    }
}

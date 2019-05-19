// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace StarkPlatform.Reflection.Metadata
{
    public readonly struct TypeLayout
    {
        private readonly int _size;
        private readonly int _packingSize;
        private readonly int _alignment;

        public TypeLayout(int size, int packingSize, int alignment)
        {
            _size = size;
            _packingSize = packingSize;
            _alignment = alignment;
        }

        public int Size
        {
            get { return _size; }
        }

        public int PackingSize
        {
            get { return _packingSize; }
        }

        public int Alignment
        {
            get { return _alignment; }
        }

        public bool IsDefault
        {
            get { return _size == 0 && _packingSize == 0 && _alignment == 0; }
        }
    }
}

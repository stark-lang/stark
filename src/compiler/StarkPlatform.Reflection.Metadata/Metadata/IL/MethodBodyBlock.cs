using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using StarkPlatform.Reflection.Metadata.Ecma335;
using StarkPlatform.Reflection.Resources;

namespace StarkPlatform.Reflection.Metadata
{
    public struct MethodBodyBlock
    {
        private readonly IntPtr _il;
        private readonly int _ilSize;
        private readonly ushort _maxStack;
        private readonly bool _isFatException;
        private readonly int _exceptionHandlerCount;
        private readonly int _offsetExceptionHandlers;

        private unsafe MethodBodyBlock(
            bool localVariablesInitialized,
            ushort maxStack,
            StandaloneSignatureHandle localSignatureHandle,
            IntPtr il,
            int ilSize,
            bool isFatException,
            int exceptionHandlerCount,
            int offsetExceptionHandlers)
        {
            LocalVariablesInitialized = localVariablesInitialized;
            _maxStack = maxStack;
            LocalSignature = localSignatureHandle;
            _il = il;
            _ilSize = ilSize;
            _isFatException = isFatException;
            _exceptionHandlerCount = exceptionHandlerCount;
            _offsetExceptionHandlers = offsetExceptionHandlers;
        }

        public int MaxStack => _maxStack;

        public bool LocalVariablesInitialized { get; }

        public StandaloneSignatureHandle LocalSignature { get; }

        public int ExceptionHandlerCount => _exceptionHandlerCount;

        public bool HasException => ExceptionHandlerCount > 0;

        public unsafe ExceptionRegion GetExceptionHandler(int index)
        {
            if (((uint)index) > (uint)_exceptionHandlerCount)
            {
                Throw.OutOfBounds();
            }

            if (_isFatException)
            {
                return *(ExceptionRegion*)((byte*)_il + _offsetExceptionHandlers);
            }

            ref var smallExceptionView = ref *(SmallExceptionView*)((byte*) _il + _offsetExceptionHandlers);
            return new ExceptionRegion(smallExceptionView.ExceptionRegionkind, smallExceptionView.TryOffset, smallExceptionView.TryLength, smallExceptionView.HandlerOffset, smallExceptionView.HandlerLength, smallExceptionView.ClassTokenOrFilterOffset);
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        readonly struct SmallExceptionView
        {
            public readonly ExceptionRegionKind ExceptionRegionkind;
            public readonly ushort TryOffset;
            public readonly byte TryLength;
            public readonly ushort HandlerOffset;
            public readonly byte HandlerLength;
            public readonly int ClassTokenOrFilterOffset;
        }

        public BlobReader GetILReader()
        {
            unsafe
            {
                return new BlobReader((byte*) _il, _ilSize);
            }
        }

        //public ILInstructionReader GetILInstructionReader()
        //{
        //    return new ILInstructionReader(GetILReader());
        //}

        public static MethodBodyBlock Create(BlobReader reader)
        {
            const byte ILTinyFormat = 0x02;
            const byte ILFatFormat = 0x03;
            const byte ILFormatMask = 0x03;
            const int ILTinyFormatSizeShift = 2;
            const byte ILMoreSects = 0x08;
            const byte ILInitLocals = 0x10;
            const byte ILFatFormatHeaderSize = 0x03;
            const int ILFatFormatHeaderSizeShift = 4;
            const byte SectEHTable = 0x01;
            const byte SectOptILTable = 0x02;
            const byte SectFatFormat = 0x40;
            const byte SectMoreSects = 0x40;

            int startOffset = reader.Offset;
            int ilSize;

            // Error need to check if the Memory Block is empty. This is false for all the calls...
            byte headByte = reader.ReadByte();
            if ((headByte & ILFormatMask) == ILTinyFormat)
            {
                // tiny IL can't have locals so technically this shouldn't matter, 
                // but false is consistent with other metadata readers and helps
                // for use cases involving comparing our output with theirs.
                const bool initLocalsForTinyIL = false;

                ilSize = headByte >> ILTinyFormatSizeShift;
                unsafe
                {
                    return new MethodBodyBlock(
                        initLocalsForTinyIL,
                        8,
                        default(StandaloneSignatureHandle),
                        new IntPtr(reader.CurrentPointer),
                        ilSize,
                        false,
                        0,
                        0
                    );
                }
            }

            if ((headByte & ILFormatMask) != ILFatFormat)
            {
                throw new BadImageFormatException(StarkPlatform.Reflection.Resources.SR.Format(SR.InvalidMethodHeader1, headByte));
            }

            // FatILFormat
            byte headByte2 = reader.ReadByte();
            if ((headByte2 >> ILFatFormatHeaderSizeShift) != ILFatFormatHeaderSize)
            {
                throw new BadImageFormatException(StarkPlatform.Reflection.Resources.SR.Format(SR.InvalidMethodHeader2, headByte, headByte2));
            }

            bool localsInitialized = (headByte & ILInitLocals) == ILInitLocals;
            bool hasExceptionHandlers = (headByte & ILMoreSects) == ILMoreSects;

            ushort maxStack = reader.ReadUInt16();
            ilSize = reader.ReadInt32();

            int localSignatureToken = reader.ReadInt32();
            StandaloneSignatureHandle localSignatureHandle;
            if (localSignatureToken == 0)
            {
                localSignatureHandle = default(StandaloneSignatureHandle);
            }
            else if ((localSignatureToken & TokenTypeIds.TypeMask) == TokenTypeIds.Signature)
            {
                localSignatureHandle = StandaloneSignatureHandle.FromRowId((int)((uint)localSignatureToken & TokenTypeIds.RIDMask));
            }
            else
            {
                throw new BadImageFormatException(StarkPlatform.Reflection.Resources.SR.Format(SR.InvalidLocalSignatureToken, unchecked((uint)localSignatureToken)));
            }

            var startIlBlockOffset = reader.Offset;
            IntPtr ilPtr;
            unsafe
            {
                ilPtr = new IntPtr(reader.CurrentPointer);
            }
            reader.Offset += ilSize;

            ImmutableArray<ExceptionRegion> exceptionHandlers;
            int countExceptionHandler = 0;
            int offsetExceptionHandler = 0;
            bool isFatExceptionHandler = false;
            if (hasExceptionHandlers)
            {
                reader.Align(4);
                byte sehHeader = reader.ReadByte();
                if ((sehHeader & SectEHTable) != SectEHTable)
                {
                    throw new BadImageFormatException(StarkPlatform.Reflection.Resources.SR.Format(SR.InvalidSehHeader, sehHeader));
                }

                bool sehFatFormat = (sehHeader & SectFatFormat) == SectFatFormat;
                int dataSize = reader.ReadByte();
                if (sehFatFormat)
                {
                    dataSize += reader.ReadUInt16() << 8;
                    countExceptionHandler = dataSize / 24;
                    isFatExceptionHandler = true;
                }
                else
                {
                    reader.Offset += 2; // skip over reserved field
                    countExceptionHandler = dataSize / 12;
                }
                offsetExceptionHandler = reader.Offset - startIlBlockOffset;
            }

            return new MethodBodyBlock(
                localsInitialized,
                maxStack,
                localSignatureHandle,
                ilPtr,
                ilSize,
                isFatExceptionHandler,
                offsetExceptionHandler,
                countExceptionHandler);
        }
    }
}
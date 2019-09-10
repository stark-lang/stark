using System.Runtime.InteropServices;
using System.Text;

namespace StarkPlatform.Reflection.Metadata
{
    /// <summary>
    /// An IL Instruction
    /// </summary>
    /// <remarks>
    /// This struct is packed to 16 bytes to keep it efficient.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    public struct ILInstruction
    {
        /// <summary>
        /// IL Offset of the instruction from the start of the IL <see cref="BlobReader"/>
        /// </summary>
        [FieldOffset(0)]
        public int Offset;

        /// <summary>
        /// The <see cref="ILOpCode"/> of this instruction
        /// </summary>
        [FieldOffset(4)]
        public ILOpCode OpCode;

        /// <summary>
        /// The Operand as a byte
        /// </summary>
        [FieldOffset(8)]
        public byte Operand8;

        /// <summary>
        /// The Operand as a short
        /// </summary>
        [FieldOffset(8)]
        public short Operand16;

        /// <summary>
        /// The Operand as a int
        /// </summary>
        [FieldOffset(8)]
        public int Operand32;

        /// <summary>
        /// The Operand as a ulong
        /// </summary>
        [FieldOffset(8)]
        public ulong Operand64;

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("0x");
            builder.Append(Offset.ToString("x8"));
            builder.Append(' ');
            var opCodeStr = OpCode.ToString();
            builder.Append(opCodeStr);
            builder.Append(' ', 14 - opCodeStr.Length);

            switch (OpCode.GetOperandSize())
            {
                case 1:
                    builder.Append(" 0x");
                    builder.Append(Operand8.ToString("x2"));
                    break;
                case 2:
                    builder.Append(" 0x");
                    builder.Append(Operand8.ToString("x4"));
                    break;
                case 4:
                    builder.Append(" 0x");
                    builder.Append(Operand8.ToString("x8"));
                    break;
                case 8:
                    builder.Append(" 0x");
                    builder.Append(Operand8.ToString("x16"));
                    break;
            }

            return builder.ToString();
        }
    }
}
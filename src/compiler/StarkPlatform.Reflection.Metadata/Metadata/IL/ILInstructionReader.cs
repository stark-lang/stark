using System;
using System.Diagnostics;
using SR = StarkPlatform.Reflection.Resources.SR;

namespace StarkPlatform.Reflection.Metadata
{
    /// <summary>
    /// IL Instruction reader.
    /// </summary>
    public struct ILInstructionReader
    {
        private BlobReader _ilReader;

        /// <summary>
        /// Create a new instance of this reader from a IL <see cref="BlobReader"/>
        /// </summary>
        /// <param name="ilReader">An IL <see cref="BlobReader"/></param>
        public ILInstructionReader(BlobReader ilReader)
        {
            _ilReader = ilReader;
        }

        /// <summary>
        /// Gets a boolean indicating whether there are instructions left
        /// </summary>
        public bool HasNext => _ilReader.RemainingBytes != 0;

        /// <summary>
        /// Reads the next ILInstruction. Must have HasNext
        /// </summary>
        /// <param name="instruction">Output the next ILInstruction</param>
        public void Read(ref ILInstruction instruction)
        {
            Debug.Assert(HasNext);
            instruction.Offset = _ilReader.Offset;
            var byte1 = _ilReader.ReadByte();
            var opCode = byte1 >= 0xfe ? (ILOpCode) ((byte1 << 8) | _ilReader.ReadByte()) : (ILOpCode) byte1;
            instruction.OpCode = opCode;
            switch (opCode)
            {
                case ILOpCode.Nop:
                case ILOpCode.Break:
                case ILOpCode.Ldarg_0:
                case ILOpCode.Ldarg_1:
                case ILOpCode.Ldarg_2:
                case ILOpCode.Ldarg_3:
                case ILOpCode.Ldloc_0:
                case ILOpCode.Ldloc_1:
                case ILOpCode.Ldloc_2:
                case ILOpCode.Ldloc_3:
                case ILOpCode.Stloc_0:
                case ILOpCode.Stloc_1:
                case ILOpCode.Stloc_2:
                case ILOpCode.Stloc_3:
                case ILOpCode.Ldnull:
                case ILOpCode.Ldc_i4_m1:
                case ILOpCode.Ldc_i4_0:
                case ILOpCode.Ldc_i4_1:
                case ILOpCode.Ldc_i4_2:
                case ILOpCode.Ldc_i4_3:
                case ILOpCode.Ldc_i4_4:
                case ILOpCode.Ldc_i4_5:
                case ILOpCode.Ldc_i4_6:
                case ILOpCode.Ldc_i4_7:
                case ILOpCode.Ldc_i4_8:
                case ILOpCode.Dup:
                case ILOpCode.Pop:
                case ILOpCode.Ret:
                case ILOpCode.Ldind_i1:
                case ILOpCode.Ldind_u1:
                case ILOpCode.Ldind_i2:
                case ILOpCode.Ldind_u2:
                case ILOpCode.Ldind_i4:
                case ILOpCode.Ldind_u4:
                case ILOpCode.Ldind_i8:
                case ILOpCode.Ldind_i:
                case ILOpCode.Ldind_r4:
                case ILOpCode.Ldind_r8:
                case ILOpCode.Ldind_ref:
                case ILOpCode.Stind_ref:
                case ILOpCode.Stind_i1:
                case ILOpCode.Stind_i2:
                case ILOpCode.Stind_i4:
                case ILOpCode.Stind_i8:
                case ILOpCode.Stind_r4:
                case ILOpCode.Stind_r8:
                case ILOpCode.Add:
                case ILOpCode.Sub:
                case ILOpCode.Mul:
                case ILOpCode.Div:
                case ILOpCode.Div_un:
                case ILOpCode.Rem:
                case ILOpCode.Rem_un:
                case ILOpCode.And:
                case ILOpCode.Or:
                case ILOpCode.Xor:
                case ILOpCode.Shl:
                case ILOpCode.Shr:
                case ILOpCode.Shr_un:
                case ILOpCode.Neg:
                case ILOpCode.Not:
                case ILOpCode.Conv_i1:
                case ILOpCode.Conv_i2:
                case ILOpCode.Conv_i4:
                case ILOpCode.Conv_i8:
                case ILOpCode.Conv_r4:
                case ILOpCode.Conv_r8:
                case ILOpCode.Conv_u4:
                case ILOpCode.Conv_u8:
                case ILOpCode.Conv_r_un:
                case ILOpCode.Throw:
                case ILOpCode.Conv_ovf_i1_un:
                case ILOpCode.Conv_ovf_i2_un:
                case ILOpCode.Conv_ovf_i4_un:
                case ILOpCode.Conv_ovf_i8_un:
                case ILOpCode.Conv_ovf_u1_un:
                case ILOpCode.Conv_ovf_u2_un:
                case ILOpCode.Conv_ovf_u4_un:
                case ILOpCode.Conv_ovf_u8_un:
                case ILOpCode.Conv_ovf_i_un:
                case ILOpCode.Conv_ovf_u_un:
                case ILOpCode.Ldlen:
                case ILOpCode.Ldelem_i1:
                case ILOpCode.Ldelem_u1:
                case ILOpCode.Ldelem_i2:
                case ILOpCode.Ldelem_u2:
                case ILOpCode.Ldelem_i4:
                case ILOpCode.Ldelem_u4:
                case ILOpCode.Ldelem_i8:
                case ILOpCode.Ldelem_i:
                case ILOpCode.Ldelem_r4:
                case ILOpCode.Ldelem_r8:
                case ILOpCode.Ldelem_ref:
                case ILOpCode.Stelem_i:
                case ILOpCode.Stelem_i1:
                case ILOpCode.Stelem_i2:
                case ILOpCode.Stelem_i4:
                case ILOpCode.Stelem_i8:
                case ILOpCode.Stelem_r4:
                case ILOpCode.Stelem_r8:
                case ILOpCode.Stelem_ref:
                case ILOpCode.Conv_ovf_i1:
                case ILOpCode.Conv_ovf_u1:
                case ILOpCode.Conv_ovf_i2:
                case ILOpCode.Conv_ovf_u2:
                case ILOpCode.Conv_ovf_i4:
                case ILOpCode.Conv_ovf_u4:
                case ILOpCode.Conv_ovf_i8:
                case ILOpCode.Conv_ovf_u8:
                case ILOpCode.Ckfinite:
                case ILOpCode.Conv_u2:
                case ILOpCode.Conv_u1:
                case ILOpCode.Conv_i:
                case ILOpCode.Conv_ovf_i:
                case ILOpCode.Conv_ovf_u:
                case ILOpCode.Add_ovf:
                case ILOpCode.Add_ovf_un:
                case ILOpCode.Mul_ovf:
                case ILOpCode.Mul_ovf_un:
                case ILOpCode.Sub_ovf:
                case ILOpCode.Sub_ovf_un:
                case ILOpCode.Endfinally:
                case ILOpCode.Stind_i:
                case ILOpCode.Conv_u:
                case ILOpCode.Arglist:
                case ILOpCode.Ceq:
                case ILOpCode.Cgt:
                case ILOpCode.Cgt_un:
                case ILOpCode.Clt:
                case ILOpCode.Clt_un:
                case ILOpCode.Localloc:
                case ILOpCode.Endfilter:
                case ILOpCode.Volatile:
                case ILOpCode.Tail:
                case ILOpCode.Cpblk:
                case ILOpCode.Initblk:
                case ILOpCode.Rethrow:
                case ILOpCode.Refanytype:
                case ILOpCode.Readonly:
                    break;
                case ILOpCode.Ldarg_s:
                case ILOpCode.Ldarga_s:
                case ILOpCode.Starg_s:
                case ILOpCode.Ldloc_s:
                case ILOpCode.Ldloca_s:
                case ILOpCode.Stloc_s:
                case ILOpCode.Ldc_i4_s:
                case ILOpCode.Br_s:
                case ILOpCode.Brfalse_s:
                case ILOpCode.Brtrue_s:
                case ILOpCode.Beq_s:
                case ILOpCode.Bge_s:
                case ILOpCode.Bgt_s:
                case ILOpCode.Ble_s:
                case ILOpCode.Blt_s:
                case ILOpCode.Bne_un_s:
                case ILOpCode.Bge_un_s:
                case ILOpCode.Bgt_un_s:
                case ILOpCode.Ble_un_s:
                case ILOpCode.Blt_un_s:
                case ILOpCode.Leave_s:
                case ILOpCode.Unaligned:
                    // Writing to the entire operand to make sure we are resetting the entire value
                    instruction.Operand64 = _ilReader.ReadByte();
                    break;
                case ILOpCode.Ldarg:
                case ILOpCode.Ldarga:
                case ILOpCode.Starg:
                case ILOpCode.Ldloc:
                case ILOpCode.Ldloca:
                case ILOpCode.Stloc:
                    // Writing to the entire operand to make sure we are resetting the entire value
                    instruction.Operand64 = _ilReader.ReadUInt16();
                    break;
                case ILOpCode.Ldc_i4:
                case ILOpCode.Ldc_r4:
                case ILOpCode.Jmp:
                case ILOpCode.Call:
                case ILOpCode.Calli:
                case ILOpCode.Br:
                case ILOpCode.Brfalse:
                case ILOpCode.Brtrue:
                case ILOpCode.Beq:
                case ILOpCode.Bge:
                case ILOpCode.Bgt:
                case ILOpCode.Ble:
                case ILOpCode.Blt:
                case ILOpCode.Bne_un:
                case ILOpCode.Bge_un:
                case ILOpCode.Bgt_un:
                case ILOpCode.Ble_un:
                case ILOpCode.Blt_un:
                case ILOpCode.Callvirt:
                case ILOpCode.Cpobj:
                case ILOpCode.Ldobj:
                case ILOpCode.Ldstr:
                case ILOpCode.Newobj:
                case ILOpCode.Castclass:
                case ILOpCode.Isinst:
                case ILOpCode.Unbox:
                case ILOpCode.Ldfld:
                case ILOpCode.Ldflda:
                case ILOpCode.Stfld:
                case ILOpCode.Ldsfld:
                case ILOpCode.Ldsflda:
                case ILOpCode.Stsfld:
                case ILOpCode.Stobj:
                case ILOpCode.Box:
                case ILOpCode.Newarr:
                case ILOpCode.Ldelema:
                case ILOpCode.Ldelem:
                case ILOpCode.Stelem:
                case ILOpCode.Unbox_any:
                case ILOpCode.Refanyval:
                case ILOpCode.Mkrefany:
                case ILOpCode.Ldtoken:
                case ILOpCode.Leave:
                case ILOpCode.Ldtarg:
                case ILOpCode.Ldftn:
                case ILOpCode.Ldvirtftn:
                case ILOpCode.Initobj:
                case ILOpCode.Constrained:
                case ILOpCode.Sizeof:
                    // Writing to the entire operand to make sure we are resetting the entire value
                    instruction.Operand64 = _ilReader.ReadUInt32();
                    break;
                case ILOpCode.Ldc_i8:
                case ILOpCode.Ldc_r8:
                    // Writing to the entire operand to make sure we are resetting the entire value
                    instruction.Operand64 = _ilReader.ReadUInt64();
                    break;
                case ILOpCode.Switch:
                    var targetCount = _ilReader.ReadInt32();
                    // Writing to the entire operand to make sure we are resetting the entire value
                    instruction.Operand64 = _ilReader.ReadUInt32();
                    // Special case, followed by Operand = target count * 4 bytes
                    _ilReader.Offset += _ilReader.Offset + targetCount * sizeof(uint);
                    break;
                default:
                    throw new ArgumentException(StarkPlatform.Reflection.Resources.SR.Format(SR.UnexpectedOpCode, opCode), nameof(opCode));
            }
        }
    }
}
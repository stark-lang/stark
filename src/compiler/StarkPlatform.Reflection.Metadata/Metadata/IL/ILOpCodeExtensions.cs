// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using SR = StarkPlatform.Reflection.Resources.SR;

namespace StarkPlatform.Reflection.Metadata
{
    public static partial class ILOpCodeExtensions
    {
        /// <summary>
        /// Get the size in bytes of the operand for a particular <see cref="ILOpCode"/>.
        /// </summary>
        /// <param name="opCode">The OpCode</param>
        /// <returns>Size in bytes of the operand of this OpCode</returns>
        /// <remarks>
        /// For switch OpCode it will return 4 while the full size is determined by also multiplying the operand value by sizeof(uint)
        /// </remarks>
        public static int GetOperandSize(this ILOpCode opCode)
        {
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
                    return 0;

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
                    return 1;

                case ILOpCode.Ldarg:
                case ILOpCode.Ldarga:
                case ILOpCode.Starg:
                case ILOpCode.Ldloc:
                case ILOpCode.Ldloca:
                case ILOpCode.Stloc:
                    return 2;

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
                    return 4;
                case ILOpCode.Ldc_i8:
                case ILOpCode.Ldc_r8:
                    return 8;
                case ILOpCode.Switch:
                    return 4;
                default:
                    throw new ArgumentException(StarkPlatform.Reflection.Resources.SR.Format(SR.UnexpectedOpCode, opCode), nameof(opCode));
            }
        }

        /// <summary>
        /// Returns true of the specified op-code is a branch to a label.
        /// </summary>
        public static bool IsBranch(this ILOpCode opCode)
        {
            switch (opCode)
            {
                case ILOpCode.Br:
                case ILOpCode.Br_s:
                case ILOpCode.Brtrue:
                case ILOpCode.Brtrue_s:
                case ILOpCode.Brfalse:
                case ILOpCode.Brfalse_s:
                case ILOpCode.Beq:
                case ILOpCode.Beq_s:
                case ILOpCode.Bne_un:
                case ILOpCode.Bne_un_s:
                case ILOpCode.Bge:
                case ILOpCode.Bge_s:
                case ILOpCode.Bge_un:
                case ILOpCode.Bge_un_s:
                case ILOpCode.Bgt:
                case ILOpCode.Bgt_s:
                case ILOpCode.Bgt_un:
                case ILOpCode.Bgt_un_s:
                case ILOpCode.Ble:
                case ILOpCode.Ble_s:
                case ILOpCode.Ble_un:
                case ILOpCode.Ble_un_s:
                case ILOpCode.Blt:
                case ILOpCode.Blt_s:
                case ILOpCode.Blt_un:
                case ILOpCode.Blt_un_s:
                case ILOpCode.Leave:
                case ILOpCode.Leave_s:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Calculate the size of the specified branch instruction operand.
        /// </summary>
        /// <param name="opCode">Branch op-code.</param>
        /// <returns>1 if <paramref name="opCode"/> is a short branch or 4 if it is a long branch.</returns>
        /// <exception cref="ArgumentException">Specified <paramref name="opCode"/> is not a branch op-code.</exception>
        public static int GetBranchOperandSize(this ILOpCode opCode)
        {
            switch (opCode)
            {
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
                    return 1;

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
                case ILOpCode.Leave:
                    return 4;
            }

            throw new ArgumentException(StarkPlatform.Reflection.Resources.SR.Format(SR.UnexpectedOpCode, opCode), nameof(opCode));
        }

        public static bool IsConditionalBranch(this ILOpCode opCode)
        {
            return !(opCode == ILOpCode.Br || opCode == ILOpCode.Br_s ||
                   opCode == ILOpCode.Leave || opCode == ILOpCode.Leave_s);
        }

        /// <summary>
        /// Get a short form of the specified branch op-code.
        /// </summary>
        /// <param name="opCode">Branch op-code.</param>
        /// <returns>Short form of the branch op-code.</returns>
        /// <exception cref="ArgumentException">Specified <paramref name="opCode"/> is not a branch op-code.</exception>
        public static ILOpCode GetShortBranch(this ILOpCode opCode)
        {
            switch (opCode)
            {
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
                    return opCode;

                case ILOpCode.Br:
                    return ILOpCode.Br_s;

                case ILOpCode.Brfalse:
                    return ILOpCode.Brfalse_s;

                case ILOpCode.Brtrue:
                    return ILOpCode.Brtrue_s;

                case ILOpCode.Beq:
                    return ILOpCode.Beq_s;

                case ILOpCode.Bge:
                    return ILOpCode.Bge_s;

                case ILOpCode.Bgt:
                    return ILOpCode.Bgt_s;

                case ILOpCode.Ble:
                    return ILOpCode.Ble_s;

                case ILOpCode.Blt:
                    return ILOpCode.Blt_s;

                case ILOpCode.Bne_un:
                    return ILOpCode.Bne_un_s;

                case ILOpCode.Bge_un:
                    return ILOpCode.Bge_un_s;

                case ILOpCode.Bgt_un:
                    return ILOpCode.Bgt_un_s;

                case ILOpCode.Ble_un:
                    return ILOpCode.Ble_un_s;

                case ILOpCode.Blt_un:
                    return ILOpCode.Blt_un_s;

                case ILOpCode.Leave:
                    return ILOpCode.Leave_s;
            }

            throw new ArgumentException(StarkPlatform.Reflection.Resources.SR.Format(SR.UnexpectedOpCode, opCode), nameof(opCode));
        }

        /// <summary>
        /// Get a long form of the specified branch op-code.
        /// </summary>
        /// <param name="opCode">Branch op-code.</param>
        /// <returns>Long form of the branch op-code.</returns>
        /// <exception cref="ArgumentException">Specified <paramref name="opCode"/> is not a branch op-code.</exception>
        public static ILOpCode GetLongBranch(this ILOpCode opCode)
        {
            switch (opCode)
            {
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
                case ILOpCode.Leave:
                    return opCode;

                case ILOpCode.Br_s:
                    return ILOpCode.Br;

                case ILOpCode.Brfalse_s:
                    return ILOpCode.Brfalse;

                case ILOpCode.Brtrue_s:
                    return ILOpCode.Brtrue;

                case ILOpCode.Beq_s:
                    return ILOpCode.Beq;

                case ILOpCode.Bge_s:
                    return ILOpCode.Bge;

                case ILOpCode.Bgt_s:
                    return ILOpCode.Bgt;

                case ILOpCode.Ble_s:
                    return ILOpCode.Ble;

                case ILOpCode.Blt_s:
                    return ILOpCode.Blt;

                case ILOpCode.Bne_un_s:
                    return ILOpCode.Bne_un;

                case ILOpCode.Bge_un_s:
                    return ILOpCode.Bge_un;

                case ILOpCode.Bgt_un_s:
                    return ILOpCode.Bgt_un;

                case ILOpCode.Ble_un_s:
                    return ILOpCode.Ble_un;

                case ILOpCode.Blt_un_s:
                    return ILOpCode.Blt_un;

                case ILOpCode.Leave_s:
                    return ILOpCode.Leave;
            }

            throw new ArgumentException(StarkPlatform.Reflection.Resources.SR.Format(SR.UnexpectedOpCode, opCode), nameof(opCode));
        }
    }
}

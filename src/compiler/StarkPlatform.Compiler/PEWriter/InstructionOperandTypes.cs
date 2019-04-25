// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Reflection.Emit;
using System.Reflection.Stark.Metadata;

namespace StarkPlatform.Cci
{
    internal static class InstructionOperandTypes
    {
        internal static OperandType ReadOperandType(ImmutableArray<byte> il, ref int position)
        {
            byte operation = il[position++];
            byte operandType;
            ILOpCode opCode;
            if (operation == 0xfe)
            {
                opCode = (ILOpCode)0xfe00;
                var index = il[position++];
                opCode = (ILOpCode)((ushort)opCode | index);
                operandType = TwoByte[index];
            }
            else
            {
                opCode = (ILOpCode)operation;
                operandType = OneByte[operation];
            }

            if (operandType == 0)
            {
                throw new InvalidOperationException($"The opcode `{opCode}:{(ushort)opCode:4X}` does not an a proper operand type");
            }
            return (OperandType)(operandType - 1);
        }

        // internal for testing
        internal static readonly byte[] OneByte = new byte[]
        {
            1 + (byte)OperandType.InlineNone,           // nop
            1 + (byte)OperandType.InlineNone,           // break
            1 + (byte)OperandType.InlineNone,           // ldarg.0
            1 + (byte)OperandType.InlineNone,           // ldarg.1
            1 + (byte)OperandType.InlineNone,           // ldarg.2
            1 + (byte)OperandType.InlineNone,           // ldarg.3
            1 + (byte)OperandType.InlineNone,           // ldloc.0
            1 + (byte)OperandType.InlineNone,           // ldloc.1
            1 + (byte)OperandType.InlineNone,           // ldloc.2
            1 + (byte)OperandType.InlineNone,           // ldloc.3
            1 + (byte)OperandType.InlineNone,           // stloc.0
            1 + (byte)OperandType.InlineNone,           // stloc.1
            1 + (byte)OperandType.InlineNone,           // stloc.2
            1 + (byte)OperandType.InlineNone,           // stloc.3
            1 + (byte)OperandType.ShortInlineVar,       // ldarg.s
            1 + (byte)OperandType.ShortInlineVar,       // ldarga.s
            1 + (byte)OperandType.ShortInlineVar,       // starg.s
            1 + (byte)OperandType.ShortInlineVar,       // ldloc.s
            1 + (byte)OperandType.ShortInlineVar,       // ldloca.s
            1 + (byte)OperandType.ShortInlineVar,       // stloc.s
            1 + (byte)OperandType.InlineNone,           // ldnull
            1 + (byte)OperandType.InlineNone,           // ldc.i4.m1
            1 + (byte)OperandType.InlineNone,           // ldc.i4.0
            1 + (byte)OperandType.InlineNone,           // ldc.i4.1
            1 + (byte)OperandType.InlineNone,           // ldc.i4.2
            1 + (byte)OperandType.InlineNone,           // ldc.i4.3
            1 + (byte)OperandType.InlineNone,           // ldc.i4.4
            1 + (byte)OperandType.InlineNone,           // ldc.i4.5
            1 + (byte)OperandType.InlineNone,           // ldc.i4.6
            1 + (byte)OperandType.InlineNone,           // ldc.i4.7
            1 + (byte)OperandType.InlineNone,           // ldc.i4.8
            1 + (byte)OperandType.ShortInlineI,         // ldc.i4.s
            1 + (byte)OperandType.InlineI,              // ldc.i4
            1 + (byte)OperandType.InlineI8,             // ldc.i8
            1 + (byte)OperandType.ShortInlineR,         // ldc.r4
            1 + (byte)OperandType.InlineR,              // ldc.r8
            0,
            1 + (byte)OperandType.InlineNone,           // dup
            1 + (byte)OperandType.InlineNone,           // pop
            1 + (byte)OperandType.InlineMethod,         // jmp
            1 + (byte)OperandType.InlineMethod,         // call
            1 + (byte)OperandType.InlineSig,            // calli
            1 + (byte)OperandType.InlineNone,           // ret
            1 + (byte)OperandType.ShortInlineBrTarget,  // br.s
            1 + (byte)OperandType.ShortInlineBrTarget,  // brfalse.s
            1 + (byte)OperandType.ShortInlineBrTarget,  // brtrue.s
            1 + (byte)OperandType.ShortInlineBrTarget,  // beq.s
            1 + (byte)OperandType.ShortInlineBrTarget,  // bge.s
            1 + (byte)OperandType.ShortInlineBrTarget,  // bgt.s
            1 + (byte)OperandType.ShortInlineBrTarget,  // ble.s
            1 + (byte)OperandType.ShortInlineBrTarget,  // blt.s
            1 + (byte)OperandType.ShortInlineBrTarget,  // bne.un.s
            1 + (byte)OperandType.ShortInlineBrTarget,  // bge.un.s
            1 + (byte)OperandType.ShortInlineBrTarget,  // bgt.un.s
            1 + (byte)OperandType.ShortInlineBrTarget,  // ble.un.s
            1 + (byte)OperandType.ShortInlineBrTarget,  // blt.un.s
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1 + (byte)OperandType.InlineSwitch,         // switch
            1 + (byte)OperandType.InlineNone,           // ldind.i1
            1 + (byte)OperandType.InlineNone,           // ldind.u1
            1 + (byte)OperandType.InlineNone,           // ldind.i2
            1 + (byte)OperandType.InlineNone,           // ldind.u2
            1 + (byte)OperandType.InlineNone,           // ldind.i4
            1 + (byte)OperandType.InlineNone,           // ldind.u4
            1 + (byte)OperandType.InlineNone,           // ldind.i8
            1 + (byte)OperandType.InlineNone,           // ldind.i
            1 + (byte)OperandType.InlineNone,           // ldind.r4
            1 + (byte)OperandType.InlineNone,           // ldind.r8
            1 + (byte)OperandType.InlineNone,           // ldind.ref
            1 + (byte)OperandType.InlineNone,           // stind.ref
            1 + (byte)OperandType.InlineNone,           // stind.i1
            1 + (byte)OperandType.InlineNone,           // stind.i2
            1 + (byte)OperandType.InlineNone,           // stind.i4
            1 + (byte)OperandType.InlineNone,           // stind.i8
            1 + (byte)OperandType.InlineNone,           // stind.r4
            1 + (byte)OperandType.InlineNone,           // stind.r8
            1 + (byte)OperandType.InlineNone,           // add
            1 + (byte)OperandType.InlineNone,           // sub
            1 + (byte)OperandType.InlineNone,           // mul
            1 + (byte)OperandType.InlineNone,           // div
            1 + (byte)OperandType.InlineNone,           // div.un
            1 + (byte)OperandType.InlineNone,           // rem
            1 + (byte)OperandType.InlineNone,           // rem.un
            1 + (byte)OperandType.InlineNone,           // and
            1 + (byte)OperandType.InlineNone,           // or
            1 + (byte)OperandType.InlineNone,           // xor
            1 + (byte)OperandType.InlineNone,           // shl
            1 + (byte)OperandType.InlineNone,           // shr
            1 + (byte)OperandType.InlineNone,           // shr.un
            1 + (byte)OperandType.InlineNone,           // neg
            1 + (byte)OperandType.InlineNone,           // not
            1 + (byte)OperandType.InlineNone,           // conv.i1
            1 + (byte)OperandType.InlineNone,           // conv.i2
            1 + (byte)OperandType.InlineNone,           // conv.i4
            1 + (byte)OperandType.InlineNone,           // conv.i8
            1 + (byte)OperandType.InlineNone,           // conv.r4
            1 + (byte)OperandType.InlineNone,           // conv.r8
            1 + (byte)OperandType.InlineNone,           // conv.u4
            1 + (byte)OperandType.InlineNone,           // conv.u8
            1 + (byte)OperandType.InlineMethod,         // callvirt
            1 + (byte)OperandType.InlineType,           // cpobj
            1 + (byte)OperandType.InlineType,           // ldobj
            1 + (byte)OperandType.InlineString,         // ldstr
            1 + (byte)OperandType.InlineMethod,         // newobj
            1 + (byte)OperandType.InlineType,           // castclass
            1 + (byte)OperandType.InlineType,           // isinst
            1 + (byte)OperandType.InlineNone,           // conv.r.un
            0,
            0,
            1 + (byte)OperandType.InlineType,           // unbox
            1 + (byte)OperandType.InlineNone,           // throw
            1 + (byte)OperandType.InlineField,          // ldfld
            1 + (byte)OperandType.InlineField,          // ldflda
            1 + (byte)OperandType.InlineField,          // stfld
            1 + (byte)OperandType.InlineField,          // ldsfld
            1 + (byte)OperandType.InlineField,          // ldsflda
            1 + (byte)OperandType.InlineField,          // stsfld
            1 + (byte)OperandType.InlineType,           // stobj
            1 + (byte)OperandType.InlineNone,           // conv.ovf.i1.un
            1 + (byte)OperandType.InlineNone,           // conv.ovf.i2.un
            1 + (byte)OperandType.InlineNone,           // conv.ovf.i4.un
            1 + (byte)OperandType.InlineNone,           // conv.ovf.i8.un
            1 + (byte)OperandType.InlineNone,           // conv.ovf.u1.un
            1 + (byte)OperandType.InlineNone,           // conv.ovf.u2.un
            1 + (byte)OperandType.InlineNone,           // conv.ovf.u4.un
            1 + (byte)OperandType.InlineNone,           // conv.ovf.u8.un
            1 + (byte)OperandType.InlineNone,           // conv.ovf.i.un
            1 + (byte)OperandType.InlineNone,           // conv.ovf.u.un
            1 + (byte)OperandType.InlineType,           // box
            1 + (byte)OperandType.InlineType,           // newarr
            1 + (byte)OperandType.InlineNone,           // ldlen
            1 + (byte)OperandType.InlineType,           // ldelema
            1 + (byte)OperandType.InlineNone,           // ldelem.i1
            1 + (byte)OperandType.InlineNone,           // ldelem.u1
            1 + (byte)OperandType.InlineNone,           // ldelem.i2
            1 + (byte)OperandType.InlineNone,           // ldelem.u2
            1 + (byte)OperandType.InlineNone,           // ldelem.i4
            1 + (byte)OperandType.InlineNone,           // ldelem.u4
            1 + (byte)OperandType.InlineNone,           // ldelem.i8
            1 + (byte)OperandType.InlineNone,           // ldelem.i
            1 + (byte)OperandType.InlineNone,           // ldelem.r4
            1 + (byte)OperandType.InlineNone,           // ldelem.r8
            1 + (byte)OperandType.InlineNone,           // ldelem.ref
            1 + (byte)OperandType.InlineNone,           // stelem.i
            1 + (byte)OperandType.InlineNone,           // stelem.i1
            1 + (byte)OperandType.InlineNone,           // stelem.i2
            1 + (byte)OperandType.InlineNone,           // stelem.i4
            1 + (byte)OperandType.InlineNone,           // stelem.i8
            1 + (byte)OperandType.InlineNone,           // stelem.r4
            1 + (byte)OperandType.InlineNone,           // stelem.r8
            1 + (byte)OperandType.InlineNone,           // stelem.ref
            1 + (byte)OperandType.InlineType,           // ldelem
            1 + (byte)OperandType.InlineType,           // stelem
            1 + (byte)OperandType.InlineType,           // unbox.any
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1 + (byte)OperandType.InlineNone,           // conv.ovf.i1
            1 + (byte)OperandType.InlineNone,           // conv.ovf.u1
            1 + (byte)OperandType.InlineNone,           // conv.ovf.i2
            1 + (byte)OperandType.InlineNone,           // conv.ovf.u2
            1 + (byte)OperandType.InlineNone,           // conv.ovf.i4
            1 + (byte)OperandType.InlineNone,           // conv.ovf.u4
            1 + (byte)OperandType.InlineNone,           // conv.ovf.i8
            1 + (byte)OperandType.InlineNone,           // conv.ovf.u8
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1 + (byte)OperandType.InlineType,           // refanyval
            1 + (byte)OperandType.InlineNone,           // ckfinite
            0,
            0,
            1 + (byte)OperandType.InlineType,           // mkrefany
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1 + (byte)OperandType.InlineTok,            // ldtoken
            1 + (byte)OperandType.InlineNone,           // conv.u2
            1 + (byte)OperandType.InlineNone,           // conv.u1
            1 + (byte)OperandType.InlineNone,           // conv.i
            1 + (byte)OperandType.InlineNone,           // conv.ovf.i
            1 + (byte)OperandType.InlineNone,           // conv.ovf.u
            1 + (byte)OperandType.InlineNone,           // add.ovf
            1 + (byte)OperandType.InlineNone,           // add.ovf.un
            1 + (byte)OperandType.InlineNone,           // mul.ovf
            1 + (byte)OperandType.InlineNone,           // mul.ovf.un
            1 + (byte)OperandType.InlineNone,           // sub.ovf
            1 + (byte)OperandType.InlineNone,           // sub.ovf.un
            1 + (byte)OperandType.InlineNone,           // endfinally
            0,
            1 + (byte)OperandType.ShortInlineBrTarget,  // leave.s
            1 + (byte)OperandType.InlineNone,           // stind.i
            1 + (byte)OperandType.InlineNone,           // conv.u            (0xe0)
            1 + (byte)OperandType.InlineType,           // ldtarg            (0xe1)
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
        };

        // internal for testing
        internal static readonly byte[] TwoByte = new byte[]
        {
            1 + (byte)OperandType.InlineNone,           // arglist           (0xfe 0x00)
            1 + (byte)OperandType.InlineNone,           // ceq
            1 + (byte)OperandType.InlineNone,           // cgt
            1 + (byte)OperandType.InlineNone,           // cgt.un
            1 + (byte)OperandType.InlineNone,           // clt
            1 + (byte)OperandType.InlineNone,           // clt.un
            1 + (byte)OperandType.InlineMethod,         // ldftn
            1 + (byte)OperandType.InlineMethod,         // ldvirtftn
            0,
            1 + (byte)OperandType.InlineVar,            // ldarg
            1 + (byte)OperandType.InlineVar,            // ldarga
            1 + (byte)OperandType.InlineVar,            // starg
            1 + (byte)OperandType.InlineVar,            // ldloc
            1 + (byte)OperandType.InlineVar,            // ldloca
            1 + (byte)OperandType.InlineVar,            // stloc
            1 + (byte)OperandType.InlineNone,           // localloc
            0,
            1 + (byte)OperandType.InlineNone,           // endfilter
            1 + (byte)OperandType.ShortInlineI,         // unaligned.
            1 + (byte)OperandType.InlineNone,           // volatile.
            1 + (byte)OperandType.InlineNone,           // tail.
            1 + (byte)OperandType.InlineType,           // initobj
            1 + (byte)OperandType.InlineType,           // constrained.
            1 + (byte)OperandType.InlineNone,           // cpblk
            1 + (byte)OperandType.InlineNone,           // initblk
            0,
            1 + (byte)OperandType.InlineNone,           // rethrow
            0,
            1 + (byte)OperandType.InlineType,           // sizeof
            1 + (byte)OperandType.InlineNone,           // refanytype
            1 + (byte)OperandType.InlineNone,           // readonly.         (0xfe 0x1e)
        };
    }
}

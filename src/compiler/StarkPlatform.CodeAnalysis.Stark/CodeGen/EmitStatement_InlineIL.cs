
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Stark.Metadata;
using StarkPlatform.CodeAnalysis.CodeGen;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.PooledObjects;
using StarkPlatform.CodeAnalysis.Text;
using Roslyn.Utilities;
using static StarkPlatform.CodeAnalysis.Stark.Binder;

namespace StarkPlatform.CodeAnalysis.Stark.CodeGen
{
    internal partial class CodeGenerator
    {
        private void EmitInlineILStatement(BoundInlineILStatement ilEmit)
        {
            var opcode = ilEmit.Instruction.OpCode;
            if (opcode.HasVariableStackBehavior())
            {
                int stackBehavior;
                if (ilEmit.Argument is BoundCall)
                {
                    var call = (BoundCall)ilEmit.Argument;
                    stackBehavior = GetCallStackBehavior(call);
                }
                else if (ilEmit.Argument is BoundMethodGroup)
                {
                    var call = ((BoundMethodGroup)ilEmit.Argument).Methods[0];
                    stackBehavior = GetCallStackBehavior(call);
                }
                else if (opcode == ILOpCode.Ret)
                {
                    _builder.EmitRet(_builder.IsStackEmpty);
                    return;
                }
                else
                {
                    throw new NotSupportedException($"Unsupported method call [{ilEmit.Argument.GetType()}] for the IL instruction <{ilEmit.Instruction.Name}>");
                }
                _builder.EmitOpCode(opcode, stackBehavior);
            }
            else
            {
                // ldloc/stloc ldarg/starg are handle below so we skip them here
                if (ilEmit.Instruction.Argument != OpCodeArg.LocalVar
                    && ilEmit.Instruction.Argument != OpCodeArg.ArgumentVar)
                {
                    _builder.EmitOpCode(opcode);
                }
            }

            if (ilEmit.Instruction.Argument == OpCodeArg.None)
            {
                return;
            }

            switch (ilEmit.Instruction.Argument)
            {
                case OpCodeArg.Target32:
                case OpCodeArg.Target8:
                    _builder.MarkLabel((BoundLabel)ilEmit.Argument);
                    break;
                case OpCodeArg.ElementType:
                case OpCodeArg.TypeToken:
                case OpCodeArg.Class:
                    if (ilEmit.Argument is BoundTypeExpression boundType)
                    {
                        EmitSymbolToken(boundType.Type, ilEmit.Syntax);
                    }
                    else if (ilEmit.Argument is BoundLiteral boundLiteral)
                    {
                        Debug.Assert(boundLiteral.ConstantValue.TypeParameter != null);
                        EmitSymbolToken((TypeSymbol)boundLiteral.ConstantValue.TypeParameter, ilEmit.Syntax);
                    }
                    else
                    {
                        EmitSymbolToken(((BoundConstTypeParameterExpression)ilEmit.Argument).Parameter, ilEmit.Syntax);
                    }
                    break;
                case OpCodeArg.LocalVar:
                    if (opcode == ILOpCode.Ldloc && ilEmit.Argument is BoundLocal)
                    {
                        var localDef = _builder.LocalSlotManager.GetLocal(((BoundLocal)ilEmit.Argument).LocalSymbol);
                        _builder.EmitLocalLoad(localDef);
                    }
                    else if (opcode == ILOpCode.Stloc && ilEmit.Argument is BoundLocal)
                    {
                        var localDef = _builder.LocalSlotManager.GetLocal(((BoundLocal)ilEmit.Argument).LocalSymbol);
                        _builder.EmitLocalStore(localDef);
                    }
                    else
                    {
                        throw new NotSupportedException($"The bound type [{ilEmit.Argument.GetType()}] is not supported for the IL instruction <{ilEmit.Instruction.Name}>");
                    }
                    break;
                case OpCodeArg.ArgumentVar:
                    if (opcode == ILOpCode.Ldarg && ilEmit.Argument is BoundParameter)
                    {
                        this.EmitParameterLoad((BoundParameter)ilEmit.Argument);
                    }
                    else if (opcode == ILOpCode.Starg && ilEmit.Argument is BoundParameter)
                    {
                        this.EmitParameterStore((BoundParameter)ilEmit.Argument, false);
                    }
                    else
                    {
                        throw new NotSupportedException($"The bound type [{ilEmit.Argument.GetType()}] is not supported for the IL instruction <{ilEmit.Instruction.Name}>");
                    }
                    break;
                case OpCodeArg.Method:
                case OpCodeArg.CallSite:
                    if (ilEmit.Argument is BoundCall)
                    {
                        EmitSymbolToken(((BoundCall)ilEmit.Argument).Method, ilEmit.Syntax, null); // TODO: handle varargs
                    }
                    else if (ilEmit.Argument is BoundMethodGroup)
                    {
                        EmitSymbolToken(((BoundMethodGroup)ilEmit.Argument).Methods[0], ilEmit.Syntax, null); // TODO: handle varargs
                    }
                    else
                    {
                        throw new NotSupportedException($"The bound type [{ilEmit.Argument.GetType()}] is not supported for the IL instruction <{ilEmit.Instruction.Name}>");
                    }
                    break;
                case OpCodeArg.Field:
                    EmitSymbolToken(((BoundFieldAccess)ilEmit.Argument).FieldSymbol, ilEmit.Syntax);
                    break;
                case OpCodeArg.Token:
                    var typeExpression = ilEmit.Argument as BoundTypeExpression;
                    if (typeExpression != null)
                    {
                        EmitSymbolToken(typeExpression.Type, ilEmit.Syntax);
                    }
                    else if (ilEmit.Argument is BoundFieldAccess)
                    {
                        EmitSymbolToken(((BoundFieldAccess)ilEmit.Argument).FieldSymbol, ilEmit.Syntax);
                    }
                    else if (ilEmit.Argument is BoundCall)
                    {
                        EmitSymbolToken(((BoundCall)ilEmit.Argument).Method, ilEmit.Syntax, null); // TODO: handle varargs
                    }
                    else if (ilEmit.Argument is BoundMethodGroup)
                    {
                        EmitSymbolToken(((BoundMethodGroup)ilEmit.Argument).Methods[0], ilEmit.Syntax, null); // TODO: don't handle varargs
                    }
                    else
                    {
                        throw new NotSupportedException($"The bound type [{ilEmit.Argument.GetType()}] is not supported for the IL instruction <{ilEmit.Instruction.Name}>");
                    }
                    break;
                case OpCodeArg.Constructor:
                    EmitSymbolToken(((BoundObjectCreationExpression)ilEmit.Argument).Constructor, ilEmit.Syntax, null);
                    break;
                case OpCodeArg.Int8:
                case OpCodeArg.UInt8:
                case OpCodeArg.UInt16:
                case OpCodeArg.Int32:
                case OpCodeArg.UInt32:
                case OpCodeArg.Int64:
                case OpCodeArg.UInt64:
                case OpCodeArg.Float32:
                case OpCodeArg.Float64:
                case OpCodeArg.String:
                    // TODO: check values
                    _builder.EmitConstantValue(((BoundLiteral)ilEmit.Argument).ConstantValue);
                    break;
                default:
                    throw new NotImplementedException(
                        $"The IL instruction <{ilEmit.Instruction.Name}> is not implemented");
            }
        }

        private static int GetCallStackBehavior(MethodSymbol method)
        {
            int stack = 0;

            if (!method.ReturnsVoid)
            {
                // The call puts the return value on the stack.
                stack += 1;
            }

            if (!method.IsStatic)
            {
                // The call pops the receiver off the stack.
                stack -= 1;
            }

            if (method.IsVararg)
            {
                throw new NotImplementedException("Vararg is not supported for method");
            }
            else
            {
                // The call pops all the arguments.
                stack -= method.ParameterCount;
            }

            return stack;
        }
    }
}

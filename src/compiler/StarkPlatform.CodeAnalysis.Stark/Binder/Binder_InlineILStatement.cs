using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using Roslyn.Utilities;
using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark
{
    /// <summary>
    /// Handles Binder to Inline IL
    /// </summary>
    internal partial class Binder
    {
        private BoundStatement BindInlineILStatement(InlineILStatementSyntax node, DiagnosticBag diagnostics)
        {
            var ilBinder = this.GetBinder(node.Parent);
            Debug.Assert(ilBinder != null);

            // TODO: pool this string writer
            var builder = new StringWriter();
            foreach (var part in node.Instruction)
            {
                part.WriteTo(builder, false, false);
            }
            var ilOpcode = builder.ToString();

            var instruction = ILInstruction.Get(ilOpcode);

            // Error: The IL opcode `{0}` is not recognized. Check the documentation for the list of opcodes.
            if (instruction == null)
            {
                Error(diagnostics, ErrorCode.ERR_InvalidInlineILOpcode, node.Instruction.First(), ilOpcode);
                return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
            }

            // Error: The IL opcode `{0}` is expecting an argument of type `{1}`
            if (instruction.Argument != OpCodeArg.None && node.Argument == null)
            {
                Error(diagnostics, ErrorCode.ERR_InlineILArgumentExpected, node.Instruction.First(), ilOpcode, instruction.Argument.ToString());
                return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
            }

            // Error: The IL opcode `{0}` is not expecting an argument
            if (instruction.Argument == OpCodeArg.None && node.Argument != null)
            {
                Error(diagnostics, ErrorCode.ERR_InlineILNoArgumentExpected, node.Argument, ilOpcode, instruction.Argument.ToString());
                return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
            }

            BoundExpression bound = null;
            switch (instruction.Argument)
            {
                case OpCodeArg.None:
                    break;
                case OpCodeArg.Target32:
                    bound = ilBinder.BindLabel(node.Argument, diagnostics) as BoundLabel;
                    if (bound == null)
                    {
                        Error(diagnostics, ErrorCode.ERR_LabelNotFound, node.Argument, node.Argument.ToString());
                        return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
                    }
                    break;
                case OpCodeArg.Target8:
                    bound = ilBinder.BindLabel(node.Argument, diagnostics) as BoundLabel;
                    if (bound == null)
                    {
                        Error(diagnostics, ErrorCode.ERR_LabelNotFound, node.Argument, node.Argument.ToString());
                        return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
                    }
                    break;
                case OpCodeArg.Method:
                case OpCodeArg.CallSite:
                    {
                        var methodBound = ilBinder.BindExpression(node.Argument, diagnostics);
                        if (methodBound == null || (!(methodBound is BoundCall) && !(methodBound is BoundMethodGroup)))
                        {
                            Error(diagnostics, ErrorCode.ERR_MethodNameExpected, node.Argument);
                            return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
                        }
                        else
                        {
                            bound = methodBound;
                        }
                        break;
                    }

                case OpCodeArg.ElementType:
                case OpCodeArg.TypeToken:
                case OpCodeArg.Class:
                    var expression = ilBinder.BindExpression(node.Argument, diagnostics);
                    bound = expression as BoundTypeExpression;
                    if (bound == null)
                    {
                        bound = expression as BoundConstTypeParameterExpression;
                        if (bound == null)
                        {
                            Error(diagnostics, ErrorCode.ERR_TypeExpected, node.Argument);
                            return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
                        }
                    }
                    break;

                case OpCodeArg.Field:
                    bound = ilBinder.BindExpression(node.Argument, diagnostics) as BoundFieldAccess;
                    if (bound == null)
                    {
                        Error(diagnostics, ErrorCode.ERR_BadAccess, node.Argument);
                        return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
                    }
                    break;
                case OpCodeArg.String:
                    bound = ilBinder.BindExpression(node.Argument, diagnostics) as BoundLiteral;
                    if (bound == null || !bound.ConstantValue.IsString)
                    {
                        Error(diagnostics, ErrorCode.ERR_ExpectedVerbatimLiteral, node.Argument);
                        return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
                    }
                    break;
                case OpCodeArg.Token:
                    var tokenExpression = ilBinder.BindExpression(node.Argument, diagnostics);

                    if (tokenExpression is BoundTypeExpression || tokenExpression is BoundFieldAccess || tokenExpression is BoundCall || (tokenExpression is BoundMethodGroup && ((BoundMethodGroup)tokenExpression).Methods.Length == 1))
                    {
                        bound = tokenExpression;
                    }
                    else
                    {
                        // TODO: log proper error
                        Error(diagnostics, ErrorCode.ERR_InvalidInlineILArgument, node.Argument, ilOpcode);
                        return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
                    }
                    break;
                case OpCodeArg.Constructor:
                    bound = ilBinder.BindExpression(node.Argument, diagnostics) as BoundObjectCreationExpression;
                    if (bound == null)
                    {
                        // TODO: log proper error
                        Error(diagnostics, ErrorCode.ERR_InvalidInlineILArgument, node.Argument, ilOpcode);
                        return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
                    }
                    break;
                case OpCodeArg.ArgumentVar:
                    bound = ilBinder.BindExpression(node.Argument, diagnostics) as BoundParameter;
                    if (bound == null)
                    {
                        // TODO: log proper error
                        Error(diagnostics, ErrorCode.ERR_InvalidInlineILArgument, node.Argument, ilOpcode);
                        return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
                    }
                    break;
                case OpCodeArg.LocalVar:
                    bound = ilBinder.BindExpression(node.Argument, diagnostics) as BoundLocal;
                    if (bound == null)
                    {
                        // TODO: log proper error
                        Error(diagnostics, ErrorCode.ERR_InvalidInlineILArgument, node.Argument, ilOpcode);
                        return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
                    }
                    break;
                case OpCodeArg.Int8:
                case OpCodeArg.UInt8:
                case OpCodeArg.UInt16:
                case OpCodeArg.Int32:
                case OpCodeArg.UInt32:
                case OpCodeArg.Int64:
                case OpCodeArg.UInt64:
                    // TODO: Handle correctly arguments
                    bound = ilBinder.BindExpression(node.Argument, diagnostics) as BoundLiteral;
                    if (bound == null || !bound.ConstantValue.IsIntegral)
                    {
                        Error(diagnostics, ErrorCode.ERR_InvalidInlineILArgument, node.Argument, ilOpcode);
                        return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
                    }
                    break;
                case OpCodeArg.Float32:
                case OpCodeArg.Float64:
                    // TODO: Handle correctly arguments
                    bound = ilBinder.BindExpression(node.Argument, diagnostics) as BoundLiteral;
                    if (bound == null || !bound.ConstantValue.IsFloating)
                    {
                        Error(diagnostics, ErrorCode.ERR_InvalidInlineILArgument, node.Argument, ilOpcode);
                        return new BoundBadStatement(node, ImmutableArray<BoundNode>.Empty, true);
                    }
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(instruction.Argument);
            }

            // If we are using IL Inline outside of an unsafe context, we report an error
            ReportUnsafeIfNotAllowed(node, diagnostics);
            
            return new BoundInlineILStatement(node, instruction, bound);
        }
    }
}

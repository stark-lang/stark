using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace IntrinsicGenerator
{
    public class IntrinsicWriter
    {
        private readonly XDocument _doc;
        private readonly TextWriter _writer;
        private static readonly HashSet<string> Supports = new HashSet<string>()
        {
            "SSE",
            "SSE2",
            "SSE3",
            "SSSE3",
            "SSE4.1",
            "SSE4.2",
            "AVX",
            "AVX2",
        };

        private int _indent;
        private bool _isStartOfLine;

        private static readonly Dictionary<string, string> MapTypes = new Dictionary<string, string>()
        {
            {"unsigned int", "u32"},
            {"void", "u8"},
            {"__int64", "i64"},
            {"__int32", "i32"},
            {"__int8", "i8" },
            {"__int16", "i16" },
            {"size_t", "uint"},
            {"unsigned char", "u8"},
            {"unsigned short", "u16"},
            {"unsigned __int64", "u64"},
            {"short", "i16"},
            {"int", "i32"},
            {"char", "u8"},
            {"float", "f32"},
            {"double", "f64"},
            {"long long", "i64"},
        };

        private string _currentTech;

        public IntrinsicWriter(XDocument doc, TextWriter writer)
        {
            _doc = doc;
            _writer = writer;
        }


        public void Generate()
        {
            _currentTech = null;

            WriteLine("import core.runtime");
            WriteLine();

            WriteLine("namespace core");
            WriteLine("{");
            Indent();

            WriteLine("public partial module intrinsics");
            WriteLine("{");
            Indent();

            WriteLine("public partial module x86");
            WriteLine("{");
            Indent();

            foreach (var elt in _doc.Descendants("intrinsic"))
            {
                var tech = elt.Attribute("tech")?.Value;

                if (!Supports.Contains(tech))
                {
                    continue;
                }

                ///// __m128 _mm_mul_ps (__m128 a, __m128 b)
                ///// MULPS xmm, xmm/m128
                //@FuncImpl(FuncImplOptions.INTERNAL_CALL)
                //@Intrinsic
                //public static extern func _mm_mul_ps(left: __m128, right: __m128) -> __m128

                var name = elt.Attribute("name")?.Value;
                Debug.Assert(name != null);

                // Is this a macro
                if (name.IndexOf("M") > 0) continue;

                var techName = tech.ToLowerInvariant().Replace(".", string.Empty);

                var retType = elt.Attribute("rettype")?.Value;

                var parameters = elt.Elements("parameter");
                var isUnsafe = IsUnsafe(retType) || parameters.Any(x => IsUnsafe(x.Attribute("type").Value));
                var isMmx = retType.Contains("__m64") || parameters.Any(x => x.Attribute("type").Value.Contains("__m64"));
                // don't output mmx
                if (isMmx) continue;

                HandleModule(techName);
                WriteLine();
                //WriteLine("@FuncImpl(FuncImplOptions.INTERNAL_CALL)");
                var description = elt.Element("description")?.Value;
                if (description != null)
                {
                    description = description.Replace('"', '`');
                    description = description.Replace("\r\n", " ");
                    description = description.Replace("\n", " ");
                    WriteLine($"/// {description}");
                }
                WriteLine("@Intrinsic");
                Write($"public static {(isUnsafe?"unsafe ":string.Empty)}extern func {name}(");

                bool isFirst = true;
                foreach (var parameter in elt.Elements("parameter"))
                {
                    if (!isFirst) Write(", ");

                    var param_name = parameter.Attribute("varname")?.Value;
                    var param_type = parameter.Attribute("type")?.Value;

                    if (param_type == "void") continue;

                    Write($"{param_name}: {GetStarkType(param_type)}");
                    isFirst = false;
                }
                Write(")");

                if (retType != "void")
                {
                    Write($" -> {GetStarkType(retType)}");
                }
                WriteLine();
            }
            HandleModule(null);

            DeIndent();
            WriteLine("}"); // module x86
            DeIndent();
            WriteLine("}"); // module intrinsics
            DeIndent();
            WriteLine("}"); // namespace core
        }

        private void Indent()
        {
            _indent++;
        }

        private void DeIndent()
        {
            if (_indent == 0) throw new InvalidOperationException("Cannot de-indent more than indent");
            _indent--;
        }

        private void Write(string value)
        {
            if (_isStartOfLine)
            {
                for (int i = 0; i < _indent; i++)
                {
                    _writer.Write("    ");
                }
                _isStartOfLine = false;
            }
            _writer.Write(value);
            if (value.Contains('\n')) _isStartOfLine = true;
        }
        private void WriteLine()
        {
            Write(Environment.NewLine);
        }

        private void WriteLine(string str)
        {
            Write(str);
            Write(Environment.NewLine);
        }

        private void HandleModule(string techName)
        {
            if (_currentTech != techName)
            {
                if (_currentTech != null)
                {
                    DeIndent();
                    WriteLine("}");
                    WriteLine();
                }

                if (techName != null)
                {
                    WriteLine($"public partial module {techName}");
                    WriteLine("{");
                    Indent();
                    _currentTech = techName;
                }
            }
        }

        private static bool IsUnsafe(string cType)
        {
            return cType != null && cType.Contains('*');
        }

        private static string GetStarkType(string cType)
        {
            cType = cType.TrimEnd();
            cType = cType.Replace(" *", "*");
            cType = cType.Replace("const ", string.Empty);
            cType = cType.Replace("const", string.Empty);

            var isPointer = cType.EndsWith("*");
            cType = cType.TrimEnd('*');
            cType = cType.TrimEnd();

            string starkType;
            if (MapTypes.TryGetValue(cType, out starkType))
            {
                return (isPointer ? "*" : string.Empty) + starkType;
            }

            return (isPointer ? "*" : string.Empty) + cType;
        }
    }
}
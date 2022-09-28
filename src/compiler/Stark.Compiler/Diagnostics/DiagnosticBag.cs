// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections;
using System.Diagnostics;

namespace Stark.Compiler.Diagnostics;

[DebuggerTypeProxy(typeof(DiagnosticBagDebugView))]
public class DiagnosticBag : IEnumerable<Diagnostic>
{
    private readonly List<Diagnostic> _list;

    public DiagnosticBag()
    {
        _list = new List<Diagnostic>();
    }

    public bool HasErrors { get; private set; }

    public int Count => _list.Count;

    public void Add(Diagnostic diagnostic)
    {
        if (diagnostic.Kind == DiagnosticKind.Error)
        {
            HasErrors = true;
        }

        _list.Add(diagnostic);
    }

    public void AddRange(IEnumerable<Diagnostic> collection)
    {
        foreach (var diagnostic in collection)
        {
            Add(diagnostic);
        }
    }

    public void Clear()
    {
        _list.Clear();
        HasErrors = false;
    }

    public void CopyTo(DiagnosticBag diagnosticBag)
    {
        diagnosticBag._list.AddRange(_list);
        diagnosticBag.HasErrors = HasErrors;
    }
    
    public List<Diagnostic>.Enumerator GetEnumerator()
    {
        // List<Diagnostic>.Enumerator 
        return _list.GetEnumerator();
    }

    IEnumerator<Diagnostic> IEnumerable<Diagnostic>.GetEnumerator()
    {
        // List<Diagnostic>.Enumerator 
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_list).GetEnumerator();
    }


    internal sealed class DiagnosticBagDebugView
    {
        private readonly DiagnosticBag _collection;

        public DiagnosticBagDebugView(DiagnosticBag collection)
        {
            _collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public Diagnostic[] Items
        {
            get
            {
                var array = new Diagnostic[_collection.Count];
                _collection._list.CopyTo(array, 0);
                return array;
            }
        }
    }
}
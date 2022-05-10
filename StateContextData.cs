using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TP4
{
    public struct StateContextData
    {
        public bool HasInterfaceField;
        public SyntaxToken UsedInterface;
        public bool HasStateSetter;
        public MethodDeclarationSyntax StateSetter;

        public StateContextData(bool hasInterfaceField, SyntaxToken usedInterface, bool hasStateSetter, MethodDeclarationSyntax stateSetter)
        {
            HasInterfaceField = hasInterfaceField;
            UsedInterface = usedInterface;
            HasStateSetter = hasStateSetter;
            StateSetter = stateSetter;
        }

        public override string ToString()
        {
            return $"Has interface field? {HasInterfaceField}\n" +
                   $"Used interface is: {UsedInterface}\n" +
                   $"Has state setter? {HasStateSetter}\n" +
                   $"State setter is: {StateSetter}";
        }
    }
}

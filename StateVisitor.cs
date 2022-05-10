using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TP4
{
    public class StateVisitor : StatePatternVisitor
    {
        private readonly List<SyntaxToken> _interfacesFound;
        public StateVisitor(string filePath) : base(filePath)
        {
            _interfacesFound = new List<SyntaxToken>();
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            base.VisitInterfaceDeclaration(node);

            if (HasPublicModifier(node))
            {
                _interfacesFound.Add(node.Identifier);
            }
            
        }

        public List<SyntaxToken> GetInterfacesFound()
        {
            StartVisiting();
            return _interfacesFound;
        }

        private bool HasPublicModifier(InterfaceDeclarationSyntax node)
        {
            foreach (var modifier in node.Modifiers)
            {
                SyntaxNodeOrToken modifiNodeOrToken = modifier;

                if (modifiNodeOrToken.IsKind(SyntaxKind.PublicKeyword))
                    return true;
            }

            return false;
        }
    }
}

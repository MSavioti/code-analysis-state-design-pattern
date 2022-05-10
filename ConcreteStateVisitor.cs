using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TP4
{
    public class ConcreteStateVisitor : StatePatternVisitor
    {
        private readonly List<ClassDeclarationSyntax> _extendedClasses;
        public ConcreteStateVisitor(string filePath) : base(filePath)
        {
            _extendedClasses = new List<ClassDeclarationSyntax>();
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            base.VisitClassDeclaration(node);

            if (node.BaseList == null)
                return;

            _extendedClasses.Add(node);
        }

        public List<ClassDeclarationSyntax> GetConcreteStates(SyntaxToken usedInterface)
        {
            StartVisiting();
            var concreteStates = new List<ClassDeclarationSyntax>();

            foreach (var extendedClass in _extendedClasses)
            {
                if (ImplementsInterface(extendedClass, usedInterface))
                    concreteStates.Add(extendedClass);
            }

            return concreteStates;
        }

        private bool ImplementsInterface(ClassDeclarationSyntax extendedClass, SyntaxToken targetInterface)
        {
            if (extendedClass.BaseList == null)
                return false;

            foreach (var baseType in extendedClass.BaseList.Types)
            {
                if (baseType.ToString().Equals(targetInterface.ToString()))
                    return true;
            }

            return false;
        }
    }
}

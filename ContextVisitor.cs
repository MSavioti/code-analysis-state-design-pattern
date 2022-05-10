using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TP4
{
    public class ContextVisitor : StatePatternVisitor
    {
        private readonly List<VariableDeclarationSyntax> _fieldDeclarations;
        private readonly List<MethodDeclarationSyntax> _setters;
        private StateContextData _contextData;
        private const int ShortMethodLineCount = 4;

        public ContextVisitor(string filePath) : base(filePath)
        {
            _contextData = new StateContextData();
            _fieldDeclarations = new List<VariableDeclarationSyntax>();
            _setters = new List<MethodDeclarationSyntax>();
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            base.VisitFieldDeclaration(node);
            _fieldDeclarations.Add(node.Declaration);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            base.VisitMethodDeclaration(node);

            if (IsSetter(node))
                _setters.Add(node);
        }

        public StateContextData GetContextData(List<SyntaxToken> foundInterfaces)
        {
            StartVisiting();
            _contextData.HasInterfaceField = HasInterfaceField(foundInterfaces, out SyntaxToken usedInterface);
            _contextData.UsedInterface = usedInterface;
            _contextData.HasStateSetter = HasStateSetter(usedInterface, out MethodDeclarationSyntax stateSetter);
            _contextData.StateSetter = stateSetter;
            return _contextData;
        }

        private bool HasInterfaceField(IReadOnlyCollection<SyntaxToken> foundInterfaces, out SyntaxToken usedInterface)
        {
            foreach (var declaration in _fieldDeclarations)
            {
                foreach (var foundInterface in foundInterfaces)
                {
                    if (declaration.Type.ToString().Equals(foundInterface.ToString()))
                    {
                        usedInterface = foundInterface;
                        return true;
                    }
                }
            }

            usedInterface = new SyntaxToken();
            return false;
        }

        private bool HasStateSetter(SyntaxToken usedInterface, out MethodDeclarationSyntax stateSetter)
        {
            stateSetter = null;

            foreach (var setter in _setters)
            {
                string setterParameterName = setter.ParameterList.Parameters[0].Type.ToString();

                if (setterParameterName.Equals(usedInterface.ToString()))
                {
                    stateSetter = setter;
                    return true;
                }
            }

            return false;
        }

        private bool IsSetter(MethodDeclarationSyntax methodDeclaration)
        {
            if (!IsPublicMethod(methodDeclaration))
                return false;

            if (methodDeclaration.ParameterList.Parameters.Count != 1)
                return false;

            if (methodDeclaration.DescendantNodes().Where(
                node => node.Span.IntersectsWith(methodDeclaration.Body.Span)).OfType<ReturnStatementSyntax>().Any())
                return false;

            if (!methodDeclaration.ReturnType.GetFirstToken().RawKind.Equals(SyntaxKind.VoidKeyword.GetHashCode()))
                return false;

            if (methodDeclaration.Body.GetText().Lines.Count > ShortMethodLineCount)
                return false;

            if (HasSingleAssignmentExpression(methodDeclaration, out AssignmentExpressionSyntax setterAssignment))
            {
                if (!IsFieldMember(setterAssignment?.Left, _fieldDeclarations))
                    return false;
            }
            else
            {
                return false;
            }

            return true;
        }

        private bool IsPublicMethod(MethodDeclarationSyntax node)
        {
            foreach (var modifier in node.Modifiers)
            {
                SyntaxNodeOrToken modifiNodeOrToken = modifier;

                if (modifiNodeOrToken.IsKind(SyntaxKind.PublicKeyword))
                    return true;
            }

            return false;
        }

        private bool IsFieldMember(ExpressionSyntax expressionSyntax, List<VariableDeclarationSyntax> classVariableMembers)
        {
            foreach (var member in classVariableMembers)
            {
                foreach (var variable in member.Variables)
                {
                    if (expressionSyntax.ToString().Equals(variable.Identifier.ToString()))
                        return true;
                }
            }

            return false;
        }

        private bool HasSingleAssignmentExpression(MethodDeclarationSyntax methodDeclaration, out AssignmentExpressionSyntax setterAssignment)
        {
            int assignmentExpressionCount = 0;
            setterAssignment = null;

            IEnumerable<AssignmentExpressionSyntax> assignmentExpressions = methodDeclaration.DescendantNodes().Where(
                node => node.Span.IntersectsWith(methodDeclaration.Span)).OfType<AssignmentExpressionSyntax>();

            foreach (var assignmentExpression in assignmentExpressions)
            {
                if (!assignmentExpression.IsKind(SyntaxKind.SimpleAssignmentExpression))
                    return false;

                setterAssignment = assignmentExpression;
                assignmentExpressionCount++;

                if (assignmentExpressionCount > 1)
                    return false;
            }

            return true;
        }
    }
}

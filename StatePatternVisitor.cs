using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TP4
{
    public class StatePatternVisitor : CSharpSyntaxVisitor
    {
        private readonly SyntaxTree _currentTree;

        public StatePatternVisitor(string filePath)
        {
            _currentTree = ParseProgramFile(filePath);
        }

        public void StartVisiting()
        {
            VisitAllNodes(_currentTree.GetRoot());
        }

        protected void VisitAllNodes(SyntaxNode root)
        {
            VisitChildrenNodes(root);
        }

        protected void VisitChildrenNodes(SyntaxNode node)
        {
            foreach (var childNode in node.ChildNodes())
            {
                Visit(childNode);

                if (childNode.ChildNodes().Any())
                {
                    VisitChildrenNodes(childNode);
                }
            }
        }

        public static SyntaxTree ParseProgramFile(string filePath)
        {
            StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8);
            return CSharpSyntaxTree.ParseText(streamReader.ReadToEnd());
        }
    }
}

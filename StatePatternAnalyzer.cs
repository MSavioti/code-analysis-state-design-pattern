using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TP4
{
    public class StatePatternAnalyzer
    {
        private readonly string[] _contextFiles;
        private readonly string[] _cSharpFiles;
        private readonly List<SyntaxToken> _foundInterfaces;
        private readonly List<ClassDeclarationSyntax> _concreteStates;

        public StatePatternAnalyzer(string[] contextFiles, string[] cSharpFiles)
        {
            _contextFiles = contextFiles;
            _cSharpFiles = cSharpFiles;
            _foundInterfaces = new List<SyntaxToken>();
            _concreteStates = new List<ClassDeclarationSyntax>();
        }

        public void Start()
        {
            Console.WriteLine("_______________________");
            Console.WriteLine("| INICIANDO OPERAÇÕES |");
            Console.WriteLine("_______________________\n");

            foreach (var contextFile in _contextFiles)
            {
                Console.Write("Iniciando análise da classe de contexto localizada em ");
                Console.WriteLine($"\"{contextFile}\"\n");
                AnalyzeFiles(contextFile);
            }
        }

        private void AnalyzeFiles(string contextFilePath)
        {
            if (!AnalyzeInterfaces())
                return;

            StateContextData contextData = VisitContextFile(contextFilePath);
            //Console.WriteLine(contextData);

            if (!AnalyzeUsedInterface(contextData.HasInterfaceField, contextData.UsedInterface))
                return;

            if (!AnalyzeStateSetter(contextData.HasStateSetter, contextData.StateSetter))
                return;

            if (!AnalyzeConcreteStateClasses(contextData.UsedInterface))
                return;

            Console.Write($"A análise foi concluída e a classe localizada em \"{contextFilePath}\" ");
            Console.WriteLine("atende aos requisitos do State Design Pattern.");
        }

        private bool AnalyzeInterfaces()
        {
            Console.WriteLine("Procurando por interfaces nos arquivos fornecidos...");

            foreach (var file in _cSharpFiles)
            {
                List<SyntaxToken> interfaces = VisitStateFile(file);

                if (interfaces.Count <= 0) continue;

                foreach (var i in interfaces.Where(i => !_foundInterfaces.Contains(i)))
                {
                    _foundInterfaces.Add(i);
                }
            }

            if (_foundInterfaces.Count > 0)
            {
                Console.WriteLine("Interfaces encontradas:\n");

                foreach (var i in _foundInterfaces)
                {
                    Console.WriteLine($" - {i}");
                }

                Console.Write("\n");
                return true;
            }
            else
            {
                Console.WriteLine("\nERRO! Nenhuma interface pública encontrada!");
                Console.Write("Para implementar o State Design Pattern é necessário que");
                Console.WriteLine(" exista alguma interface.");
                return false;
            }
        }

        private bool AnalyzeUsedInterface(bool hasInterfaceField, SyntaxToken usedInterface)
        {
            if (!hasInterfaceField)
            {
                Console.WriteLine("ERRO! Classe de contexto fornecida não possui nenhum campo de tipo interface!");
                return false;
            }


            Console.WriteLine("A seguinte interface é um campo da classe de contexto:\n");
            Console.WriteLine($" - {usedInterface}\n");

            return true;
        }

        private bool AnalyzeStateSetter(bool hasStateSetter, MethodDeclarationSyntax stateSetter)
        {
            if (!hasStateSetter)
            {
                Console.Write("ERRO! Classe de contexto não possui um setter de estado público, ");
                Console.WriteLine("impedindo que esta possa alternar entre diferentes estados.");
                return false;
            }

            Console.WriteLine("A classe de contexto possui um setter de estado válido:\n");
            Console.WriteLine($" - {stateSetter.Identifier}\n");
            return true;
        }

        private bool AnalyzeConcreteStateClasses(SyntaxToken usedInterface)
        {
            foreach (var file in _cSharpFiles)
            {
                foreach (var concreteState in VisitConcreteStateFile(file, usedInterface))
                {
                    _concreteStates.Add(concreteState);
                }
            }

            switch (_concreteStates.Count)
            {
                case 0:
                    Console.Write("ERRO! Não foram encontradas classes que implementam a interface");
                    Console.WriteLine(" usada pela classe de contexto.");
                    return false;
                case 1:
                    Console.Write("ERRO! Foi encontrada apenas uma classe que implementa a interface");
                    Console.WriteLine(" usada pela classe de contexto. Desta forma não há como trocar de estados.");
                    return false;
                default:
                    Console.WriteLine("As seguintes classes Concrete States foram encontradas:\n");

                    foreach (var concreteState in _concreteStates)
                    {
                        Console.WriteLine($" - {concreteState.Identifier}");
                    }

                    Console.Write("\n");
                    return true;
            }
        }

        private StateContextData VisitContextFile(string contextFilePath)
        {
            var visitor = new ContextVisitor(contextFilePath);
            return visitor.GetContextData(_foundInterfaces);
        }

        private List<SyntaxToken> VisitStateFile(string filePath)
        {
            var visitor = new StateVisitor(filePath);
            return visitor.GetInterfacesFound();
        }

        private List<ClassDeclarationSyntax> VisitConcreteStateFile(string filePath, SyntaxToken usedInterface)
        {
            var visitor = new ConcreteStateVisitor(filePath);
            return visitor.GetConcreteStates(usedInterface);
        }

        private SyntaxToken GetStateInterface()
        {
            return new SyntaxToken();
        }
    }
}

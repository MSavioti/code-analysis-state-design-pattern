using System;

namespace TP4.Test
{
    public class ErrorState : IProgramState
    {
        public void Run()
        {
            Console.WriteLine("O programa encontrou erros irreversíveis e está finalizando sua execução.");
        }
    }
}

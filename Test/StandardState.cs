using System;

namespace TP4.Test
{
    public class StandardState : IProgramState
    {
        public void Run()
        {
            Console.WriteLine("Entrando em estado de operação normal...");
        }
    }
}

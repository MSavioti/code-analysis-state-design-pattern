using System;

namespace TP4.Test
{
    public class WarningState : IProgramState
    {
        public void Run()
        {
            Console.WriteLine("O programa encontrou erros e entrou em estado de alerta.");
        }
    }
}

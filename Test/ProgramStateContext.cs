namespace TP4.Test
{
    public class ProgramStateContext
    {
        private IProgramState _currentState;

        public ProgramStateContext()
        {
            _currentState = new StandardState();
        }

        public void SetState(IProgramState state)
        {
            _currentState = state;
        }

        public IProgramState GetState()
        {
            return _currentState;
        }

        public void RunState()
        {
            _currentState.Run();
        }
    }
}

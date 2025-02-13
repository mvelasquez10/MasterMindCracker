namespace Engine
{
    public class CrackerEventArgs : EventArgs
    {
        public CrackerEventArgs(string entry, CrackerEvent @event = CrackerEvent.AttemptCreated)
        {
            Entry = entry;
            Event = @event;
        }

        public enum CrackerEvent
        {
            AttemptCreated,
            AttemptUsed,
            AttemptFailed,
            AttemptSucceeded,
            AttemptError
        }

        public string Entry { get; }
        public CrackerEvent Event { get; }
    }
}
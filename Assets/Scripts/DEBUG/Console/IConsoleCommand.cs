namespace DEBUG.Console
{
    public interface IConsoleCommand
    {
        public string CommandWord { get; }
        public bool Execute(string[] args);
    }
}
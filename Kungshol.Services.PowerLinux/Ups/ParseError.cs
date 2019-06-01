namespace Kungshol.Services.PowerLinux.Ups
{
    public class ParseError
    {
        public ParseError(string error)
        {
            Error = error;
        }

        public string Error { get; }
    }
}
namespace Kungshol.Services.PowerLinux.Controllers
{
    public class ParseError
    {
        public string Error { get; }

        public ParseError(string error)
        {
            Error = error;
        }
    }
}
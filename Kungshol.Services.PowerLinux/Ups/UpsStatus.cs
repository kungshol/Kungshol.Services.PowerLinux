using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Serilog;

namespace Kungshol.Services.PowerLinux.Ups
{
    public class UpsStatus
    {
        public UpsStatus(PowerStatus status)
        {
            Succeeded = status != null;

            PowerStatus = status ?? new PowerStatus();
        }

        public string Error { get; private set; }

        public PowerStatus PowerStatus { get; }

        public bool Succeeded { get; }

        public static UpsStatus TryCreate(IEnumerable<string> lines, ILogger logger)
        {
            ImmutableArray<string> actualLines = lines?.ToImmutableArray() ?? ImmutableArray<string>.Empty;

            if (PowerStatus.TryParse(actualLines,
                out PowerStatus status,
                out ImmutableArray<ParseError> parseErrors))
            {
                return new UpsStatus(status);
            }

            logger.Error("Could not get status from lines {Lines}", actualLines);

            return Invalid(string.Join(", ", parseErrors.Select(e => e.Error)));
        }

        public static UpsStatus Invalid(string error)
        {
            return new UpsStatus(null)
            {
                Error = error
            };
        }
    }
}
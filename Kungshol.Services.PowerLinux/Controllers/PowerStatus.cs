using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Kungshol.Services.PowerLinux.Controllers
{
    public class PowerStatus
    {
        private const string BatteryCharge = "battery.charge";

        public double? UpsRealPower { get; set; }

        public double? InputFrequency { get; set; }

        public double? InputVoltage { get; set; }

        public double? BatteryRuntime { get; set; }

        public double? BatteryChargeLevel { get; set; }

        public ImmutableDictionary<string, string> Properties { get; private set; }

        public static bool TryParse(
            IEnumerable<string> values,
            out PowerStatus status,
            out ImmutableArray<ParseError> errors)
        {
            ImmutableArray<string> array = values?.ToImmutableArray() ?? ImmutableArray<string>.Empty;

            if (array.Length == 0)
            {
                status = default;
                errors = new[] { new ParseError("Array is empty") }.ToImmutableArray();
                return false;
            }

            if (!array.Any(line => line.StartsWith($"{BatteryCharge}:", StringComparison.OrdinalIgnoreCase)))
            {
                errors = new[] { new ParseError($"Missing property {BatteryCharge}") }.ToImmutableArray();
                status = default;
                return false;
            }

            string PropertyValue(string lookup)
            {
                string[] matchingLines = array
                    .Where(line => !string.IsNullOrWhiteSpace(line)
                                   && line.StartsWith($"{lookup}:", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (matchingLines.Length != 1)
                {
                    return null;
                }

                string propertyAndValue = matchingLines[0];

                if (!propertyAndValue.Contains(':'))
                {
                    return null;
                }

                string[] strings = propertyAndValue.Split(':');

                if (strings.Length != 2)
                {
                    return null;
                }

                return strings[1].Trim();
            }

            double? NumericValue(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                if (int.TryParse(value, out int numericInteger))
                {
                    return numericInteger;
                }

                if (!double.TryParse(value, out double numeric))
                {
                    return null;
                }

                return numeric;
            }

            ImmutableDictionary<string, string> properties = array
                .Where(line => line.Contains(".") && line.Contains(":"))
                .Select(line =>
                {
                    string[] parts = line.Split(':');

                    if (parts.Length != 2)
                    {
                        return default;
                    }

                    return new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim());
                })
                .Where(keyValuePair => keyValuePair.Key != null)
                .ToImmutableDictionary(keyValuePair => keyValuePair.Key, s => s.Value);

            status = new PowerStatus
            {
                BatteryChargeLevel = NumericValue(PropertyValue(BatteryCharge)),
                BatteryRuntime = NumericValue(PropertyValue("battery.runtime")),
                InputVoltage = NumericValue(PropertyValue("input.voltage")),
                InputFrequency = NumericValue(PropertyValue("input.frequency")),
                UpsRealPower = NumericValue(PropertyValue("ups.realpower")),
                Properties = properties
            };

            return true;
        }
    }
}
using Kungshol.Services.PowerLinux.Controllers;
using Kungshol.Services.PowerLinux.Ups;
using Serilog.Core;
using Xunit;

namespace Kungshol.Services.PowerLinux.Tests.Integration
{
    public class UpsStatusTests
    {
        [Fact]
        public void TryCreateShouldSucceedWithData()
        {
            string[] data = { "abc", "battery.charge: 100", "battery.charge.low: 20" };
            UpsStatus upsStatus = UpsStatus.TryCreate(data, Logger.None);

            Assert.True(upsStatus.Succeeded);
        }
    }
}
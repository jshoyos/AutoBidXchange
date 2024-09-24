using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionService.IntegrationTests.Fixtures
{
    [CollectionDefinition("SharedCollection")]
    public class SharedFixture : ICollectionFixture<CustomWebAppFactory>
    {
    }
}

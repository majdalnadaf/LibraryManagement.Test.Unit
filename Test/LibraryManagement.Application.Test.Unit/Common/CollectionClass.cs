using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Test.Unit.Common
{
    [CollectionDefinition("My Collection Fixture Class")]
    public class CollectionClass : ICollectionFixture<MyFixtureClass>
    {
    }
}

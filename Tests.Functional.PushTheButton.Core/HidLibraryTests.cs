using System.Linq;
using NUnit.Framework;

namespace Tests.Functional.PushTheButton.Core
{
    [TestFixture]
    public class HidLibraryTests
    {
        [Test]
        public void ShouldFindOneOrMoreHidDevices()
        {
            var devices = HidLibrary.HidDevices.Enumerate().ToList();
            Assert.That(devices.Count(), Is.GreaterThan(0));
        }
    }
}
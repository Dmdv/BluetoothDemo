using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    internal class DeviceFix : IEqualityComparer<DeviceFix>
    {
        public DeviceFix(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public bool Equals(DeviceFix x, DeviceFix y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(DeviceFix obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void AssumeFilteringIsOk()
        {
            // Use Autofixture instead

            var dev1 = new DeviceFix("Sample1");
            var dev2 = new DeviceFix("Sample2");
            var dev3 = new DeviceFix("Sample3");

            var watched = new[] {dev1, dev2, dev3};
            var listed = new[] {dev1, dev3};

            var devices = watched.Except(listed).ToArray();

            devices.Length.Should().Be(1);
            devices.First().Name.Should().Be(dev2.Name);
        }
    }
}
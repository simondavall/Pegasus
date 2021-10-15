using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pegasus.Extensions;

namespace PegasusTests.ExtensionTests
{
    // ReSharper disable once InconsistentNaming
    class IEnumerableExtensionTests
    {
        private int _numberOfItems;
        private int _page;
        private int _pagesize;
        private bool _paginationEnabled;
        private List<int> _list;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _numberOfItems = 40;
            _list = new List<int>();
            for (var i = 0; i < _numberOfItems; i++)
            {
                _list.Add(i);
            }
        }

        [SetUp]
        public void TestSetup()
        {
            _page = 1;
            _pagesize = 20;
            _paginationEnabled = true;
        }

        [Test]
        public void Paginated_Disabled_ReturnsFullList()
        {
            _paginationEnabled = false;

            var sut = _list.Paginated(_page, _pagesize, _paginationEnabled);

            Assert.AreEqual(_numberOfItems, sut.Count());
        }

        [Test]
        public void Paginated_PageSize10_Returns10ItemsReturned()
        {
            _pagesize = 10;

            var sut = _list.Paginated(_page, _pagesize, _paginationEnabled);

            Assert.AreEqual(_pagesize, sut.Count());
        }

        [Test]
        public void Paginated_Page2Requested_ReturnsPage2()
        {
            _page = 2;

            var sut = _list.Paginated(_page, _pagesize, _paginationEnabled).ToList();

            Assert.AreEqual(_pagesize, sut.Count());
            Assert.AreEqual(20, sut.ToList()[0]);
        }
    }
}

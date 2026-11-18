using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndependentWork7.Tests;

[TestClass]
public class CacheTests
{
    [TestMethod]
    public void Add_AddsItemToCache()
    {
        var cache = new Cache<string, int>(2);

        cache.Add("one", 1);

        Assert.IsTrue(cache.ContainsKey("one"));
        Assert.AreEqual(1, cache.Get("one"));
    }

    [TestMethod]
    public void Get_ReturnsValueForExistingKey()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("one", 1);

        Assert.AreEqual(1, cache.Get("one"));
    }

    [TestMethod]
    public void Add_WhenCapacityIsExceeded_EvictsOldestItem()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("one", 1);
        cache.Add("two", 2);

        cache.Add("three", 3);

        Assert.IsFalse(cache.ContainsKey("one"));
        Assert.IsTrue(cache.ContainsKey("two"));
        Assert.IsTrue(cache.ContainsKey("three"));
        Assert.AreEqual(2, cache.Count);
    }

    [TestMethod]
    public void Get_WhenKeyDoesNotExist_ThrowsKeyNotFoundException()
    {
        var cache = new Cache<string, int>(2);

        Assert.ThrowsException<KeyNotFoundException>(() => cache.Get("missing"));
    }

    [TestMethod]
    public void ContainsKey_ReturnsWhetherKeyExists()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("one", 1);

        Assert.IsTrue(cache.ContainsKey("one"));
        Assert.IsFalse(cache.ContainsKey("missing"));
    }

    [TestMethod]
    public void Count_ReflectsCurrentNumberOfItems()
    {
        var cache = new Cache<string, int>(2);

        Assert.AreEqual(0, cache.Count);
        cache.Add("one", 1);
        Assert.AreEqual(1, cache.Count);
        cache.Add("two", 2);
        cache.Add("three", 3);
        Assert.AreEqual(2, cache.Count);
    }

    [TestMethod]
    public void Add_WhenKeyAlreadyExists_UpdatesValueWithoutChangingFifoOrder()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("one", 1);
        cache.Add("two", 2);

        cache.Add("one", 10);
        Assert.AreEqual(10, cache.Get("one"));
        cache.Add("three", 3);

        Assert.IsFalse(cache.ContainsKey("one"));
        Assert.IsTrue(cache.ContainsKey("two"));
    }

    [TestMethod]
    public void Constructor_WhenCapacityIsNotPositive_ThrowsArgumentOutOfRangeException()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Cache<string, int>(0));
    }
}

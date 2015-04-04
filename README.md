# CacheManager

[![Build Status](https://travis-ci.org/MichaCo/CacheManager.svg?branch=master)](https://travis-ci.org/MichaCo/CacheManager)

The purpose of this project is 

* to define a common interface for different cache providers and techniques
* to create an abstraction layer between your application logic and cache to make it easier to switch different cache strategies later 
For example starting with a simple in-process cache and switching to a more complex distributed cache layer later.
* to enable the use of multiple caches layers on top of each other.   
A common scenario would be using a distributed cache, and to make read access faster on each node, have an in-process cache on top of it.  
* additional features like region support, events, cross server invalidation/sync, statistics, performance counters etc...

## Packages

| Package | .Net 4.0 | .Net 4.5
----------|----------|------------
| [CacheManager.Core] [Core.nuget] | x | x
| [CacheManager.StackExchange.Redis] [Redis.nuget] | x | x 
| [CacheManager.SystemRuntimeCaching] [SystemRuntimeCaching.nuget]  | x | x 
| [CacheManager.AppFabricCache] [AppFabricCache.nuget]  | - | x 
| [CacheManager.WindowsAzureCaching] [WindowsAzureCaching.nuget]  | - | x 
| [CacheManager.Memcached] [Memcached.nuget]  | x | x
| [CacheManager.Web] [Web.nuget]  | - | x
| [CacheManager.Couchbase] [Couchbase.nuget]  | - | x

## Getting Started
The easiest way to get started is to

* Create an empty console application in Visual Studio
* Install the [CacheManager.SystemRuntimeCaching](https://www.nuget.org/packages/CacheManager.SystemRuntimeCaching ) nuget package (which will also install the core package).
The Nuget will update the app.config of your project and will add the available handles and a sample cache configuration.

Now you can use the configured cache "myCache" by loading the configuration using the `CacheFactory`.
As you can see, the name of the cacheManager/managers/cache element within app.config must match with the parameter you pass to `CacheFactory.FromConfiguration`.

    class Program
    {
        static void Main(string[] args)
        {
            var cache = CacheFactory.FromConfiguration<object>("myCache");
            cache.Add("key", "value");
            var value = cache.Get("key");
        }
    }

## Implemented Features

### Version 0.4

* One common interface for handling different caching technologies: `ICache<T>`
* Configurable via app/web.config or by code.
* Support for different caches
    * **MemoryCache** (System.Runtime.Caching)
    * **Redis** using [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis)
    * **Memcached** using [Enyim.Memcached](https://github.com/enyim/EnyimMemcached)
    * **Couchbase** using [Couchbase.Net.Client v2](https://github.com/couchbase/couchbase-net-client)
    * AppFabric cache (might get descoped)
    * Azure cache (might get descoped)
* **Update values with lock or transaction** for distributed caches. 
The interfaced provides a simple update method which internally ensures you work with the latest version.
And CacheManager handles version conflicts for you.
* **Strongly typed** cache interface.
* **Multiple layers**
By having multiple cache handles managed by CacheManager, you can easily implement layered caches. For example an in process cache infront of your distributed cache, to make read access faster.
CacheManager will synchronize those layers for you. 
    * `Put` and `Add` operations will always be excecuted on all cache handles registered on the manager.
    * On `Get`, there are different configuration options defined by `CacheUpdateMode`, if the item was available in one cache handle:
        * None: No update across the cache handles on Get
        * Up: Updates the handles "above"
        * All: Updates/Adds the item to all handles
* **Cache Regions**: Even if some cache systems do not support or implement cache regions, the CacheManager implements the mechanism for you.
* **Statistics**: Counters for all kind of cache actions, including counters per region
* **Performance Counters**: To be able to inspect certain numbers with perfmon, CacheManager supports performance counters per instance of the manager and per cache handle.
* **Event System**: CacheManager triggers events for common cache actions:
OnGet, OnAdd, OnPut, OnRemove, OnClear, OnClearRegion
* **System.Web.OutputCache** implementation to use CacheManager as OutputCache provider which makes the cache extremly flexible, for example by using a distributed cache like Redis across many web servers.
* **Cache clients synchronization** 
    * Implemented with the Redis cache handle using Redis' pub/sub
    * (Other implementations without Redis might be an option for a later version)

[Core.nuget]: https://www.nuget.org/packages/CacheManager.Core
[Redis.nuget]: https://www.nuget.org/packages/CacheManager.StackExchange.Redis 
[SystemRuntimeCaching.nuget]: https://www.nuget.org/packages/CacheManager.SystemRuntimeCaching
[AppFabricCache.nuget]: https://www.nuget.org/packages/CacheManager.AppFabricCache
[WindowsAzureCaching.nuget]: https://www.nuget.org/packages/CacheManager.WindowsAzureCaching
[Memcached.nuget]: https://www.nuget.org/packages/CacheManager.Memcached
[Web.nuget]: https://www.nuget.org/packages/CacheManager.Web
[Couchbase.nuget]: https://www.nuget.org/packages/CacheManager.Couchbase


 
 
 
 
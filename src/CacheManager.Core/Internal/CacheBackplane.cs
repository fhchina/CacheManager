﻿using System;
using static CacheManager.Core.Utility.Guard;

namespace CacheManager.Core.Internal
{
    /// <summary>
    /// In CacheManager, a cache backplane is used to keep in process and distributed caches in
    /// sync. <br/> If the cache manager runs inside multiple nodes or applications accessing the
    /// same distributed cache, and an in process cache is configured to be in front of the
    /// distributed cache handle. All Get calls will hit the in process cache. <br/> Now when an
    /// item gets removed for example by one client, all other clients still have that cache item
    /// available in the in process cache. <br/> This could lead to errors and unexpected behavior,
    /// therefore a cache backplane will send a message to all other cache clients to also remove
    /// that item.
    /// <para>
    /// The same mechanism will apply to any Update, Put, Remove, Clear or ClearRegion call of the cache.
    /// </para>
    /// </summary>
    public abstract class CacheBackplane : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheBackplane" /> class.
        /// </summary>
        /// <param name="configuration">The cache manager configuration.</param>
        /// <exception cref="System.ArgumentNullException">If configuration is null.</exception>
        protected CacheBackplane(CacheManagerConfiguration configuration)
        {
            NotNull(configuration, nameof(configuration));
            this.CacheConfiguration = configuration;
            this.ConfigurationKey = configuration.BackplaneConfigurationKey;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CacheBackplane"/> class.
        /// </summary>
        ~CacheBackplane()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// The event gets fired whenever a change message for a key comes in,
        /// which means, another client changed a key.
        /// </summary>
        public event EventHandler<CacheItemChangedEventArgs> Changed;

        /// <summary>
        /// The event gets fired whenever a cache clear message comes in.
        /// </summary>
        public event EventHandler<EventArgs> Cleared;

        /// <summary>
        /// The event gets fired whenever a clear region message comes in.
        /// </summary>
        public event EventHandler<RegionEventArgs> ClearedRegion;

        /// <summary>
        /// The event gets fired whenever a removed message for a key comes in.
        /// </summary>
        public event EventHandler<CacheItemEventArgs> Removed;

        /// <summary>
        /// Gets the cache configuration.
        /// </summary>
        /// <value>
        /// The cache configuration.
        /// </value>
        public CacheManagerConfiguration CacheConfiguration { get; }

        /// <summary>
        /// Gets the name of the configuration to be used.
        /// <para>The key might be used to find cache vendor specific configuration.</para>
        /// </summary>
        /// <value>The configuration key.</value>
        public string ConfigurationKey { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Notifies other cache clients about a changed cache key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="action">The action.</param>
        public abstract void NotifyChange(string key, CacheItemChangedEventAction action);

        /// <summary>
        /// Notifies other cache clients about a changed cache key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        /// <param name="action">The action.</param>
        public abstract void NotifyChange(string key, string region, CacheItemChangedEventAction action);

        /// <summary>
        /// Notifies other cache clients about a cache clear.
        /// </summary>
        public abstract void NotifyClear();

        /// <summary>
        /// Notifies other cache clients about a cache clear region call.
        /// </summary>
        /// <param name="region">The region.</param>
        public abstract void NotifyClearRegion(string region);

        /// <summary>
        /// Notifies other cache clients about a removed cache key.
        /// </summary>
        /// <param name="key">The key.</param>
        public abstract void NotifyRemove(string key);

        /// <summary>
        /// Notifies other cache clients about a removed cache key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        public abstract void NotifyRemove(string key, string region);

        /// <summary>
        /// Sends a changed message for the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="action">The action.</param>
        protected internal void TriggerChanged(string key, CacheItemChangedEventAction action)
        {
            this.Changed?.Invoke(this, new CacheItemChangedEventArgs(key, action));
        }

        /// <summary>
        /// Sends a changed message for the given <paramref name="key"/> in <paramref name="region"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        /// <param name="action">The action.</param>
        protected internal void TriggerChanged(string key, string region, CacheItemChangedEventAction action)
        {
            this.Changed?.Invoke(this, new CacheItemChangedEventArgs(key, region, action));
        }

        /// <summary>
        /// Sends a cache cleared message.
        /// </summary>
        protected internal void TriggerCleared()
        {
            this.Cleared?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Sends a region cleared message for the given <paramref name="region"/>.
        /// </summary>
        /// <param name="region">The region.</param>
        protected internal void TriggerClearedRegion(string region)
        {
            this.ClearedRegion?.Invoke(this, new RegionEventArgs(region));
        }

        /// <summary>
        /// Sends a removed message for the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key</param>
        protected internal void TriggerRemoved(string key)
        {
            this.Removed?.Invoke(this, new CacheItemEventArgs(key));
        }

        /// <summary>
        /// Sends a removed message for the given <paramref name="key"/> in <paramref name="region"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        protected internal void TriggerRemoved(string key, string region)
        {
            this.Removed?.Invoke(this, new CacheItemEventArgs(key, region));
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="managed">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
        /// only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool managed)
        {
        }
    }

    /// <summary>
    /// Arguments for the region cleared event
    /// </summary>
    public class RegionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionEventArgs" /> class.
        /// </summary>
        /// <param name="region">The region.</param>
        public RegionEventArgs(string region)
        {
            NotNull(region, nameof(region));
            this.Region = region;
        }

        /// <summary>
        /// Gets the region which got cleared.
        /// </summary>
        /// <value>The region.</value>
        public string Region { get; }
    }

    /// <summary>
    /// Base cache events arguments.
    /// </summary>
    public class CacheItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItemEventArgs" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public CacheItemEventArgs(string key)
        {
            NotNull(key, nameof(key));
            this.Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItemEventArgs" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        public CacheItemEventArgs(string key, string region)
            : this(key)
        {
            NotNull(region, nameof(region));
            this.Region = region;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the region.
        /// </summary>
        public string Region { get; }
    }

    /// <summary>
    /// The enum defines the actual operation used to change the value in the cache.
    /// </summary>
    public enum CacheItemChangedEventAction
    {
        /// <summary>
        /// Default value is invalid to ensure we are not getting wrong results.
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// If Put was used to change the value.
        /// </summary>
        Put,
        /// <summary>
        /// If Add was used to change the value.
        /// </summary>
        Add,
        /// <summary>
        /// If Update was used to change the value.
        /// </summary>
        Update
    }

    /// <summary>
    /// Arguments for cache change events.
    /// </summary>
    public class CacheItemChangedEventArgs : CacheItemEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItemChangedEventArgs" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="action">The cache action.</param>
        public CacheItemChangedEventArgs(string key, CacheItemChangedEventAction action)
            : base(key)
        {
            this.Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItemChangedEventArgs" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        /// <param name="action">The cache action.</param>
        public CacheItemChangedEventArgs(string key, string region, CacheItemChangedEventAction action)
            : base(key, region)
        {
            this.Action = action;
        }

        /// <summary>
        /// Gets the action used to change a key in the cache.
        /// </summary>
        public CacheItemChangedEventAction Action { get; }
    }
}
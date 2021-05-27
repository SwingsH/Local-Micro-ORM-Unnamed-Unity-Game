using System;
using System.Collections.Generic;
using UnityEngine;

namespace TIZSoft.UnityHTTP.Client
{
    /// <summary>
    /// 表示一個 host 管理器，提供 key-value 模式的 host 管理。
    /// </summary>
    public class ClientHostManager
    {
        [Serializable]
        public class Settings
        {
            [Serializable]
            public class Entry
            {
                [Tooltip("伺服器 ID")]
                public string HostId;

                [Tooltip("伺服器 URL。e.g. http://app.domain.com")]
                public string Host;
            }

            [Tooltip("預設要使用的伺服器 ID")]
            public string DefaultHostId;

            [Tooltip("伺服器列表")]
            public List<Entry> Hosts;
        }

        static readonly Utils.Log.Logger logger = Utils.Log.LogManager.Default.FindOrCreateLogger<ClientHostManager>();

        readonly Dictionary<string, string> hosts = new Dictionary<string, string>();

        string currentHostId;

        /// <summary>
        /// Gets or sets the current host identifier.
        /// </summary>
        /// <value>The current host identifier.</value>
        public string CurrentHostId
        {
            get { return currentHostId; }
            set
            {
                if (!ContainsHostId(value))
                {
                    throw new KeyNotFoundException(string.Format("Host ID {0} does not exist.", value));
                }

                currentHostId = value;
            }
        }

        /// <summary>
        /// Gets the current host by <see cref="CurrentHostId"/>.
        /// </summary>
        /// <value>The current host.</value>
        public string CurrentHost
        {
            get { return FindHost(CurrentHostId); }
        }

        /// <summary>
        /// Gets the count of hosts.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return hosts.Count; }
        }

        public ClientHostManager()
        {
            // Do nothing.
        }

        public ClientHostManager(Settings settings)
        {
            //ExceptionUtils.VerifyArgumentNull(settings, "settings");
            AddHosts(settings.Hosts);
            CurrentHostId = settings.DefaultHostId;
        }

        /// <summary>
        /// Finds the host by host ID.
        /// </summary>
        /// <returns>The host.</returns>
        /// <param name="hostId">Host identifier.</param>
        public string FindHost(string hostId)
        {
            string host;
            TryFindHost(hostId, out host);
            return host;
        }

        /// <summary>
        /// Tries find the host by host ID.
        /// </summary>
        /// <returns><c>true</c>, if host was found, <c>false</c> otherwise.</returns>
        /// <param name="hostId">Host identifier.</param>
        /// <param name="host">Host.</param>
        public bool TryFindHost(string hostId, out string host)
        {
            return hosts.TryGetValue(hostId, out host);
        }

        /// <summary>
        /// Finds the host identifier by host.
        /// </summary>
        /// <returns>The host identifier.</returns>
        /// <param name="host">Host.</param>
        public string FindHostId(string host)
        {
            string hostId;
            TryFindHostId(host, out hostId);
            return hostId;
        }

        /// <summary>
        /// Tries find the host identifier by host.
        /// </summary>
        /// <returns><c>true</c>, if host identifier was found, <c>false</c> otherwise.</returns>
        /// <param name="host">Host.</param>
        /// <param name="hostId">Host identifier.</param>
        public bool TryFindHostId(string host, out string hostId)
        {
            foreach (var pair in hosts)
            {
                if (pair.Value == host)
                {
                    hostId = pair.Key;
                    return true;
                }
            }

            hostId = null;
            return false;
        }

        /// <summary>
        /// Finds all hosts.
        /// </summary>
        /// <returns>All hosts.</returns>
        public IEnumerable<KeyValuePair<string, string>> FindAllHosts()
        {
            return FindAllHosts(null);
        }

        /// <summary>
        /// Finds all hosts by predicate.
        /// </summary>
        /// <returns>All matched hosts.</returns>
        public IEnumerable<KeyValuePair<string, string>> FindAllHosts(Predicate<KeyValuePair<string, string>> predicate)
        {
            if (predicate == null)
            {
                return hosts;
            }

            var matches = new List<KeyValuePair<string, string>>();
            foreach (var pair in hosts)
            {
                if (predicate(pair))
                {
                    matches.Add(pair);
                }
            }
            return matches;
        }

        public bool ContainsHostId(string hostId)
        {
            return hosts.ContainsKey(hostId);
        }

        public bool ContainsHost(string host)
        {
            return hosts.ContainsValue(host);
        }

        public void AddHost(Settings.Entry entry)
        {
            AddHost(entry, false);
        }

        public void AddHost(Settings.Entry entry, bool canReplace)
        {
            //ExceptionUtils.VerifyArgumentNull(entry, "entry");
            AddHost(entry.HostId, entry.Host, canReplace);
        }

        public void AddHost(string hostId, string host)
        {
            AddHost(hostId, host, false);
        }

        public void AddHost(string hostId, string host, bool canReplace)
        {
            //ExceptionUtils.VerifyArgumentNullOrEmpty(hostId, "hostId");
            //ExceptionUtils.VerifyArgumentNullOrEmpty(host, "host");

            if (canReplace)
            {
                Debug.AssertFormat(!hosts.ContainsKey(hostId), "Host ID {0} already exists with value {1}.", hostId, hosts[hostId]);
                hosts[hostId] = host;
            }
            else
            {
                hosts.Add(hostId, host);
            }

            if (Count == 1)
            {
                CurrentHostId = hostId;
            }
        }

        public void AddHosts(IEnumerable<Settings.Entry> entries)
        {
            //ExceptionUtils.VerifyArgumentNull(entries, "entries");

            foreach (var entry in entries)
            {
                if (entry == null)
                {
                    logger.Warn("Found a null host entry.");
                    continue;
                }

                try
                {
                    AddHost(entry.HostId, entry.Host);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Add host {{\"{0}\":\"{1}\"}} failed.", entry.HostId, entry.Host);
                }
            }
        }

        public void AddHosts(IEnumerable<KeyValuePair<string, string>> hosts)
        {
            foreach (var pair in hosts)
            {
                try
                {
                    AddHost(pair.Key, pair.Value);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Add host {{\"{0}\":\"{1}\"}} failed.", pair.Key, pair.Value);
                }
            }
        }

        public bool RemoveHostById(string hostId)
        {
            return hosts.Remove(hostId);
        }

        public bool RemoveHostByValue(string host)
        {
            string hostId;
            return TryFindHostId(host, out hostId) && RemoveHostById(hostId);
        }

        public void ClearAllHosts()
        {
            hosts.Clear();
        }

        public void SetHost(string hostId, string host)
        {
            hosts[hostId] = host;
        }
    }
}
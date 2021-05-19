using System;
using Tizsoft.Utils;
using UniRx;
using UnityEngine;

namespace Tizsoft.Net
{
    using ServerNode = ReactiveCollection<string>;
    using ServerGroup = ReactiveDictionary<ServerType, ReactiveCollection<string>>;
    using ServerTable = ReactiveDictionary<string, ReactiveDictionary<ServerType, ReactiveCollection<string>>>;

    /// <summary>
    /// 表示一組網路設定資料。
    /// </summary>
    [Serializable]
    public class ClientHTTPNetwork
    {
        public class ChangedEventArgs
        {
            public ClientHTTPNetwork Sender { get; private set; }

            public string GroupName { get; private set; }

            public ServerType ServerType { get; private set; }

            public int ServerIndex { get; private set; }

            public string ServerUrl { get; private set; }

            public ChangedEventArgs(
                ClientHTTPNetwork sender,
                string groupName,
                ServerType serverType,
                int serverIndex,
                string serverUrl)
            {
                Sender = sender;
                GroupName = groupName;
                ServerType = serverType;
                ServerIndex = serverIndex;
                ServerUrl = serverUrl;
            }
        }

        const int DefaultServerIndex = 0;

        [SerializeField]
        StringReactiveProperty currentGroupId = new StringReactiveProperty();

        /// <summary>
        /// {
        ///     "group_id": {
        ///         "server_type": {
        ///             "urls": [
        ///                 "url[0]",
        ///                 "url[1]",
        ///                 "url[n]"
        ///             ]
        ///         }
        ///     }
        /// }
        /// </summary>
        readonly ServerTable serverTable = new ServerTable();

        readonly Subject<ChangedEventArgs> serverAdded = new Subject<ChangedEventArgs>();
        readonly Subject<ChangedEventArgs> serverChanged = new Subject<ChangedEventArgs>();

        /// <summary>
        /// 取得或設定目前的伺服器群組 ID。。
        /// </summary>
        //public ReactiveProperty<string> CurrentGroupId
        public ReactiveProperty<string> CurrentGroupId
        {
            get { return currentGroupId; }
        }

        public IObservable<ChangedEventArgs> ServerAdded
        {
            get { return serverAdded.AsObservable(); }
        }

        public IObservable<ChangedEventArgs> ServerChanged
        {
            get { return serverChanged.AsObservable(); }
        }


        public string FindDefaultCurrentServer(ServerType serverType)
        {
            return FindCurrentServer(serverType, DefaultServerIndex);
        }

        public string FindCurrentServer(ServerType serverType, int index)
        {
            return FindServer(CurrentGroupId.Value, serverType, index);
        }
        
        public string FindServer(string groupName, ServerType serverType, int index)
        {
            ServerGroup serverGroup;
            if (serverTable.TryGetValue(groupName, out serverGroup))
            {
                ServerNode serverNode;
                if (serverGroup.TryGetValue(serverType, out serverNode))
                {
                    ExceptionUtils.VerifyIndex(serverNode, index);
                    return serverNode[index];
                }
            }
            return string.Empty;
        }

        public void AddServer(string groupName, ServerType serverType, string url)
        {
            EnsureGroupAndNode(groupName, serverType);
            serverTable[groupName][serverType].Add(url);
            RaiseServerAdded(groupName, serverType, serverTable[groupName][serverType].Count - 1, url);
        }

        public void SetDefaultCurrentServer(ServerType serverType, string url)
        {
            SetServer(CurrentGroupId.Value, serverType, DefaultServerIndex, url);
        }

        public void SetDefaultServer(string groupName, ServerType serverType, string url)
        {
            SetServer(groupName, serverType, DefaultServerIndex, url);
        }

        public void SetServer(string groupName, ServerType serverType, int index, string url)
        {
            EnsureGroupAndNode(groupName, serverType);
            var node = serverTable[groupName][serverType];
            EnsureNodeSize(node, index + 1);
            node[index] = url;
            RaiseServerChanged(groupName, serverType, index, url);
        }

        public bool RemoveGroup(string groupName)
        {
            return serverTable.Remove(groupName);
        }

        void EnsureGroupAndNode(string groupName, ServerType serverType)
        {
            EnsureGroup(groupName);
            EnsureNode(serverTable[groupName], serverType);
        }

        void EnsureGroup(string groupName)
        {
            if (serverTable.ContainsKey(groupName))
            {
                return;
            }

            serverTable.Add(groupName, new ServerGroup());
        }

        static void EnsureNode(ServerGroup serverGroup, ServerType serverType)
        {
            if (serverGroup.ContainsKey(serverType))
            {
                return;
            }

            serverGroup.Add(serverType, new ServerNode());
        }

        static void EnsureNodeSize(ServerNode serverNode, int count)
        {
            while (serverNode.Count < count)
            {
                serverNode.Add(string.Empty);
            }
        }

        void RaiseServerAdded(string groupName, ServerType serverType, int serverIndex, string serverUrl)
        {
            serverAdded.OnNext(new ChangedEventArgs(this, groupName, serverType, serverIndex, serverUrl));
        }

        void RaiseServerChanged(string groupName, ServerType serverType, int serverIndex, string serverUrl)
        {
            serverChanged.OnNext(new ChangedEventArgs(this, groupName, serverType, serverIndex, serverUrl));
        }
    }
}
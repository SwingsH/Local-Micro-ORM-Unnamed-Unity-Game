using System;
using UniRx;
using UnityEngine;
using TIZSoft.UnityHTTP.Client;

namespace TIZSoft.UnknownGame
{
    /// <summary>
    /// 表示一組遊戲資料。
    /// </summary>
    [Serializable]
    public class Game
    {
        [SerializeField]
        ClientHTTPNetwork network = new ClientHTTPNetwork();

        [SerializeField]
        User localUser = new User();

        /// <summary>
        /// 取得網路設定。
        /// </summary>
        public ReadOnlyReactiveProperty<ClientHTTPNetwork> Network { get; private set; }

        /// <summary>
        /// 取得本地端玩家 runtime data。
        /// </summary>
        public ReadOnlyReactiveProperty<User> LocalUser { get; private set; }

        public Game()
        {
            Network = new ReactiveProperty<ClientHTTPNetwork>(network).ToReadOnlyReactiveProperty();
            LocalUser = new ReactiveProperty<User>(localUser).ToReadOnlyReactiveProperty();
        }
    }
}

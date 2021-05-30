using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace TIZSoft.UnknownGame
{
    /// <summary>
    /// 表示一個玩家資料。
    /// </summary>
    [Serializable]
    public class User
    {
        [SerializeField]
        IntReactiveProperty id = new IntReactiveProperty();

        [SerializeField]
        StringReactiveProperty name = new StringReactiveProperty();

        [SerializeField]
        StringReactiveProperty accessToken = new StringReactiveProperty();

        [SerializeField]
        StringReactiveProperty wasabiiUserId = new StringReactiveProperty();

        [SerializeField]
        IntReactiveProperty practiceDoubleCount = new IntReactiveProperty();

        [SerializeField]
        IntReactiveProperty giftCount = new IntReactiveProperty();

        [SerializeField]
        IntReactiveProperty lunchCount = new IntReactiveProperty();

        [SerializeField]
        IntReactiveProperty niaInfoCount = new IntReactiveProperty();

        [SerializeField]
        IntReactiveProperty stamina = new IntReactiveProperty();

        /// <summary>
        /// 取得或設定玩家 ID。
        /// </summary>
        public ReactiveProperty<int> Id
        {
            get { return id; }
        }

        /// <summary>
        /// 取得或設定玩家名稱。
        /// </summary>
        public ReactiveProperty<string> Name
        {
            get { return name; }
        }

        /// <summary>
        /// 取得或設定玩家的 access token。
        /// </summary>
        public ReactiveProperty<string> AccessToken
        {
            get { return accessToken; }
        }

        public ReactiveProperty<string> WasabiiUserId
        {
            get { return wasabiiUserId; }
        }

        /// <summary>
        /// 取得判斷玩家是否已登入。
        /// </summary>
        public bool IsLoggedIn
        {
            get { return Id.Value != 0 && !string.IsNullOrEmpty(AccessToken.Value); }
        }

        //public void UpdateItem(NetMessages.Json..Item itemInfos, ItemDataRepository itemData
        public void UpdateItem()
        {
        }

        public int GetItemQuantity(int itemId)
        {
            //Item item;
            //var result = 0;
            //if (items.TryGetValue(itemId, out item))
            //{
            //    Assert.IsNotNull(item);
            //    result = item.Quantity.Value;
            //}
            //return result;
            return default;
        }
    }
}

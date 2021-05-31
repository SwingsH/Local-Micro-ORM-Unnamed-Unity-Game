using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using TIZSoft.SaveData;
using TIZSoft.Utils;
using UnityEngine;

namespace TIZSoft.UnknownGame.SaveData
{
    /// <summary>
    /// 遊戲存檔。
    /// </summary>
    [Serializable]
    public class GameSave : Save
    {
        public const string DefaultGroupName = "default";

        /// <summary>
        /// 存檔群組。
        /// </summary>
        [Serializable]
        public class Group
        {
            [SerializeField]
            [Tooltip("存檔群組名稱。")]
            string groupName = DefaultGroupName;

            [JsonProperty]
            [SerializeField]
            [Tooltip("玩家存檔。")]
            UserSave userSave = new UserSave();

            [JsonProperty]
            [SerializeField]
            [Tooltip("系統存檔。")]
            SystemSave systemSave = new SystemSave();
            
            /// <summary>
            /// 取得或設定存檔群組名稱。
            /// </summary>
            [JsonProperty("groupName")]
            public string GroupName
            {
                get { return groupName; }
                set { groupName = value; }
            }

            /// <summary>
            /// 取得玩家存檔。
            /// </summary>
            [JsonIgnore]
            public UserSave UserSave
            {
                get { return userSave; }
            }

            /// <summary>
            /// 取得系統存檔。
            /// </summary>
            [JsonIgnore]
            public SystemSave SystemSave
            {
                get { return systemSave; }
            }
        }

        [SerializeField]
        string currentGroupName;

        [JsonProperty]
        [SerializeField]
        List<Group> groups = new List<Group>();

        /// <summary>
        /// 取得或設定目前要使用的群組名稱。
        /// </summary>
        [JsonProperty("currentGroupName")]
        public string CurrentGroupName
        {
            get { return currentGroupName; }
            set
            {
                var groupNames = GroupNames;
                if (groupNames.Contains(value))
                {
                    currentGroupName = value;
                }
                else if (groupNames.Count > 0)
                {
                    currentGroupName = groupNames[0];
                }
                else
                {
                    currentGroupName = string.Empty;
                }
            }
        }

        public bool GameDataInitialized = false;

        [JsonIgnore]
        public ReadOnlyCollection<Group> Groups
        {
            get { return groups.AsReadOnly(); }
        }

        /// <summary>
        /// 取得所有群組名稱。
        /// </summary>
        [JsonIgnore]
        public List<string> GroupNames
        {
            get
            {
                var groupNames = new List<string>();
                foreach (var group in groups)
                {
                    groupNames.Add(group.GroupName);
                }
                return groupNames;
            }
        }

        /// <summary>
        /// 取得目前的存檔群組。
        /// </summary>
        [JsonIgnore]
        public Group CurrentGroup
        {
            get { return FindGroup(currentGroupName); }
        }

        /// <summary>
        /// 取得目前的玩家存檔資料。
        /// </summary>
        [JsonIgnore]
        public UserSave CurrentUserSave
        {
            get
            {
                var group = CurrentGroup;
                if (group != null)
                {
                    return group.UserSave;
                }
                return null;
            }
        }
        
        /// <summary>
        /// 取得目前的系統存檔資料。
        /// </summary>
        [JsonIgnore]
        public SystemSave CurrentSystemSave
        {
            get
            {
                var group = CurrentGroup;
                if (group != null)
                {
                    return group.SystemSave;
                }
                return null;
            }
        }

        /// <summary>
        /// 回傳指定群組名稱是否存在。
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public bool ContainsGroup(string groupName)
        {
            return groups.Any(group => group.GroupName == groupName);
        }

        /// <summary>
        /// 尋找指定群組。
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public Group FindGroup(string groupName)
        {
            return groups.Find(g => g.GroupName == groupName);
        }

        /// <summary>
        /// 新增群組。group.GroupName 是必須非空值。
        /// </summary>
        /// <param name="group"></param>
        /// <returns>新增失敗時回傳 false，否則回傳 true。</returns>
        public bool AddGroup(Group group)
        {
            ExceptionUtils.VerifyArgumentNull(group, "group");
            ExceptionUtils.VerifyArgumentNullOrEmpty(group.GroupName, "group.GroupName");

            if (ContainsGroup(group.GroupName))
            {
                return false;
            }
            groups.Add(group);
            return true;
        }

        /// <summary>
        /// 移除指定群組。GroupName 是必須非空值。
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns>移除失敗時回傳 false，否則回傳 true。</returns>
        public bool RemoveGroup(string groupName)
        {
            ExceptionUtils.VerifyArgumentNullOrEmpty(groupName, "GroupName");

            var index = groups.FindIndex(g => g.GroupName == groupName);
            if (index < 0)
            {
                return false;
            }
            groups.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// 以給定的群組取代之。group.GroupName 是必須非空值。
        /// </summary>
        /// <param name="group"></param>
        /// <returns>取代失敗時回傳 false，否則回傳 true。</returns>
        public bool ReplaceGroup(Group group)
        {
            ExceptionUtils.VerifyArgumentNull(group, "group");
            ExceptionUtils.VerifyArgumentNullOrEmpty(group.GroupName, "group.GroupName");

            var index = groups.FindIndex(g => g.GroupName == group.GroupName);
            if (index < 0)
            {
                return false;
            }

            groups[index] = group;
            return true;
        }

        /// <summary>
        /// 取代或新增群組。
        /// </summary>
        /// <param name="group"></param>
        /// <returns>新增失敗時回傳 false，否則回傳 true。</returns>
        public bool ReplaceOrAddGroup(Group group)
        {
            ExceptionUtils.VerifyArgumentNull(group, "group");
            ExceptionUtils.VerifyArgumentNullOrEmpty(group.GroupName, "group.GroupName");

            if (ReplaceGroup(group))
            {
                return true;
            }
            return AddGroup(group);
        }
    }
}

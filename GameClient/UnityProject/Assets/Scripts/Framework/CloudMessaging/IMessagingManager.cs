namespace TIZSoft.CloudMessaging
{
    /// <summary>
    /// 定義推播管理員可提供的方法。
    /// </summary>
    /// <remarks>
    /// 有需要其他推播功能再加開 API 即可。
    /// </remarks>
    public interface IMessagingManager
    {
        void SubscribeTopic(string topic);
        void UnsubscribeTopic(string topic);
    }
}

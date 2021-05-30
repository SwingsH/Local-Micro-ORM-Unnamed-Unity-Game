namespace TIZSoft.CloudMessaging
{
    public class MessagingManager : IMessagingManager
    {
        IMessagingManager messagingManager;

        public MessagingManager()
        {
#if Q0061
            var go = new UnityEngine.GameObject(GetType().Name, typeof(FirebaseMessagingManager));
            messagingManager = go.GetComponent<FirebaseMessagingManager>();
#else
            messagingManager = new EmptyMessagingManager();
#endif
        }

        public void SubscribeTopic(string topic)
        {
            messagingManager.SubscribeTopic(topic);
        }

        public void UnsubscribeTopic(string topic)
        {
            messagingManager.UnsubscribeTopic(topic);
        }
    }
}

namespace TIZSoft.CloudMessaging
{
    public class EmptyMessagingManager : IMessagingManager
    {
        public void SubscribeTopic(string topic)
        {
            // Do nothing.
        }

        public void UnsubscribeTopic(string topic)
        {
            // Do nothing.
        }
    }
}

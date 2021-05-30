#if Q0061
using Firebase;
using Firebase.Messaging;
using TIZSoft.Utils.Log;
using UnityEngine;

namespace TIZSoft.CloudMessaging
{
    /// <summary>
    /// 推播管理員實作品，使用 Firebase。
    /// </summary>
    class FirebaseMessagingManager : MonoBehaviour, IMessagingManager
    {
        static readonly Log.Logger logger = LogManager.Default.FindOrCreateCurrentTypeLogger();

        DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

        void Start()
        {
            FirebaseApp
                .CheckAndFixDependenciesAsync()
                .ContinueWith(task =>
                {
                    dependencyStatus = task.Result;
                    if (dependencyStatus == DependencyStatus.Available)
                    {
                        InitializeFirebase();
                    }
                    else
                    {
                        logger.Error("Could not resolve all Firebase dependencies: " + dependencyStatus);
                    }
                });
        }

        void InitializeFirebase()
        {
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
            logger.Debug("Firebase initialized.");
        }

        void OnDestroy()
        {
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
        }

        void OnTokenReceived(object sender, TokenReceivedEventArgs token)
        {
            logger.Debug("Received Registration Token: " + token.Token);
        }

        void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            logger.Debug("Received a new message.");

#if DEBUG
            var notification = e.Message.Notification;
            if (notification != null)
            {
                logger.Debug("title: " + notification.Title);
                logger.Debug("body: " + notification.Body);
            }

            if (e.Message.From.Length > 0)
            {
                logger.Trace("from: " + e.Message.From);
            }

            if (e.Message.Link != null)
            {
                logger.Trace("link: " + e.Message.Link);
            }

            if (e.Message.Data.Count > 0)
            {
                logger.Trace("data:");
                foreach (var iter in e.Message.Data)
                {
                    logger.Trace("  " + iter.Key + ": " + iter.Value);
                }
            }
#endif
        }

        public void SubscribeTopic(string topic)
        {
            FirebaseMessaging.Subscribe(topic);
        }

        public void UnsubscribeTopic(string topic)
        {
            FirebaseMessaging.Unsubscribe(topic);
        }
    }
}
#endif
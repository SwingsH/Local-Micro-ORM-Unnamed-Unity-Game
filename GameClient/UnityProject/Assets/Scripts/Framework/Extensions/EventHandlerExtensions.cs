using System;

namespace TIZSoft.Extensions
{
    public static class EventHandlerExtensions
    {
        public static void Raise(this EventHandler eventHandler, object obj, EventArgs args)
        {
            if (eventHandler != null)
            {
                eventHandler(obj, args);
            }
        }

        public static void Raise(this EventHandler eventHandler, object obj)
        {
            Raise(eventHandler, obj, EventArgs.Empty);
        }
    }
}

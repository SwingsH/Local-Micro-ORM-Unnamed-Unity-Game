using System;
using TIZSoft.Log;

namespace TIZSoft.Extensions
{
    public static class IDisposableExtensions
    {
        static readonly Logger logger = LogManager.Default.FindOrCreateLogger(typeof(IDisposableExtensions).FullName);

        public static void SafeDispose(this IDisposable disposable)
        {
            if (disposable == null)
            {
                return;
            }

            try
            {
                disposable.Dispose();
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }
    }
}

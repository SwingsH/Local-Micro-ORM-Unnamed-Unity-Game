using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace TIZSoft.UniRxExtensions
{
    public class UnityWebRequestErrorException : Exception
    {
        public string RawErrorMessage { get; private set; }
        public bool HasResponse { get; private set; }
        public string Text { get; private set; }
        public int StatusCode { get; private set; }
        public Dictionary<string, string> ResponseHeaders { get; private set; }
        public UnityWebRequest UnityWebRequest { get; private set; }

        // cache the text because if UnityWebRequest was disposed, can't access it.
        public UnityWebRequestErrorException(UnityWebRequest request, string text)
        {
            UnityWebRequest = request;
            RawErrorMessage = request.error;
            ResponseHeaders = request.GetResponseHeaders();
            HasResponse = false;
            Text = text;

            var splitted = RawErrorMessage.Split(' ', ':');
            if (splitted.Length == 0)
            {
                return;
            }

            int statusCode;
            if (int.TryParse(splitted[0], out statusCode))
            {
                HasResponse = true;
                StatusCode = statusCode;
            }
        }

        public override string ToString()
        {
            var text = Text;
            if (string.IsNullOrEmpty(text))
            {
                return RawErrorMessage;
            }

            return RawErrorMessage + " " + text;
        }
    }

    /// <summary>
    /// 提供 Rx 風格的 <see cref="UnityWebRequest"/> 操作。
    /// </summary>
    public static class ObservableUnityWebRequest
    {
        /// <summary>
        /// 透過 <see cref="UnityWebRequest"/>，使用 HTTP GET 下載並回傳貼圖。
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="nonReadable">貼圖是否為不可讀取。</param>
        /// <param name="uploadProgress">上傳進度回報。</param>
        /// <param name="downloadProgress">下載進度回報。</param>
        /// <returns></returns>
        public static IObservable<Texture2D> GetTexture(
            string url,
            bool nonReadable = false,
            IProgress<float> uploadProgress = null,
            IProgress<float> downloadProgress = null)
        {
            return Observable.FromMicroCoroutine<Texture2D>((observer, token) =>
                FetchTexture(UnityWebRequestTexture.GetTexture(url, nonReadable), observer, uploadProgress, downloadProgress, token));
        }

        static IEnumerator Fetch(
            UnityWebRequest request,
            IObserver<UnityWebRequest> observer,
            IProgress<float> reportUploadProgress,
            IProgress<float> reportDownloadProgress,
            CancellationToken cancellationToken)
        {
            using (request)
            {
                request.Send();

                while (!request.isDone && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (reportUploadProgress != null)
                        {
                            reportUploadProgress.Report(request.uploadProgress);
                        }

                        if (reportDownloadProgress != null)
                        {
                            reportDownloadProgress.Report(request.downloadProgress);
                        }
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                    yield return null;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    request.Abort();
                    yield break;
                }

                if (reportUploadProgress != null)
                {
                    try
                    {
                        reportUploadProgress.Report(request.uploadProgress);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                }

                if (reportDownloadProgress != null)
                {
                    try
                    {
                        reportDownloadProgress.Report(request.downloadProgress);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                }

                if (!string.IsNullOrEmpty(request.error))
                {
                    observer.OnError(new UnityWebRequestErrorException(request, request.error));
                    yield break;
                }

                observer.OnNext(request);
                observer.OnCompleted();
            }
        }

        static IEnumerator FetchTexture(
            UnityWebRequest request,
            IObserver<Texture2D> observer,
            IProgress<float> reportUploadProgress,
            IProgress<float> reportDownloadProgress,
            CancellationToken cancellationToken)
        {
            using (request)
            {
                request.Send();

                while (!request.isDone && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (reportUploadProgress != null)
                        {
                            reportUploadProgress.Report(request.uploadProgress);
                        }

                        if (reportDownloadProgress != null)
                        {
                            reportDownloadProgress.Report(request.downloadProgress);
                        }
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                    yield return null;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    request.Abort();
                    yield break;
                }

                if (reportUploadProgress != null)
                {
                    try
                    {
                        reportUploadProgress.Report(request.uploadProgress);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                }

                if (reportDownloadProgress != null)
                {
                    try
                    {
                        reportDownloadProgress.Report(request.downloadProgress);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                }

                if (!string.IsNullOrEmpty(request.error))
                {
                    observer.OnError(new UnityWebRequestErrorException(request, request.error));
                    yield break;
                }

                observer.OnNext(((DownloadHandlerTexture)request.downloadHandler).texture);
                observer.OnCompleted();
            }
        }
    }
}

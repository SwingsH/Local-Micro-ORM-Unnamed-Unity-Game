using System;
using System.Collections.Generic;
using TIZSoft.Extensions;
using TIZSoft.Log;
using TIZSoft.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Logger = TIZSoft.Log.Logger;
using Random = UnityEngine.Random;

/*
namespace TIZSoft.UI
{
    /// <summary>
    /// 表示一個 WebView，提供 Unity 內嵌顯示網頁功能。
    /// </summary>
    /// <remarks>
    /// 畫面大小請調整 RectTransform。
    /// 基底實作為 gree 的 unity-webview (https://github.com/gree/unity-webview)。
    /// </remarks>
    [RequireComponent(typeof(RectTransform))]
	public class WebView : MonoBehaviour
	{
		[Serializable]
		public struct Header
		{
			public string Key;
			public string Value;
		}

		static readonly Logger logger = LogManager.Default.FindOrCreateCurrentTypeLogger();

		[SerializeField]
		[Tooltip("設定 Start 時是否要自動顯示")]
		bool isVisibleOnStart = true;

		[SerializeField]
		[Tooltip("設定起始 URL，如果有設值，則會自動載入此 URL")]
		string initialUrl;

		[SerializeField]
		[Tooltip("設定共用 HTTP Headers")]
		List<Header> headers;

		string url;
		bool isVisible;
		
		WebViewObject webViewObject;

        readonly Subject<Unit> initialized = new Subject<Unit>();
		readonly Subject<string> javascriptEvaluated = new Subject<string>();
		readonly Subject<string> errorOccurred = new Subject<string>();
		readonly Subject<string> loaded = new Subject<string>();

		/// <summary>
        /// [Event] 發生於元件初始化完畢後。這個事件只會執行一次。
        /// </summary>
        public IObservable<Unit> Initialized
		{
			get { return initialized.AsObservable(); }
		}

		/// <summary>
        /// [Event] 發生於解譯 JavaScript 後。
        /// </summary>
		public IObservable<string> JavascriptEvaluated
		{
			get { return javascriptEvaluated.AsObservable(); }
		}

		/// <summary>
        /// [Event] 發生於底層 WebView 發生錯誤時。
        /// </summary>
		public IObservable<string> ErrorOccurred
		{
			get { return errorOccurred.AsObservable(); }
		}

		/// <summary>
        /// [Event] 發生於頁面載入完畢時。
        /// </summary>
		public IObservable<string> Loaded
		{
			get { return loaded.AsObservable(); }
		}

		/// <summary>
        /// 取得此元件是否初始化完畢。
        /// </summary>
		public bool IsInitialized { get; private set; }

		/// <summary>
        /// 取得或設定此元件是否要顯示。
        /// </summary>
        /// <exception cref="InvalidOperationException">元件尚未初始化完畢。</exception>
		public bool IsVisible
		{
			get { return isVisible; }
			set
			{
				CheckInitialization();

				isVisible = value;

				if (webViewObject != null)
				{
					webViewObject.SetVisibility(value);
				}
			}
		}

        /// <summary>
        /// 取得或設定 URL，設定後將會立刻載入 URL。
        /// </summary>
        /// <exception cref="InvalidOperationException">元件尚未初始化完畢。</exception>
        public string Url
		{
            get { return url; }
			set
			{
				CheckInitialization();

				url = value;

				if (webViewObject != null)
				{
					if (headers != null && headers.Count > 0)
					{
						foreach (var header in headers)
						{
							webViewObject.AddCustomHeader(header.Key, header.Value);
						}
					}

					var extraHeaders = ExtraHeadersCreator.Raise();
					if (extraHeaders != null)
					{
                        foreach (var extraHeader in extraHeaders)
                        {
                            webViewObject.AddCustomHeader(extraHeader.Key, extraHeader.Value);
                        }
                    }

					webViewObject.LoadURL(value);
				}
			}
		}

		public bool CanGoBack
		{
			get { return webViewObject != null && webViewObject.CanGoBack(); }
		}

        public bool CanGoForward
        {
            get { return webViewObject != null && webViewObject.CanGoForward(); }
        }
		
		/// <summary>
        /// 取得或設定自訂 HTTP Headers 的產生函式。
        /// </summary>
		public Func<IEnumerable<KeyValuePair<string, string>>> ExtraHeadersCreator { get; set; }

        public void AddHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            CheckInitialization();

            if (headers == null)
	        {
		        return;
	        }

	        foreach (var header in headers)
	        {
		        webViewObject.AddCustomHeader(header.Key, header.Value);
	        }
        }

        void Start()
        {
            switch (Application.platform)
			{
				case RuntimePlatform.Android:
				case RuntimePlatform.IPhonePlayer:
				case RuntimePlatform.OSXEditor:
				case RuntimePlatform.OSXPlayer:
                    var go = new GameObject(typeof(WebViewObject).Name, typeof(WebViewObject));
                    webViewObject = go.GetComponent<WebViewObject>();
                    break;
                default:
                    logger.Warn(this, "WebView is not supported in platform {0}.", Application.platform);
                    break;
			}

	        if (webViewObject != null)
	        {
		        webViewObject.Init(WebViewObject_OnJavaScriptMessage,
		                           err: WebViewObject_OnErrorMessage,
		                           ld: WebViewObject_OnLoadedMessage,
		                           enableWKWebView: true);
	        }

	        Resize();

	        IsInitialized = true;
	        IsVisible = isVisibleOnStart;
	        if (!string.IsNullOrEmpty(initialUrl))
	        {
		        Url = initialUrl;
	        }

	        OnInitialized();
        }

		void OnEnable()
		{
			if (IsInitialized)
			{
				IsVisible = true;
			}
		}

		void OnDisable()
		{
            if (IsInitialized)
            {
                IsVisible = false;
            }
        }

		void OnDestroy()
		{
			IsVisible = false;
			if (webViewObject != null)
			{
                Destroy(webViewObject.gameObject);
			}
        }

		void CheckInitialization()
		{
			if (IsInitialized)
			{
				return;
			}

			throw new InvalidOperationException("WebView is not initialized.");
		}

        void Resize()
        {
            var t = GetComponent<RectTransform>();

            //       (0, 1)      margin top        (1, 1)
            //             +----------^-----------+
            //             |          |           |
            //             |  c1      |    max=c2 |
            //             |    +-----------+     |
            //             |    |           |     |
            // margin left <----|  WebView  |-----> margin right
            //             |    |           |     |
            //             |    +-----------+     |
            //             | min=c0   |      c3   |
            //             |          |           |
            //             +----------v-----------+
            //       (0, 0)   margin bottom        (1, 0)
            var corners = new Vector3[4];  // [c0, c1, c2, c3]
            t.GetWorldCorners(corners);

            var min = RectTransformUtils.WorldToViewportPoint(corners[0]);
            var max = RectTransformUtils.WorldToViewportPoint(corners[2]);
			
            var marginLeft = Mathf.Clamp((int)(min.x * Screen.width), 0, Screen.width);
            var marginTop = Mathf.Clamp((int)((1F - max.y) * Screen.height), 0, Screen.height);
            var marginRight = Mathf.Clamp((int)((1F - max.x) * Screen.width), 0, Screen.width);
            var marginBottom = Mathf.Clamp((int)(min.y * Screen.height), 0, Screen.height);

			if (webViewObject != null)
			{
				webViewObject.SetMargins(marginLeft, marginTop, marginRight, marginBottom);
			}

			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
                // 只是用來在 editor 驗證是否有算錯
                var w = Screen.width - marginLeft - marginRight;
				var h = Screen.height - marginTop - marginBottom;

				w = (int)(w/t.lossyScale.x);
				h = (int)(h/t.lossyScale.y);

				var texture = new Texture2D(w, h, TextureFormat.ARGB32, false, false);
				for (var y = 0; y < h; ++y)
                {
                    for (var x = 0; x < w; ++x)
                    {
	                    texture.SetPixel(x, y, Random.ColorHSV());
                    }
				}
				texture.Apply();
                var sprite = Sprite.Create(texture, new Rect(0, 0, w, h), new Vector2(0.5F, 0.5F), 100);
				var image = gameObject.AddComponent<Image>();
				image.sprite = sprite;
				image.SetNativeSize();
			}
		}
		
		void WebViewObject_OnJavaScriptMessage(string message)
		{
            logger.Trace(this, message);

			if (message.StartsWith("http"))
			{
				Application.OpenURL(message);
			}

			OnJavascriptEvaluated(message);
        }

		void WebViewObject_OnErrorMessage(string message)
		{
            logger.Trace(this, message);
			OnErrorOccurred(message);
        }

		void WebViewObject_OnLoadedMessage(string message)
		{
			logger.Trace(this, message);

			if (Application.platform != RuntimePlatform.Android)
			{
				if (webViewObject != null)
				{
					webViewObject.EvaluateJS(@"
						window.Unity = {
						  call: function(message) {
							window.location = 'unity:' + message;
						  }
						}
					");
				}
			}

			OnLoaded(message);
		}

		void OnInitialized()
		{
            initialized.OnNext(Unit.Default);
            initialized.OnCompleted();
        }

		void OnJavascriptEvaluated(string message)
		{
            javascriptEvaluated.OnNext(message);
        }

        void OnErrorOccurred(string message)
        {
            errorOccurred.OnNext(message);
        }

		void OnLoaded(string message)
		{
			loaded.OnNext(message);
		}

		/// <summary>
        /// 回上一頁。
        /// </summary>
		public void GoBack()
		{
			CheckInitialization();

			if (webViewObject != null &&
                webViewObject.CanGoBack())
			{
				webViewObject.GoBack();
			}
		}

		/// <summary>
        /// 到下一頁。
        /// </summary>
		public void GoForward()
		{
			CheckInitialization();

			if (webViewObject != null &&
                webViewObject.CanGoForward())
			{
				webViewObject.GoForward();
			}
		}
    }
}
*/
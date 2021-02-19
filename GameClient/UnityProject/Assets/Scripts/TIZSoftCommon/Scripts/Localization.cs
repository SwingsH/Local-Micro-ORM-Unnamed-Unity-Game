using System;
using System.Collections.Generic;
using TIZSoft.Log;
using TIZSoft.Utils;
using UniRx;
using UnityEngine;
using Logger = TIZSoft.Log.Logger;

namespace TIZSoft
{
    /// <summary>
    /// 表示一個提供本地化文字的類別。
    /// </summary>
    public class Localization
	{
		/// <summary>
        /// AssetBundle 分類名稱。
        /// </summary>
		public const string AssetCategory = "localization";

		static readonly Logger logger = LogManager.Default.FindOrCreateCurrentTypeLogger();
		
        readonly AssetProvider assetProvider;

        readonly Dictionary<string, Dictionary<string, string>> languageMap = new Dictionary<string, Dictionary<string, string>>();

		string currentLanguage = string.Empty;

		readonly Subject<Localization> languageChanged = new Subject<Localization>();

		Func<string, Dictionary<string, string>> textParser = ParseJsonText;

		/// <summary>
        /// [Event] 發生於語言改變時。
        /// </summary>
		public IObservable<Localization> LanguageChanged
		{
			get { return languageChanged.AsObservable(); }
		}

		public string CurrentLanguage
		{
			get { return currentLanguage; }
			set
			{
				if (currentLanguage != value)
				{
					currentLanguage = value;
					languageChanged.OnNext(this);
				}
			}
		}

        public Localization(AssetProvider assetProvider)
		{
			ExceptionUtils.VerifyArgumentNull(assetProvider, "assetProvider");
			this.assetProvider = assetProvider;
		}
		
		/// <summary>
        /// 設定本地化字串表要如何處理。
        /// </summary>
        /// <param name="textParser"></param>
		public void SetTextParser(Func<string, Dictionary<string, string>> textParser)
		{
			this.textParser = textParser ?? ParseJsonText;
		}

		public string GetText(string key)
		{
			return GetText(CurrentLanguage, key);
		}

        public IObservable<string> GetTextAsync(string key)
        {
	        return GetTextAsync(CurrentLanguage, key);
        }

        public string GetText(string language, string key)
        {
            ExceptionUtils.VerifyArgumentNullOrEmpty(language, "language");
            ExceptionUtils.VerifyArgumentNullOrEmpty(language, "key");

            Dictionary<string, string> textMap;
	        if (languageMap.TryGetValue(language, out textMap))
	        {
		        string text;
		        if (textMap.TryGetValue(key, out text))
		        {
			        return text;
		        }

                logger.Warn("Language \"{0}\" didn't contain key \"{1}\". Using the key as the return value.",
					language, key);
		        return key;
	        }

            logger.Warn("Get text \"{0}\" failed. Language \"{1}\" not exists. Using the key as the return value",
				key, language);
	        return key;
        }

		public IObservable<string> GetTextAsync(string language, string key)
		{
			var subject = new Subject<string>();
			if (languageMap.ContainsKey(language))
			{
				var map = languageMap[language];
				GetTextAsyncInternal(language, key, map, subject);
			}
			else
			{
                LoadLanguageAsync(language).Subscribe(map => GetTextAsyncInternal(language, key, map, subject));
			}
			return subject;
		}

		static void GetTextAsyncInternal(string language, string key, Dictionary<string, string> map, Subject<string> subject)
		{
			string text;
            if (map.TryGetValue(key, out text))
            {
                subject.OnNext(text);
            }
            else
            {
                logger.Warn("Language \"{0}\" didn't contain key \"{1}\". Using the key as the return value.",
                    language, key);
                subject.OnNext(key);
            }
            subject.OnCompleted();
        }

        IObservable<Dictionary<string, string>> LoadLanguageAsync(string language)
		{
			var subject = new Subject<Dictionary<string, string>>();
            assetProvider.LoadAsync<TextAsset>(AssetCategory, language, asset =>
            {
	            try
	            {
		            languageMap[language] = textParser(asset.text);
		            subject.OnNext(languageMap[language]);
                    subject.OnCompleted();
                }
	            catch (Exception e)
	            {
		            logger.Error(e);
		            subject.OnError(e);
	            }
            });
			return subject.AsObservable();
		}
		
		/// <summary>
        /// 預設的 text parser。
        /// </summary>
        /// <returns></returns>
		public static Dictionary<string, string> ParseJsonText(string text)
		{
			// TODO: Unity 的 JsonUtility 不支援直接反序列化為 Dictionary
			return new Dictionary<string, string>();
		}
	}
}
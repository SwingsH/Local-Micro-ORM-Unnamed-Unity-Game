using System;
using System.IO;
using TIZSoft.Utils;
using UnityEngine;

namespace TIZSoft.SaveData
{
    /// <summary>
    /// 表示一個本地端永久資料儲存類別。
    /// </summary>
    /// <remarks>
    /// 遇到檔案太大時，才有必要考慮新增非同步方法來處理 File IO。
    /// </remarks>
    public class PersistentDataStorage
	{
		static readonly byte[] EmptyBytes = new byte[0];

		string cachedRootDirectoryName;

		/// <summary>
        /// 取得本地端資料儲存的根目錄。
        /// </summary>
        /// <remarks>
        /// Editor 會存在 "{Project}/SaveData/"，
        /// 非 Editor 會存在 Application.persistentDataPath。
        /// </remarks>
		public string RootDirectoryName
		{
			get
			{
				if (string.IsNullOrEmpty(cachedRootDirectoryName))
				{
					if (Application.isEditor)
					{
                        cachedRootDirectoryName = Application.dataPath;
						cachedRootDirectoryName = cachedRootDirectoryName.Substring(0, cachedRootDirectoryName.LastIndexOf("/", StringComparison.InvariantCulture));
						cachedRootDirectoryName = Path.Combine(cachedRootDirectoryName, "SaveData");
					}
					else
					{
						cachedRootDirectoryName = Application.persistentDataPath;
					}
				}
				return cachedRootDirectoryName;
			}
		}
		
		/// <summary>
        /// 取得檔案完整路徑。如果 <paramref name="path"/> 不是絕對路徑，
        /// 則會回傳相對於 <see cref="RootDirectoryName"/> 所在的檔案路徑。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
		string GetFullPath(string path)
		{
			return Path.IsPathRooted(path) ? path : Path.Combine(RootDirectoryName, path);
		}

		static void CreateDirectoryIfNecessary(string fullPath)
		{
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
		
		/// <summary>
        /// 將文字存到指定路徑。
        /// 如果 <paramref name="filename"/> 是相對路徑，則會從 <see cref="RootDirectoryName"/> 裡尋找。
        /// 如果檔案不存在，會建立新檔；如果檔案存在，則會覆蓋原本的檔案。
        /// </summary>
        /// <param name="filename">絕對路徑或相對路徑。</param>
        /// <param name="text"></param>
        public void SaveText(string filename, string text)
		{
			ExceptionUtils.VerifyArgumentNullOrEmpty(filename, "filename");
            var fullPath = GetFullPath(filename);
			CreateDirectoryIfNecessary(fullPath);
            File.WriteAllText(fullPath, text);
		}

        /// <summary>
        /// 將資料存到指定路徑。
        /// 如果 <paramref name="filename"/> 是相對路徑，則會從 <see cref="RootDirectoryName"/> 裡尋找。
        /// 如果檔案不存在，會建立新檔；如果檔案存在，則會覆蓋原本的檔案。
        /// </summary>
        /// <param name="filename">絕對路徑或相對路徑。</param>
        /// <param name="bytes"></param>
        public void SaveBytes(string filename, byte[] bytes)
		{
			if (bytes == null)
			{
				bytes = EmptyBytes;
			}
            SaveBytes(filename, new ArraySegment<byte>(bytes));
        }

        /// <summary>
        /// 將資料存到指定路徑。
        /// 如果 <paramref name="filename"/> 是相對路徑，則會從 <see cref="RootDirectoryName"/> 裡尋找。
        /// 如果檔案不存在，會建立新檔；如果檔案存在，則會覆蓋原本的檔案。
        /// </summary>
        /// <param name="filename">絕對路徑或相對路徑。</param>
        /// <param name="segment"></param>
        public void SaveBytes(string filename, ArraySegment<byte> segment)
		{
            ExceptionUtils.VerifyArgumentNullOrEmpty(filename, "filename");
            var fullPath = GetFullPath(filename);
            CreateDirectoryIfNecessary(fullPath);
            using (var fs = File.OpenWrite(fullPath))
            {
                fs.Write(segment.Array, segment.Offset, segment.Count);
            }
        }

        /// <summary>
        /// 讀取並回傳指定路徑的檔案之文字內容。
        /// 如果 <paramref name="filename"/> 是相對路徑，則會從 <see cref="RootDirectoryName"/> 裡尋找。
        /// </summary>
        /// <param name="filename">絕對路徑或相對路徑。</param>
        /// <returns></returns>
        public string LoadText(string filename)
		{
			ExceptionUtils.VerifyArgumentNullOrEmpty(filename, "filename");
            var fullPath = GetFullPath(filename);
			if (File.Exists(fullPath))
			{
				return File.ReadAllText(fullPath);
			}
			return string.Empty;
		}

        /// <summary>
        /// 讀取並回傳指定路徑的檔案之 byte 內容。
        /// 如果 <paramref name="filename"/> 是相對路徑，則會從 <see cref="RootDirectoryName"/> 裡尋找。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public byte[] LoadBytes(string filename)
		{
            ExceptionUtils.VerifyArgumentNullOrEmpty(filename, "filename");
            var fullPath = GetFullPath(filename);
            return File.ReadAllBytes(fullPath);
        }

        /// <summary>
        /// 刪除指定路徑之檔案。
        /// 如果 <paramref name="filename"/> 是相對路徑，則會從 <see cref="RootDirectoryName"/> 裡尋找。
        /// </summary>
        /// <param name="filename">絕對路徑或相對路徑。</param>
        /// <returns></returns>
        public bool DeleteFile(string filename)
		{
			var fullPath = GetFullPath(filename);
			if (File.Exists(fullPath))
			{
				File.Delete(fullPath);
				return true;
			}
			return false;
		}
	}
}
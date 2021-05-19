using System;
using System.Collections.Generic;
using System.Linq;
using Tizsoft.Extensions;
using Random = UnityEngine.Random;

namespace Tizsoft.Utils
{
    /// <summary>
    /// 提供隨機、亂數相關工具方法。
    /// </summary>
    public static class RandomUtils
    {
        public static List<T> GenerateRandom<T>(List<T> values, int num = -1)
        {
            var list = new List<T>();

            for (var n1 = 0; n1 < values.Count; n1++)
            {
                var iS = Random.Range(n1, values.Count);
                var tmp = values[n1];
                values[n1] = values[iS];
                values[iS] = tmp;
            }

            if (values.Count > 0)
            {
                if (num < 0)
                {
                    num = Random.Range(0, values.Count);
                }

                for (var i = 0; i <= num; i++)
                {
                    list.Add(values[i]);
                }
            }

            return list;
        }

        /// <summary>
        /// 對 <paramref name="items"/> 洗牌。可選擇是否要指定 index 產生方式，若不指定，則使用 Unity 的 <see cref="Random"/> 取 index。
        /// </summary>
        /// <typeparam name="T">Type of items.</typeparam>
        /// <param name="items">要洗牌的集合。</param>
        /// <param name="randomNumberGenerator">Index 產生方法。參數意義為 {items, 迴圈內現在的 index}，回傳的值必須落在 [0, items.Count)。</param>
        public static void Shuffle<T>(this IList<T> items, Func<IList<T>, int, int> randomNumberGenerator = null)
        {
            if (items == null || items.Count <= 1)
            {
                return;
            }

            for (var i = 0; i != items.Count; ++i)
            {
                var randIdx = randomNumberGenerator != null ? randomNumberGenerator.Raise(items, i) : Random.Range(0, items.Count);
                var temp = items[randIdx];
                items[randIdx] = items[i];
                items[i] = temp;
            }
        }

        /// <summary>
        /// 根據權重從 <paramref name="items"/> 中隨機取出一個元素，若沒設定 <paramref name="weights"/>，則會視為機率都一樣。
        /// 可選擇是否要傳入自訂的亂數產生方法，若沒傳入，則會使用預設的產生方法，使用 Unity 的 <see cref="Random"/>。
        /// 這個方法不會改變 <paramref name="items"/> 內容。
        /// </summary>
        /// <typeparam name="T">Type of items.</typeparam>
        /// <param name="items">集合。</param>
        /// <param name="weights">權重，預設為 null。若有傳入，則元素數量必須與 <paramref name="items"/> 一致。</param>
        /// <param name="randomNumberGenerator">亂數產生方法，參數意義為 {items, weights, count, 第 n 項}，回傳值必須介在 [0, 1]。</param>
        /// <returns></returns>
        public static T Choose<T>(
            this IList<T> items,
            IList<float> weights = null,
            Func<IList<T>, IList<float>, int, float> randomNumberGenerator = null)
        {
            return items.ChooseMultiple(weights, 1, randomNumberGenerator).FirstOrDefault();
        }

        /// <summary>
        /// 根據權重從 <paramref name="items"/> 中隨機取出 <paramref name="count"/> 個元素，
        /// 若沒設定 <paramref name="weights"/>，則會視為機率都一樣。
        /// 可選擇是否要傳入自訂的亂數產生方法，若沒傳入，則會使用預設的產生方法，使用 Unity 的 <see cref="Random"/>。
        /// 這個方法不會改變 <paramref name="items"/> 內容。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">集合。</param>
        /// <param name="weights">權重，預設為 null。若有傳入，則元素數量必須與 <paramref name="items"/> 一致。</param>
        /// <param name="count">要從集合中亂數骰出幾個元素。</param>
        /// <param name="randomNumberGenerator">亂數產生方法，參數意義為 {items, weights, count, 第 n 項}，回傳值必須介在 [0, 1]。</param>
        /// <returns></returns>
        public static IList<T> ChooseMultiple<T>(
            this IList<T> items,
            IList<float> weights = null,
            int count = 1,
            Func<IList<T>, IList<float>, int, float> randomNumberGenerator = null)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (weights != null && items.Count != weights.Count)
            {
                throw new ArgumentException("'items' and 'weights' must be the same lengths.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", count, "'count' is less than zero.");
            }

            if (items.Count < count)
            {
                throw new InvalidOperationException("Size of 'items' is less than 'count'.");
            }

            var result = new List<T>();

            if (items.Count == 0 || count == 0)
            {
                return result;
            }

            if (items.Count == 1 && count == 1)
            {
                result.AddRange(items);
                return result;
            }

            // 沒給權重的話，用洗牌法。
            if (weights == null)
            {
                // 防止改變 items 參數。
                var cloneItems = new T[items.Count];
                items.CopyTo(cloneItems, 0);

                cloneItems.Shuffle();
                result.Add(cloneItems.First());
                return result;
            }

            var totalWeight = weights.Sum();
            for (var n = 0; n != count; ++n)
            {
                var rand = randomNumberGenerator != null
                    ? randomNumberGenerator.Raise(items, weights, n)
                    : Random.value;
                var cumulativeWeight = 0F;
                for (var i = 0; i != items.Count; ++i)
                {
                    cumulativeWeight += weights[i];
                    var p = cumulativeWeight / totalWeight;

                    if (rand > p)
                    {
                        continue;
                    }

                    result.Add(items[i]);
                    break;
                }
            }

            return result;
        }
    }
}

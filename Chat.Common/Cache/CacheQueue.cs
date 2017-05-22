using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace Chat.Common.Caching
{

    public abstract class ChatCacheBase<TValue> : CacheQueue<TValue>
    {
        #region Properties

        /// <summary>
        /// 缓存键
        /// </summary>
        public abstract string CacheKey { get; }

        public ReadOnlyCollection<TValue> AllDatas
        {
            get
            {
                return this[this.CacheKey].AsReadOnly();
            }
        }
        #endregion

        #region Methods

        public List<TValue> FindAll(Predicate<TValue> match)
        {
            return this.FindAll(this.CacheKey, match);
        }

        public TValue Find(Predicate<TValue> match)
        {
            return this.Find(this.CacheKey, match);
        }

        public void AddItem(TValue data)
        {
            this.Add(this.CacheKey, data);
        }

        public void RemoveItem(TValue data)
        {
            this.RemoveItem(this.CacheKey, data);
        }

        #endregion

    }

    /// <summary>
    /// 一泛型Cache类
    /// 用户在使用此Cache时需要从此类派生
    /// </summary>
    /// <typeparam name="TValue">键值类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class CacheQueue<TValue>
    {
        /// <summary>
        /// 保存Cache项的字典
        /// </summary>
        private Dictionary<string, List<TValue>> innerDictionary = null;
        #region 构造方法
        /// <summary>
        /// 构造函数，没有设置CacheQueue的容量大小，则使用默认值100
        /// </summary>
        protected CacheQueue()
        {
            this.innerDictionary = new Dictionary<string, List<TValue>>();
        }

        #endregion 构造方法

        #region 公有属性

        /// <summary>
        /// 获取CacheQueue中的存储的Cache项的数量
        /// </summary>
        public int Count
        {
            get
            {
                return this.innerDictionary.Count;
            }
        }

        /// <summary>
        /// 通过Cache项的key获取Cache项Value的索引器
        /// </summary>
        /// <param name="key">cache项key</param>
        /// <returns>cache项Value</returns>
        public List<TValue> this[string key]
        {
            get
            {
                List<TValue> item = null;
                if (key != null && this.innerDictionary.ContainsKey(key))
                    item = this.innerDictionary[key];
                return item;
            }
            set
            {
                this.innerDictionary[key] = value;
            }
        }

        #endregion 公有属性

        #region Add

        /// <summary>
        /// 向CacheQueue中增加一Cache项值对，如果相应的key已经存在，则抛出异常
        /// 此种构造方法无相关Dependency，所以此增加Cache项不会过期，只可能当CacheQueue
        /// 的长度超过预先设定时，才可能被清理掉
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">值</param>
        /// <returns>值</returns>
        public void Add(string key, TValue data)
        {
            try
            {
                lock (this.innerDictionary)
                {
                    if (!this.ContainsKey(key))
                        this.innerDictionary[key] = new List<TValue>();
                    this.innerDictionary[key].Add(data);
                }
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
        }

        #endregion

        #region Remove

        /// <summary>
        /// 通过key，删除一Cache项
        /// </summary>
        /// <param name="key">缓存唯一标识</param>
        public void Remove(string key)
        {
            try
            {
                lock (this.innerDictionary)
                {
                    this.innerDictionary.Remove(key);
                }
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
        }

        /// <summary>
        /// 重载基类方法，删除传入的CacheItem
        /// </summary>
        /// <param name="cacheItem">缓存队列对象</param>
        public void RemoveItem(string key, TValue data)
        {
            List<TValue> dataList = null;
            try
            {
                this.TryGetValue(key, out dataList);
                dataList.Remove(data);
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
        }

        #endregion

        #region Clear

        /// <summary>
        /// 清空整个CacheQueue，删除CacheQueue中所有的Cache项
        /// </summary>
        public void Clear()
        {
            try
            {
                this.innerDictionary.Clear();
            }
            catch { }
        }

        #endregion

        #region Query

        /// <summary>
        /// 通过key，获取Cache项的value，如果相应的cache项存在的话
        /// 则将cache项的value作为输出参数，返回给客户端代码
        /// </summary>
        /// <param name="key">cache项的key</param>
        /// <param name="data">cache项的value</param>
        /// <returns>如果CacheQueue中包含此Cache项，则返回true，否则返回false</returns>
        public bool TryGetValue(string key, out List<TValue> datas)
        {
            datas = default(List<TValue>);
            List<TValue> cacheItem = null;
            bool result;
            try
            {
                result = this.innerDictionary.TryGetValue(key, out cacheItem);
                if (cacheItem != null)
                    datas = cacheItem;
            }
            catch { result = false; }
            return result;
        }

        /// <summary>
        /// 判断CacheQueue中是否包含key键的Cache项
        /// </summary>
        /// <param name="key">查询的cache项的键值</param>
        /// <returns>如果包含此键值，返回true，否则返回false</returns>
        public bool ContainsKey(string key)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(key))
                result = this.innerDictionary.ContainsKey(key);
            return result;
        }

        public List<TValue> FindAll(string key, Predicate<TValue> match)
        {
            List<TValue> result = new List<TValue>();
            List<TValue> cacheItem = null;
            this.TryGetValue(key, out cacheItem);
            if (cacheItem != null)
                result = cacheItem.FindAll(match);
            return result;
        }

        public TValue Find(string key, Predicate<TValue> match)
        {
            TValue result = default(TValue);
            List<TValue> cacheItem = null;
            this.TryGetValue(key, out cacheItem);
            if (cacheItem != null)
                result = cacheItem.Find(match);
            return result;
        }

        #endregion
    }

}

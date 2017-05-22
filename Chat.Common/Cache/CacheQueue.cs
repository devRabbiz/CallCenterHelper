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
        /// �����
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
    /// һ����Cache��
    /// �û���ʹ�ô�Cacheʱ��Ҫ�Ӵ�������
    /// </summary>
    /// <typeparam name="TValue">��ֵ����</typeparam>
    /// <typeparam name="TValue">ֵ����</typeparam>
    public class CacheQueue<TValue>
    {
        /// <summary>
        /// ����Cache����ֵ�
        /// </summary>
        private Dictionary<string, List<TValue>> innerDictionary = null;
        #region ���췽��
        /// <summary>
        /// ���캯����û������CacheQueue��������С����ʹ��Ĭ��ֵ100
        /// </summary>
        protected CacheQueue()
        {
            this.innerDictionary = new Dictionary<string, List<TValue>>();
        }

        #endregion ���췽��

        #region ��������

        /// <summary>
        /// ��ȡCacheQueue�еĴ洢��Cache�������
        /// </summary>
        public int Count
        {
            get
            {
                return this.innerDictionary.Count;
            }
        }

        /// <summary>
        /// ͨ��Cache���key��ȡCache��Value��������
        /// </summary>
        /// <param name="key">cache��key</param>
        /// <returns>cache��Value</returns>
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

        #endregion ��������

        #region Add

        /// <summary>
        /// ��CacheQueue������һCache��ֵ�ԣ������Ӧ��key�Ѿ����ڣ����׳��쳣
        /// ���ֹ��췽�������Dependency�����Դ�����Cache�����ڣ�ֻ���ܵ�CacheQueue
        /// �ĳ��ȳ���Ԥ���趨ʱ���ſ��ܱ������
        /// </summary>
        /// <param name="key">��</param>
        /// <param name="data">ֵ</param>
        /// <returns>ֵ</returns>
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
        /// ͨ��key��ɾ��һCache��
        /// </summary>
        /// <param name="key">����Ψһ��ʶ</param>
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
        /// ���ػ��෽����ɾ�������CacheItem
        /// </summary>
        /// <param name="cacheItem">������ж���</param>
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
        /// �������CacheQueue��ɾ��CacheQueue�����е�Cache��
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
        /// ͨ��key����ȡCache���value�������Ӧ��cache����ڵĻ�
        /// ��cache���value��Ϊ������������ظ��ͻ��˴���
        /// </summary>
        /// <param name="key">cache���key</param>
        /// <param name="data">cache���value</param>
        /// <returns>���CacheQueue�а�����Cache��򷵻�true�����򷵻�false</returns>
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
        /// �ж�CacheQueue���Ƿ����key����Cache��
        /// </summary>
        /// <param name="key">��ѯ��cache��ļ�ֵ</param>
        /// <returns>��������˼�ֵ������true�����򷵻�false</returns>
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

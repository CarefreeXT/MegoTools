using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Caredev.MegoTools.Core
{
    /// <summary>
    /// 数据库对象基类。
    /// </summary>
    public abstract partial class DatabaseBase
    {
        /// <summary>
        /// 显示标题。
        /// </summary>
        public abstract string Title { get; }
        /// <summary>
        /// 种类。
        /// </summary>
        public abstract EDatabaseKind Kind { get; }
        /// <summary>
        /// 提供程序工厂对象。
        /// </summary>
        public abstract DbProviderFactory Factory { get; }
        /// <summary>
        /// 是否支持关系。
        /// </summary>
        public virtual bool SupportRelation { get; }
        /// <summary>
        /// 提供程序名称。
        /// </summary>
        public virtual string ProviderName
        {
            get
            {
                return Factory.GetType().Namespace;
            }
        }
        /// <summary>
        /// 默认连接字符串
        /// </summary>
        public virtual string DefalutConnectionString { get; } = string.Empty;
        /// <summary>
        /// 创建数据对象集合。
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual DbObjectCollection CreateCollection(IConnectionInformation info)
        {
            throw new NotImplementedException();
        }
    }
    partial class DatabaseBase
    {
        static DatabaseBase()
        {
            var query = typeof(DatabaseBase).Assembly.GetTypes()
                .Where(a => a != typeof(DatabaseBase) && typeof(DatabaseBase).IsAssignableFrom(a))
                .Select(a => (DatabaseBase)Activator.CreateInstance(a));
            var dic = query.OrderBy(a => a.ProviderName).ToDictionary(a => a.ProviderName, a => a);
            _Databases = new ReadOnlyDictionary<string, DatabaseBase>(dic);
        }
        /// <summary>
        /// 所有数据库对象。
        /// </summary>
        public static IEnumerable<DatabaseBase> Databases => _Databases.Values;
        private readonly static IDictionary<string, DatabaseBase> _Databases;
        /// <summary>
        /// 通过提供程序名称获取指定数据对象。
        /// </summary>
        /// <param name="providerName">提供程序名称。</param>
        /// <returns>数据库对象。</returns>
        public static DatabaseBase GetDatabase(string providerName)
        {
            if (_Databases.TryGetValue(providerName, out DatabaseBase value))
            {
                return value;
            }
            throw new KeyNotFoundException(providerName);
        }
        /// <summary>
        /// 通过<see cref="EDatabaseKind"/>获取指定数据库对象。
        /// </summary>
        /// <param name="kind">数据库种类。</param>
        /// <returns>数据库对象。</returns>
        public static DatabaseBase GetDatabase(EDatabaseKind kind)
        {
            var value = _Databases.Values.FirstOrDefault(a => a.Kind == kind);
            if (value == null)
            {
                throw new KeyNotFoundException(kind.ToString());
            }
            return value;
        }
    }
}

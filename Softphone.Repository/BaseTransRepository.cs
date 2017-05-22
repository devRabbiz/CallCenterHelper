using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPhone.Repository
{

    public class BaseTransRepository : ITransRepository
    {
        private DbContext db = null;
        private bool autoCommit = true;

        /// <summary>
        /// 空构造
        /// </summary>
        public BaseTransRepository()
        {

        }
        /// <summary>
        /// 设置DbContext
        /// </summary>
        public DbContext DB
        {
            get
            {
                return db;
            }
            //set
            //{
            //    db = value;
            //}
        }

        /// <summary>
        /// 设置自动提交选项,true：调用SaveChanges时自动提交事务，false：不自动提交，需要手动在业务逻辑服务层调用SaveChanges
        /// </summary>
        public bool AutoCommit
        {
            get
            {
                return autoCommit;
            }
            set
            {
                autoCommit = value;
            }
        }

        //提供IOC注入方式接口
        public BaseTransRepository(DbContext context)
        {
            this.db = context;
        }

        /// <summary>
        /// 保存变更，默认自动提交，如果改为手动提交需要在业务逻辑层手动保存
        /// </summary>
        public int SaveChanges()
        {
            if (autoCommit)
            {
                return db.SaveChanges();
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            //if (autoCommit) db.SaveChanges();
            db.Dispose();
        }
    }
}

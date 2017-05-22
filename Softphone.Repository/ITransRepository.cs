using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPhone.Repository
{
    public interface ITransRepository
    {
        /// <summary>
        /// 保存变更，默认自动提交，如果改为手动提交需要在业务逻辑层手动保存
        /// </summary>
        int SaveChanges();
    }
}

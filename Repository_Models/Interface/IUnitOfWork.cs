using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository_Models.Interface;

namespace IUnitOfWork.Interface
{
    /// <summary>
    /// 實作Unit Of Work的interface。
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 儲存所有異動。
        /// </summary>
        void Save();

        /// <summary>
        /// 取得某一個Entity的Repository。
        /// 如果沒有取過，會initialise一個
        /// 如果有就取得之前initialise的那個。
        /// 產生出 UnitOfWork 的物件時 可循環利用該方法達到節省記憶體空間之功能
        /// </summary>
        /// <typeparam name="T">此Context裡面的Entity Type</typeparam>
        /// <returns>Entity的Repository</returns>
        IRepository<T> GetRepository<T>() where T : class;
    }
}

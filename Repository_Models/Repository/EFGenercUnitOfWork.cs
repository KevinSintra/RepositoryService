using System;
using System.Collections;
using System.Data.Entity;
using Repository_Models.Interface;
using Repository_Models.Repository;

namespace IUnitOfWork
{
    /// <summary>
    /// EFGenercUnitOfWork 是統一產生對應的 EFGenericRepository 的類別 
    /// Service層 請善用該類別不應自己實例 出 EFGenericRepository 的物件
    /// </summary>
    public class EFGenercUnitOfWork : Interface.IUnitOfWork
    {
        private readonly DbContext _context;

        //在生命週期結束前 使用 雜湊表 儲存 泛行方法 的 類型 當KEY 值
            //產生出的 instance 當 Value
        private Hashtable _repositories = null; 

        public EFGenercUnitOfWork(DbContext context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            this._context = context;
        }

        /// <summary>
        /// 取得某一個Entity的Repository。
        /// 如果沒有取過，會initialise一個
        /// 如果有就取得之前initialise的那個。
        /// 產生出 UnitOfWork 的物件時 可循環利用該方法達到節省記憶體空間之功能
        /// </summary>
        /// <typeparam name="T">此Context裡面的Entity Type</typeparam>
        /// <returns>Entity的Repository</returns>
        public IRepository<T> GetRepository<T>() where T : class
        {
            if (this._repositories == null) this._repositories = new Hashtable();

            //取得泛型中的類型型態
            var type = typeof(T).Name;

            //雜湊表中找不到對應的類型時 創立 Key 儲存 T 類型，Value 儲存 對應的實體
            if (!this._repositories.ContainsKey(type))
            {
                //取得通用的REPOSITORY類型
                var repositoryType = typeof(EFGenericRepository<>);

                //使用 EFGenericRepository類型 與外部的傳遞的泛型 建立出對應的實體，
                //最後傳遞 EFGenericRepository 建構子所需的參數
                var repositoryInsetance = Activator.CreateInstance(repositoryType.MakeGenericType(
                    typeof(T)), this._context);

                //儲存型別名稱 與對應的實例 到雜湊表之中
                this._repositories.Add(type, repositoryInsetance);
            }

            //將雜湊表中的Object取出 並 轉型
            return (this._repositories[type] as IRepository<T>);
        }

        /// <summary>
        /// 儲存此次的 DbContext 資料異動
        /// </summary>
        public void Save()
        {
            //TODO: 需注意 EFGenericRepository<T> 修改方法的驗證，怕會對整體異動資料上有影響
            this._context.SaveChanges();
            if (this._context.Configuration.ValidateOnSaveEnabled == false)
                this._context.Configuration.ValidateOnSaveEnabled = true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                this._context.Dispose();
                this._repositories.Clear();
                this._repositories = null;
                disposedValue = true;
            }
        }

        ~EFGenercUnitOfWork()
        {
            Dispose(false);
        }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

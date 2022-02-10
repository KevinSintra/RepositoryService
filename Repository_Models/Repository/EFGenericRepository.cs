using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;

namespace Repository_Models.Repository
{
    //TODO : 還需把判斷 entity == null 丟出例外的 工作 拋給 Service Layer

    //internal
    /// <summary>
    /// 該類別是通用型的 Repository。使用 Entity 的機制
    /// </summary>
    /// <typeparam name="TEntity">請傳入Entity對應的資料類別</typeparam>
    public class EFGenericRepository<TEntity> : Interface.IRepository<TEntity>
        where TEntity : class
    {
        private DbContext _context { get; set; }//使用 Entity Framework 的機制

        //public EFGenericRepository()// : this(new FinalMvcTestDatabaseEntities())
        //{
        //    throw new NotSupportedException(String.Format("{0} 不支援無參數的初始化",this.GetType().FullName));
        //}

        public EFGenericRepository(DbContext context)
        {
            this._context = context;
        }


        /// <summary>
        /// 新增一筆資料到資料庫。
        /// </summary>
        /// <param name="entity">要新增到資料的庫的Entity</param>
        public void Create(TEntity entity)
        {
            //指定entity物件為 TEntity 的類別 回傳對應的DbSet的物件 => 等同指定資料表
            this._context.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// 刪除一筆資料
        /// </summary>
        /// <param name="entity">資料對應的Entity類別</param>
        public void Delete(TEntity entity)
        {
            this._context.Entry(entity).State = EntityState.Deleted;
        }

        /// <summary>
        /// 修改一筆資料
        /// </summary>
        /// <param name="instance">資料對應的Entity類別</param>
        public void Update(TEntity entity)
        {
            this._context.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// 更新一筆資料的內容。只更新部分欄位的。
        /// Lambda 運算式 只需要傳遞欄位屬性 EX : x => x.ColumnName1, x => x.Column2....
        /// </summary>
        /// <param name="entity">要更新的內容</param>
        /// <param name="updateProperties">需要更新的欄位。</param>
        public void Update(TEntity entity, Expression<Func<TEntity, object>>[] updateProperties)
        {
            // 想要略過 EF 檢查 關閉自動追蹤實體的驗證
            this._context.Configuration.ValidateOnSaveEnabled = false;
            //其屬性還未更新到資料庫 但先做紀錄
            this._context.Entry(entity).State = EntityState.Unchanged;

            if (updateProperties != null)
            {
                //確認那些欄位是要修改 的 做上記號
                foreach (var item in updateProperties)
                {
                    this._context.Entry(entity).Property(item).IsModified = true;
                }
            }
        }

        //此方法目前內部不使用 外部也不使用 先暫時放著
        /// <summary>
        /// 儲存此次的資料異動
        /// </summary>
        public void SaveChanges()
        {
            this._context.SaveChanges();

            //System.ComponentModel; System.ComponentModel.DataAnnotations; 依據實體的規範如 [[Required]]
            //因為Update(,) 單一model需要先關掉validation，因此重新打開
            if (this._context.Configuration.ValidateOnSaveEnabled == false)
                this._context.Configuration.ValidateOnSaveEnabled = true;
        }

        /// <summary>
        /// 取得第一筆符合條件的內容。如果符合條件有多筆，也只取得第一筆。
        /// </summary>
        /// <param name="predicate">要取得的Where條件。</param>
        /// <returns>取得第一筆符合條件的內容。</returns>
        public TEntity Read(Expression<Func<TEntity, bool>> predicate)
        {
            return this._context.Set<TEntity>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// 取得合條件的內容。可能回傳一筆或是多筆 IQueryable
        /// </summary>
        /// <param name="predicate">要取得的Where條件。</param>
        /// <param name="predicates">要取得的Where條件。可新增N個條件值</param>
        /// <returns>只要符合條件則回傳全部筆數的IQueryable。</returns>
        public IQueryable<TEntity> Reads(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, bool>>[] predicates)
        {
            var datas = this._context.Set<TEntity>().Where(predicate);
            if (predicates != null)
            {
                foreach (var expression in predicates)
                {
                    datas = datas.Where(expression);
                }
            }
            return datas;
        }

        /// <summary>
        /// 取得Entity全部筆數的IQueryable。
        /// </summary>
        /// <returns>Entity全部筆數的IQueryable。</returns>
        public IQueryable<TEntity> Reads()
        {
            return this._context.Set<TEntity>().AsQueryable();
        }

        #region 釋放資源
        ~EFGenericRepository()
        {
            this._context = null;
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

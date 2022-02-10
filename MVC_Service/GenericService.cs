using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;

namespace MVC_Service 
{
    //internal
    /// <summary>
    /// 外部若要操作資料應該都透過 Service層 做存取與操作 
    /// 不應當自己實例化出 EFGenercUnitOfWork && EFGenericRepository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class GenericService<TEntity> : Interface.IGenericService<TEntity>
        where TEntity : class
    {
        //宣告IUnitOfWork的屬性
        protected IUnitOfWork.Interface.IUnitOfWork db;

        public GenericService(DbContext inContext)
        {
            if (inContext == null) { throw new ArgumentNullException("context"); }
            this.db = new IUnitOfWork.EFGenercUnitOfWork(inContext);
        }


        #region AutoMapper
        // 使用 AutoMapper 轉成對應的 Entity Model
        /// <summary>
        /// 接收前端的ViewModel 產生對應的 Model 且帶入資料
        /// </summary>
        /// <typeparam name="ViewModel">對應的ViewModel類型</typeparam>
        /// <param name="inViewModelData">帶有資料的ViewModel</param>
        /// <returns>帶有資料的 TEntity 類別</returns>
        protected static TEntity staticConvertViewModelToModel<ViewModel>(ViewModel inViewModelData)
        {
            //建立組態檔
            var cfg = new MapperConfigurationExpression();
            //可反轉
            cfg.CreateMap<ViewModel, TEntity>().ReverseMap();
            //實例 Mapper 的物件
            Mapper.Initialize(cfg);

            // 依據 ViewModel 的資料 建立出 <TEntity> 型別的物件
            return Mapper.Map<TEntity>(inViewModelData);
        }

        // 使用 AutoMapper 轉成對應的 ViewModel
        /// <summary>
        /// 抓取出來的資料 轉成對應的ViewModel 並帶入資料
        /// </summary>
        /// <typeparam name="TEntity">對應的Model類型</typeparam>
        /// <param name="inModelData">帶有資料的ViewModel</param>
        /// <returns>帶有資料的 ViewModel</returns>
        protected static ViewModel staticConvertModelToViewModel<ViewModel>(TEntity inModelData)
        {
            //建立組態檔
            var cfg = new MapperConfigurationExpression();
            //可反轉
            cfg.CreateMap<TEntity, ViewModel>().ReverseMap();
            //實例 Mapper 的物件
            Mapper.Initialize(cfg);

            // 依據 ViewModel 的資料 建立出 <TEntity> 型別的物件
            return Mapper.Map<ViewModel>(inModelData);
        }

        // 使用 AutoMapper 轉成對應的 List<ViewModel>
        /// <summary>
        /// 抓取出來的 IQueryable<TEntity> 資料 轉成對應的 IQueryable<ViewModel> 並帶入資料
        /// </summary>
        /// <typeparam name="TEntity">對應的Model類型</typeparam>
        /// <param name="inModelData">帶有資料的ViewModel</param>
        /// <returns>帶有資料的 IQueryable<ViewModel> 集合</returns>
        protected static IQueryable<ViewModel> staticConvertListModelToListViewModel<ViewModel>(
            IQueryable<TEntity> inModelData)
        {
            //建立組態檔
            var cfg = new MapperConfigurationExpression();
            //可反轉
            cfg.CreateMap<TEntity, ViewModel>().ReverseMap();
            //實例 Mapper 的物件
            Mapper.Initialize(cfg);

            // 依據 ViewModel 的資料 建立出 <TEntity> 型別的物件
            return Mapper.Map<IQueryable<ViewModel>>(inModelData);
        }

        // 使用 AutoMapper 修改Model的資料
        ///// <summary>
        ///// 是將以撈取出來的EntityModel的資料 以 ViewModel 中的資料 修改 
        ///// </summary>
        ///// <typeparam name="ViewModel">對應的ViewModel類型</typeparam>
        ///// <param name="inViewModelData">帶有資料的 ViewModel</param>
        ///// <param name="inEntityModelData">欲更新的EntityModel</param>
        //protected static void staticUpdViewModelToEntityModel<ViewModel>(
        //    ViewModel inViewModelData, TEntity inEntityModelData)
        //{
        //    //建立組態檔
        //    var cfg = new MapperConfigurationExpression();
        //    //可反轉
        //    cfg.CreateMap<TEntity, ViewModel>().ReverseMap();
        //    //實例 Mapper 的物件
        //    Mapper.Initialize(cfg);
        //    //此時 AutoMap 將使用 ViewModel 修改 Model 的資料
        //    Mapper.Map(inViewModelData, inEntityModelData);
        //}
        #endregion


        /// <summary>
        /// 依照某一個ViewModel的值，產生對應的Entity並且新增到資料庫
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel的形態</typeparam>
        /// <param name="viewModel">ViewModel的Reference</param>
        /// <returns>是否儲存成功</returns>
        /// <exception cref="System.ArgumentNullException">資料的Entity為Null</exception>
        public virtual void CreateViewModelToDatabase<TViewModel>(TViewModel viewModel)
        {
            if (viewModel == null) { throw new ArgumentNullException("instance"); }
            var entity = staticConvertViewModelToModel(viewModel);
            this.db.GetRepository<TEntity>().Create(entity);
            this.db.Save();
        }


        /// <summary>
        /// 刪除某一筆Entity
        /// </summary>
        /// <param name="wherePredicate">過濾出要被刪除的Entity條件</param>
        /// <returns>是否刪除成功</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void Delete(Expression<Func<TEntity, bool>> wherePredicate)
        {
            var data = this.db.GetRepository<TEntity>().Read(wherePredicate);
            if (data == null) { throw new ArgumentNullException("Not Found"); }
            this.db.GetRepository<TEntity>().Delete(data);
            this.db.Save();
        }


        /// <summary>
        /// 依照某一個ViewModel的值，覆寫對應的Entity
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel的形態</typeparam>
        /// <param name="viewModel">ViewModel的值</param>
        /// <returns>是否更新成功</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void UpdateViewModelToDatabase<TViewModel>(TViewModel viewModel)
        {
            if (viewModel == null) { throw new ArgumentNullException("instance"); }
            //取得對應並帶有資料的 Entity 類別
            var enitity = staticConvertViewModelToModel(viewModel);
            this.db.GetRepository<TEntity>().Update(enitity);
            this.db.Save();
        }

        /// <summary>
        /// 更新一筆資料的內容。只更新部分欄位的。
        /// Lambda 運算式 只需要傳遞欄位屬性 EX : x => x.ColumnName1, x => x.Column2....
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel的形態</typeparam>
        /// <param name="updateProperties">需要更新的欄位。</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void UpdateViewModelToDatabase<TViewModel>(
            TViewModel viewModel, Expression<Func<TEntity, object>>[] updateProperties)
        {
            if (viewModel == null) { throw new ArgumentNullException("instance"); }
            var entity = staticConvertViewModelToModel(viewModel);
            this.db.GetRepository<TEntity>().Update(entity, updateProperties);
            this.db.Save();
        }

        /// <summary>
        /// 取得全部的資料 且將資料帶入對應的 Viewmodel
        /// </summary>
        /// <typeparam name="TViewModel">Model對應的ViewModel</typeparam>
        /// <returns> List<TViewModel> 集合類別 </returns>
        public virtual List<TViewModel> GetListToViewModel<TViewModel>()
        {
            var data = this.db.GetRepository<TEntity>().Reads();
            return staticConvertListModelToListViewModel<TViewModel>(data).ToList();
        }

        /// <summary>
        /// 取得某一個條件下面的某一筆Entity並且轉成對應的ViewModel
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel的形態</typeparam>
        /// <param name="wherePredicate">過濾邏輯</param>
        /// <returns>取得轉換過的ViewModel或者是null</returns>
        public virtual TViewModel GetSpecificDetailToViewModel<TViewModel>(Expression<Func<TEntity, bool>> wherePredicate)
        {
            var data = this.db.GetRepository<TEntity>().Read(wherePredicate);
            return staticConvertModelToViewModel<TViewModel>(data);
        }

        /// <summary>
        /// 取得某一個條件下面的某一筆Entity並且轉成對應的ViewModel
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel的形態</typeparam>
        /// <param name="wherePredicate">過濾邏輯</param>
        /// <param name="wherePredicates">要取得的Where條件。可新增N個條件值</param>
        /// <returns>取得轉換過的List<ViewModel>或者是null</returns>
        public virtual List<TViewModel> GetSpecificDetailToViewModel<TViewModel>(
            Expression<Func<TEntity, bool>> wherePredicate, params Expression<Func<TEntity, bool>>[] predicates)
        {
            var datas = this.db.GetRepository<TEntity>().Reads(wherePredicate, predicates);
            return staticConvertListModelToListViewModel<TViewModel>(datas).ToList();
        }

        #region IDisposable Support
        private bool _disposedvalue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedvalue)
            {
                if (disposing)
                {                    
                }
                this.db.Dispose();
                this.db = null;
                this._disposedvalue = true;
            }
        }

        ~GenericService()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

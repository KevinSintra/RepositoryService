using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Service.Interface
{
    /// <summary>
    /// Service服務層內容的Interface
    /// </summary>
    /// <typeparam name="T">主要要儲存的Entity Type</typeparam
    public interface IGenericService<T> :IDisposable where T : class
    {
        /// <summary>
        /// 依照某一個ViewModel的值，產生對應的Entity並且新增到資料庫
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel的形態</typeparam>
        /// <param name="viewModel">ViewModel的Reference</param>
        /// <returns>是否儲存成功</returns>
        void CreateViewModelToDatabase<TViewModel>(TViewModel viewModel);

        /// <summary>
        /// 刪除某一筆Entity
        /// </summary>
        /// <param name="wherePredicate">過濾出要被刪除的Entity條件</param>
        /// <returns>是否刪除成功</returns>
        void Delete(Expression<Func<T, bool>> wherePredicate);

        /// <summary>
        /// 依照某一個ViewModel的值，覆寫對應的Entity
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel的形態</typeparam>
        /// <param name="viewModel">ViewModel的值</param>
        /// <returns>是否更新成功</returns>
        void UpdateViewModelToDatabase<TViewModel>(TViewModel viewModel);

        /// <summary>
        /// 更新一筆資料的內容。只更新部分欄位的。
        /// Lambda 運算式 只需要傳遞欄位屬性 EX : x => x.ColumnName1, x => x.Column2....
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel的形態</typeparam>
        /// <param name="updateProperties">需要更新的欄位。</param>
        void UpdateViewModelToDatabase<TViewModel>(TViewModel viewModel,
            Expression<Func<T, object>>[] updateProperties);

        /// <summary>
        /// 取得全部的資料 且將資料帶入對應的 Viewmodel
        /// </summary>
        /// <typeparam name="TViewModel">Model對應的ViewModel</typeparam>
        /// <returns></returns>
        List<TViewModel> GetListToViewModel<TViewModel>();

        /// <summary>
        /// 取得某一個條件下面的某一筆Entity並且轉成對應的ViewModel
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel的形態</typeparam>
        /// <param name="wherePredicate">過濾邏輯</param>
        /// <returns>取得轉換過的ViewModel或者是null</returns>
        TViewModel GetSpecificDetailToViewModel<TViewModel>(Expression<Func<T, bool>> wherePredicate);

        /// <summary>
        /// 取得某一個條件下面的某一筆Entity並且轉成對應的List<ViewModel>
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel的形態</typeparam>
        /// <param name="wherePredicate">過濾邏輯</param>
        /// <param name="wherePredicates">要取得的Where條件。可新增N個條件值</param>
        /// <returns>取得轉換過的ViewModel或者是null</returns>
        List<TViewModel> GetSpecificDetailToViewModel<TViewModel>(Expression<Func<T, bool>> wherePredicate,
            params Expression<Func<T, bool>>[] predicates);
    }
}

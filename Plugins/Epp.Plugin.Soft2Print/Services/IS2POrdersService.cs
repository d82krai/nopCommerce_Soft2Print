using System.Threading.Tasks;
using Epp.Plugin.Soft2Print.Domain;
using Nop.Core;


namespace Nop.Plugin.Soft2Print.Services
{
    /// <summary>
    /// Represents service S2POrders service
    /// </summary>
    public partial interface IS2POrdersService
    {
        /// <summary>
        /// Get all orders records
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the shipping by weight record
        /// </returns>
        Task<IPagedList<S2POrders>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Get a S2POrders record by passed parameters
        /// </summary>
        /// <param name="customerId">customer identifier</param>
        /// <param name="productId">product identifier</param>
        /// <param name="storeId">store identifier</param>
        /// <param name="projectId">project identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the S2POrders record
        /// </returns>
        Task<S2POrders> FindRecordsAsync(int customerId, int? productId, int? storeId, int? projectId, OrderStatus status,int? shoppingCartId);

        /// <summary>
        /// Filter S2POrders Records
        /// </summary>
        /// <param name="customerId">customer identifier</param>
        /// <param name="productId">product identifier</param>
        /// <param name="storeId">store identifier</param>
        /// <param name="projectId">project identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the S2POrders record
        /// </returns>
        Task<IPagedList<S2POrders>> FindRecordsAsync(int customerId, int? productId, int? storeId, int? projectId, int? shoppingCartId, int pageIndex, int pageSize);

        /// <summary>
        /// Get a S2POrders record by identifier
        /// </summary>
        /// <param name="s2PId">Record identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the S2POrders record
        /// </returns>
        Task<S2POrders> GetByIdAsync(int s2PId);

        /// <summary>
        /// Insert the s2pOrder record
        /// </summary>
        /// <param name="s2pOrderRecord">s2pOrder record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertS2POrderRecordAsync(S2POrders s2pOrderRecord);

        /// <summary>
        /// Update the s2pOrder record
        /// </summary>
        /// <param name="s2pOrderRecord">s2pOrder record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateS2POrderRecordAsync(S2POrders s2pOrderRecord);

        /// <summary>
        /// Delete the s2pOrder record
        /// </summary>
        /// <param name="s2pOrderRecord">s2pOrder record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteS2POrderRecordAsync(S2POrders s2pOrderRecord);
    }
}

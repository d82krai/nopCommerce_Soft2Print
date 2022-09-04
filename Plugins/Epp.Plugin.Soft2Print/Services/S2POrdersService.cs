using System;
using System.Linq;
using System.Threading.Tasks;
using Epp.Plugin.Soft2Print.Domain;
using Nop.Services.Logging;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;


namespace Nop.Plugin.Soft2Print.Services
{
    /// <summary>
    /// Represents service S2POrders service implementation
    /// </summary>
    public partial class S2POrdersService : IS2POrdersService
    {
        #region Constants

        /// <summary>
        /// Key for caching all records
        /// </summary>
        private readonly CacheKey _s2pOrdersAllKey = new("Nop.s2porders.all", S2PORDERS_PATTERN_KEY);
        private const string S2PORDERS_PATTERN_KEY = "Nop.s2porders.";

        #endregion

        #region Fields

        private readonly IRepository<S2POrders> _s2pRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ILogger _logger;
        #endregion

        #region Ctor

        public S2POrdersService(IRepository<S2POrders> s2pRepository,
            IStaticCacheManager staticCacheManager,
             ILogger logger)
        {
            _s2pRepository = s2pRepository;
            _staticCacheManager = staticCacheManager;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get all S2POrders records
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the S2POrders record
        /// </returns>
        public virtual async Task<IPagedList<S2POrders>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rez = await _s2pRepository.GetAllAsync(query =>
            {
                return from s2p in query
                       orderby s2p.CustomerId, s2p.ProductId, s2p.StoreId
                       select s2p;
            }, cache => cache.PrepareKeyForShortTermCache(_s2pOrdersAllKey));

            var records = new PagedList<S2POrders>(rez, pageIndex, pageSize);

            return records;
        }

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
        public virtual async Task<IPagedList<S2POrders>> FindRecordsAsync(int customerId, int? productId, int? storeId, int? projectId, int? shoppingCartId, int pageIndex, int pageSize)
        {

            //filter by weight and shipping method
            var existingRecord = (await GetAllAsync())
                .ToList();

            //filter by Customer
            var matchedByCustomer = customerId == 0
                ? existingRecord
                : existingRecord.Where(r => r.CustomerId == customerId || r.CustomerId == 0);

            //filter by Product
            var matchedByProduct = productId == 0
                ? matchedByCustomer
                : matchedByCustomer.Where(r => r.ProductId == productId || r.ProductId == 0);

            //filter by Project
            var matchedByProject = (projectId == 0 || projectId == null)
                ? matchedByProduct
                : matchedByProduct.Where(r => r.ProjectId == projectId || r.ProjectId == 0);

            //filter by store
            var matchedByStore = (storeId == 0 || storeId == null)
                ? matchedByProject
                : matchedByProject.Where(r => r.StoreId == storeId || r.StoreId == 0);

            //filter by ShoppingCart
            var matchedByShoppingCart = (shoppingCartId == 0 || shoppingCartId == null)
                ? matchedByStore
                : matchedByStore.Where(r => r.ShoppingCartId  == shoppingCartId || r.ShoppingCartId == 0);

            //sort from particular to general, more particular cases will be the first
            var foundRecords = matchedByShoppingCart.OrderBy(r => r.CustomerId == 0).ThenBy(r => r.ProjectId == 0)
                .ThenBy(r => r.ProductId == 0)
                .ThenBy(r => r.StoreId == 0);

            var records = new PagedList<S2POrders>(foundRecords.ToList(), pageIndex, pageSize);

            return records;
        }

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
        public virtual async Task<S2POrders> FindRecordsAsync(int customerId, int? productId, int? storeId, int? projectId, OrderStatus status, int? shoppingCartId)
        {
            var foundRecords = await FindRecordsAsync(customerId, productId, storeId, projectId, shoppingCartId, 0, int.MaxValue);
            if(shoppingCartId != 0 && shoppingCartId != null)
            {
                var newrecord = foundRecords.Where(r => r.Status == status && r.ShoppingCartId == shoppingCartId);
                await _logger.InformationAsync($"s2p : ={newrecord.Count()}");
                return newrecord.FirstOrDefault();
            }
            else
            {
                var newrecord = foundRecords.Where(r => r.Status == status);
                await _logger.InformationAsync($"s2p : ={newrecord.Count()}");
                return newrecord.FirstOrDefault();
            }       
            
        }

        /// <summary>
        /// Get a S2POrders record by identifier
        /// </summary>
        /// <param name="s2pRecordId">Record identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the S2POrders record
        /// </returns>
        public virtual async Task<S2POrders> GetByIdAsync(int s2pRecordId)
        {
            return await _s2pRepository.GetByIdAsync(s2pRecordId);
        }

        /// <summary>
        /// Insert the S2POrders record
        /// </summary>
        /// <param name="s2pRecord">S2POrders record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertS2POrderRecordAsync(S2POrders s2pRecord)
        {
            await _s2pRepository.InsertAsync(s2pRecord, false);

            await _staticCacheManager.RemoveByPrefixAsync(S2PORDERS_PATTERN_KEY);
        }

        /// <summary>
        /// Update the S2POrders record
        /// </summary>
        /// <param name="s2pRecord">S2POrders record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateS2POrderRecordAsync(S2POrders s2pRecord)
        {
            await _s2pRepository.UpdateAsync(s2pRecord, false);

            await _staticCacheManager.RemoveByPrefixAsync(S2PORDERS_PATTERN_KEY);
        }

        /// <summary>
        /// Delete the S2POrders record
        /// </summary>
        /// <param name="s2pRecord">S2POrders record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteS2POrderRecordAsync(S2POrders s2pRecord)
        {
            await _s2pRepository.DeleteAsync(s2pRecord, false);

            await _staticCacheManager.RemoveByPrefixAsync(S2PORDERS_PATTERN_KEY);
        }

        #endregion
    }
}

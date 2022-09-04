using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using Nop.Services.Orders;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Soft2Print.Services;
using Nop.Web.Models.ShoppingCart;
using Nop.Web.Factories;
using Epp.Plugin.Soft2Print.Widget.Models;
using System.Collections.Generic;
using System;
using Nop.Services.Logging;

namespace Epp.Plugin.Soft2Print.Widget.Controllers
{
    public class Soft2PrintWidgetController : BasePluginController
    {
        #region Fields
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IS2POrdersService _s2pOrdersService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly ILogger _logger;
        #endregion

        #region Ctor

        public Soft2PrintWidgetController(IShoppingCartModelFactory shoppingCartModelFactory,
            IStoreContext storeContext,
            IWorkContext workContext,
              IShoppingCartService shoppingCartService,
            IS2POrdersService s2pOrdersService,
            ILogger logger)
        {
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _s2pOrdersService = s2pOrdersService;
            _workContext = workContext;
            _logger = logger;
        }

        #endregion


        #region Methods

        public async Task<JsonResult> GetData()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            var cartModel = new ShoppingCartModel();
            cartModel = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(cartModel, cart,
                isEditable: false);
            var model = new List<ShoppingCartInfoModel>();
            await _logger.InformationAsync($"Soft2PrintWidget called: cartModel.Items={cartModel.Items.Count}");
            foreach (var item in cartModel.Items)
            {
                var s2p = await _s2pOrdersService.FindRecordsAsync(customer.Id, Convert.ToInt32(item.ProductId), store.Id, null, Epp.Plugin.Soft2Print.Domain.OrderStatus.AddedToCart, item.Id);
                var additionalSheetCount = "";
                await _logger.InformationAsync($"s2p : ={s2p?.AdditionalSheetCount},cart id={item?.Id}");
                
                if (s2p != null)
                {
                    additionalSheetCount = s2p.AdditionalSheetCount;
                }
                await _logger.InformationAsync($"Soft2PrintWidget called: customerid={customer.Id}, storeid={store.Id}, ProductId={item.ProductId}, cart status={Epp.Plugin.Soft2Print.Domain.OrderStatus.AddedToCart}, additionalSheetCount={additionalSheetCount}");
                model.Add(new ShoppingCartInfoModel()
                {
                    CartId = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    AdditionalSheetCount = additionalSheetCount
                });
            }

            return Json(model);
        }

        #endregion
    }
}
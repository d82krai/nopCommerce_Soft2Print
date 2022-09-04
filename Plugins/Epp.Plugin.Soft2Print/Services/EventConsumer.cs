using System;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Messages;
using Nop.Services.Orders;

namespace Epp.Plugin.Soft2Print.Services
{
    /// <summary>
    /// Represents event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<EmailUnsubscribedEvent>,
        IConsumer<EntityInsertedEvent<ShoppingCartItem>>,
        IConsumer<EntityUpdatedEvent<ShoppingCartItem>>,
        IConsumer<EntityDeletedEvent<ShoppingCartItem>>,
        IConsumer<OrderPaidEvent>,
        IConsumer<OrderPlacedEvent>,
        IConsumer<EntityTokensAddedEvent<Store, Token>>,
        IConsumer<EntityTokensAddedEvent<Customer, Token>>
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IProductService _productService;
        private readonly Soft2PrintManager _soft2printManager;
        #endregion

        #region Ctor

        public EventConsumer(IGenericAttributeService genericAttributeService,
            Soft2PrintManager soft2printManager,
             IShoppingCartService shoppingCartService, IStoreContext storeContext,
              IWorkContext workContext, IProductService productService
             )
        {
            _genericAttributeService = genericAttributeService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
            _productService = productService;
            _soft2printManager = soft2printManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle the email unsubscribed event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EmailUnsubscribedEvent eventMessage)
        {
            //unsubscribe contact
            // await _soft2printEmailManager.UnsubscribeAsync(eventMessage.Subscription);
        }

        /// <summary>
        /// Handle the add shopping cart item event
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
        {
            //handle event
           // await _soft2PrintMarketingAutomationManager.HandleShoppingCartChangedEventAsync(eventMessage.Entity);        

        }

        /// <summary>
        /// Handle the update shopping cart item event
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
        {
            //handle event
           // await _soft2PrintMarketingAutomationManager.HandleShoppingCartChangedEventAsync(eventMessage.Entity);
        }

        /// <summary>
        /// Handle the delete shopping cart item event
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ShoppingCartItem> eventMessage)
        {
            //handle event
            //await _soft2PrintMarketingAutomationManager.HandleShoppingCartChangedEventAsync(eventMessage.Entity);
        }

        /// <summary>
        /// Handle the order paid event
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPaidEvent eventMessage)
        {
            //handle event
            await _soft2printManager.HandleOrderPaidEventAsync(eventMessage.Order);
        }

        /// <summary>
        /// Handle the order placed event
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            //handle event
            //await _soft2PrintMarketingAutomationManager.HandleOrderPlacedEventAsync(eventMessage.Order);

        }

        /// <summary>
        /// Handle the store tokens added event
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public Task HandleEventAsync(EntityTokensAddedEvent<Store, Token> eventMessage)
        {
            //handle event
            eventMessage.Tokens.Add(new Token("Store.Id", eventMessage.Entity.Id));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handle the customer tokens added event
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityTokensAddedEvent<Customer, Token> eventMessage)
        {
            //handle event
            var phone = await _genericAttributeService.GetAttributeAsync<string>(eventMessage.Entity, NopCustomerDefaults.PhoneAttribute);
            eventMessage.Tokens.Add(new Token("Customer.PhoneNumber", phone));
        }

        #endregion
    }
}
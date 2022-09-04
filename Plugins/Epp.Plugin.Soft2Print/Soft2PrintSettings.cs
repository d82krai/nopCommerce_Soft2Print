﻿using Nop.Core.Configuration;

namespace Epp.Plugin.Soft2Print
{
    /// <summary>
    /// Represents a plugin settings
    /// </summary>
    public class Soft2PrintSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the Organization Name
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Gets or sets the Organization Code
        /// </summary>
        public string OrganizationCode { get; set; }

        /// <summary>
        /// Gets or sets the Culture Code
        /// </summary>
        public string CultureCode { get; set; }

        /// <summary>
        /// Gets or sets the Customer Id
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use Testing
        /// </summary>
        public int Testing { get; set; }

        /// <summary>
        /// Gets or sets the Project Name Format
        /// </summary>
        public string ProjectNameFormat { get; set; }


        /// <summary>
        /// Gets or sets the identifier of list to synchronize contacts
        /// </summary>

        public int ListId { get; set; }
        /// <summary>
        /// Gets or sets the identifier of unsubscribe event webhook
        /// </summary>
        public int UnsubscribeWebhookId { get; set; }

        /// <summary>
        /// Gets or sets the SMTP key (Master password)
        /// </summary>
        public string SmtpKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use SMTP (for transactional emails)
        /// </summary>
        public bool UseSmtp { get; set; }

        /// <summary>
        /// Gets or sets the identifier of sender (for transactional emails)
        /// </summary>
        public string SenderId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of email account (for transactional emails)
        /// </summary>
        public int EmailAccountId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use SMS notifications
        /// </summary>
        public bool UseSmsNotifications { get; set; }

        /// <summary>
        /// Gets or sets the SMS sender name
        /// </summary>
        public string SmsSenderName { get; set; }

        /// <summary>
        /// Gets or sets the store owner phone number for SMS notifications
        /// </summary>
        public string StoreOwnerPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the Tracking script
        /// </summary>
        public string TrackingScript { get; set; }

        /// <summary>
        /// Gets or sets the Marketing Automation key (Tracker ID)
        /// </summary>
        public string MarketingAutomationKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use Marketing Automation
        /// </summary>
        public bool UseMarketingAutomation { get; set; } = true;

    }
}
// <copyright file="MarketplaceItemTransaction.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;

    public class MarketplaceItemTransaction
    {
        public int MarketplaceItemTransactionId { get; set; }

        public MarketplaceItem MarketplaceItem { get; set; }

        public User Sender { get; set; }

        public string SenderZuiderlingAccount { get; set; }

        public User Receiver { get; set; }

        public string ReceiverZuiderlingAccount { get; set; }

        public decimal Amount { get; set; }

        public MarketplaceItem.MarketplaceCurrency Currency { get; set; }
    }
}

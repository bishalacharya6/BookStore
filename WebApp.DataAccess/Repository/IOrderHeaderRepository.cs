﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Models;

namespace Web.DataAccess.Repository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);
        void UpdateStatus (int id, string orderStatus, string? paymentStatus = null);
        void UpdateStripePaymentStatus(int id, string sessionId, string paymentIntentId);
    }
}

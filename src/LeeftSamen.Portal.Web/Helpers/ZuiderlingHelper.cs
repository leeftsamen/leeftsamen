// <copyright file="ZuiderlingHelper.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Web;
using System.Threading.Tasks;
using System.Web.Configuration;
using LeeftSamen.Common.InterfaceText;
using LeeftSamen.Portal.Web.ZuiderlingAccess;
using LeeftSamen.Portal.Web.ZuiderlingPayment;

namespace LeeftSamen.Portal.Web.Helpers
{
    public class ZuiderlingHelper
    {
        private static string ServiceUsername
        {
            get { return WebConfigurationManager.AppSettings["ZuiderlingServiceUsername"]; }
        }

        private static string ServicePassword
        {
            get { return WebConfigurationManager.AppSettings["ZuiderlingServicePassword"]; }
        }

        private static ZuiderlingAccess.AccessWebServiceClient GetAccessService()
        {
            var service = new ZuiderlingAccess.AccessWebServiceClient();
            if (!string.IsNullOrEmpty(ServiceUsername) && !string.IsNullOrEmpty(ServicePassword) && service.ClientCredentials != null)
            {
                service.ClientCredentials.UserName.UserName = ServiceUsername;
                service.ClientCredentials.UserName.Password = ServicePassword;
            }

            return service;
        }

        private static ZuiderlingPayment.PaymentWebServiceClient GetPaymentService()
        {
            var service = new ZuiderlingPayment.PaymentWebServiceClient();
            if (!string.IsNullOrEmpty(ServiceUsername) && !string.IsNullOrEmpty(ServicePassword) && service.ClientCredentials != null)
            {
                service.ClientCredentials.UserName.UserName = ServiceUsername;
                service.ClientCredentials.UserName.Password = ServicePassword;
            }

            return service;
        }

        private static ZuiderlingAccount.AccountWebServiceClient GetAccountService()
        {
            var service = new ZuiderlingAccount.AccountWebServiceClient();
            if (!string.IsNullOrEmpty(ServiceUsername) && !string.IsNullOrEmpty(ServicePassword) && service.ClientCredentials != null)
            {
                service.ClientCredentials.UserName.UserName = ServiceUsername;
                service.ClientCredentials.UserName.Password = ServicePassword;
            }

            return service;
        }

        public AccountCredentialsStatus CheckCredentials(string username, string password)
        {
            var parameters = new ZuiderlingAccess.checkCredentialsParameters()
            {
                principal = username,
                credentials = password
            };
            try
            {
                using (var service = GetAccessService())
                {
                    return (AccountCredentialsStatus) service.checkCredentials(parameters);
                    //return await service.checkCredentialsAsync(parameters);
                }
            }
            catch (Exception e)
            {
                return AccountCredentialsStatus.Error;
            }
        }

        public async Task<ZuiderlingPayment.doPaymentResponse> DoPaymentAsync(string from, string to, decimal amount)
        {
            var parameters = new ZuiderlingPayment.paymentParameters()
            {
                fromMember = from,
                toMember = to,
                amount = amount,
                traceNumber = Guid.NewGuid().ToString(),
                amountSpecified = true,
                description = Text.ZuiderlingServiceTransactionDescription,
                currency = "ZL"
            };

            try
            {
                using (var service = GetPaymentService())
                {
                    return await service.doPaymentAsync(parameters);
                }
            }
            catch (Exception e)
            {
                return new ZuiderlingPayment.doPaymentResponse { @return = new paymentResult() { status = paymentStatus.UNKNOWN_ERROR } };
            }
        }

        public static decimal? GetCurrentBalance(string username)
        {
            var parameters = new ZuiderlingAccount.accountHistorySearchParameters
            {
                principal = username,
                pageSize = 0
            };

            try
            {
                using (var service = GetAccountService())
                {
                    var result =  service.searchAccountHistory(parameters);
                    return result.accountStatus.availableBalance;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public enum AccountCredentialsStatus
        {
            Valid,
            Invalid,
            Blocked,
            Error
        }
    }
}
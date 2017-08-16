// <copyright file="ISharedService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Enums;
using LeeftSamen.Portal.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Services
{
    public interface ISharedService
    {
        int GetNewActivitiesCount(User user);

        int GetNewCirclesActivityCount(User user);

        int GetNewForSaleCount(User user);

        int GetNewMealsCount(User user);

        int GetNewNeighborhoodMessageCount(User user);

        int GetNewNeighborHelpCount(User user);

        int GetNewOrganisationsCount(User user);

        int GetNewPublicCirclesCount(User user);

        int GetNewToBorrowCount(User user);

        void VisitPage(User user, PageVisitType page);

        DateTime LastVisitDate(User user, PageVisitType page);
    }
}

// <copyright file="SharedService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data;
using LeeftSamen.Portal.Data.Enums;
using LeeftSamen.Portal.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Services
{
    public class SharedService : ISharedService
    {
        private readonly IApplicationDbContext databaseContext;

        public SharedService(IApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public int GetNewActivitiesCount(User user)
        {
            var now = this.LastVisitDate(user, PageVisitType.Activities);
            return this.databaseContext.Activities.Count(a => a.Position.Distance(user.Position) < user.NeighborhoodRadius && a.CreationDate > now && a.Creator.Id != user.Id);
        }

        public int GetNewCirclesActivityCount(User user)
        {
            int circleactivities = 0;
            var circleIds = this.databaseContext.Circles.Where(c => c.Members.Any(m => m.User.Id == user.Id)).Select(c => c.CircleId).ToList();

            var circleDate = this.LastVisitDate(user, PageVisitType.Circles);
            circleactivities += this.databaseContext.CircleMessages.Count(m => circleIds.Contains(m.Circle.CircleId) && !m.IsHidden && m.CreationDateTime > circleDate && m.Creator.Id != user.Id);
            circleactivities += this.databaseContext.Activities.Count(a => circleIds.Contains(a.Circle.CircleId) && a.CreationDate > circleDate);
            circleactivities += this.databaseContext.Jobs.Count(j => circleIds.Contains(j.Circle.CircleId) && j.CreationDateTime > circleDate);
            circleactivities += this.databaseContext.MarketplaceItems.Count(j => j.ShowInCircleId.HasValue && circleIds.Contains(j.ShowInCircleId.Value) && j.CreationDateTime > circleDate);
            circleactivities += this.databaseContext.CircleEmailMessages.Count(m => circleIds.Contains(m.CircleId) && m.CreationDateTime > circleDate);

            return circleactivities;
        }

        public int GetNewForSaleCount(User user)
        {
            var now = this.LastVisitDate(user, PageVisitType.ForSale);
            return this.databaseContext.MarketplaceItems.Count(m => m.Category.Alias == MarketplaceItemCategory.CategoryAlias.Stuff && m.Position.Distance(user.Position) < user.NeighborhoodRadius && m.CreationDateTime > now && m.Owner.Id != user.Id);
        }

        public int GetNewMealsCount(User user)
        {
            var now = this.LastVisitDate(user, PageVisitType.NewMeals);
            return this.databaseContext.MarketplaceItems.Count(m => m.Category.Alias == MarketplaceItemCategory.CategoryAlias.Meals && m.Position.Distance(user.Position) < user.NeighborhoodRadius && m.CreationDateTime > now && m.Owner.Id != user.Id);
        }

        public int GetNewNeighborHelpCount(User user)
        {
            var now = this.LastVisitDate(user, PageVisitType.NeighborHelp);
            return this.databaseContext.MarketplaceItems.Count(m => m.Category.Alias == MarketplaceItemCategory.CategoryAlias.HelpNeighborhood && m.Position.Distance(user.Position) < user.NeighborhoodRadius && m.CreationDateTime > now && m.Owner.Id != user.Id);
        }

        public int GetNewNeighborhoodMessageCount(User user)
        {
            var now = this.LastVisitDate(user, PageVisitType.Activities);
            return this.databaseContext.NeighborhoodMessages.Count(a => a.Position.Distance(user.Position) < user.NeighborhoodRadius && a.CreationDateTime > now && a.Creator.Id != user.Id);
        }

        public int GetNewOrganisationsCount(User user)
        {
            var now = this.LastVisitDate(user, PageVisitType.Organizations);
            return this.databaseContext.Organizations.Count(a => a.RequestDateTime > now && !a.IsRequestPending);
        }

        public int GetNewPublicCirclesCount(User user)
        {
            var now = this.LastVisitDate(user, PageVisitType.PublicCircles);
            return this.databaseContext.Circles.Count(c => c.Position.Distance(user.Position) < user.NeighborhoodRadius && c.CreationDateTime > now && c.Creator.Id != user.Id);
        }

        public int GetNewToBorrowCount(User user)
        {
            var now = this.LastVisitDate(user, PageVisitType.ToBorrow);
            return this.databaseContext.MarketplaceItems.Count(m => m.Category.Alias == MarketplaceItemCategory.CategoryAlias.Borrowing && m.Position.Distance(user.Position) < user.NeighborhoodRadius && m.CreationDateTime > now && m.Owner.Id != user.Id);
        }

        public DateTime LastVisitDate(User user, PageVisitType page)
        {
            var item = this.databaseContext.UserPageVisits.FirstOrDefault(v => v.UserId == user.Id && v.Page == page.ToString());
            if (item != null)
            {
                return item.Date;
            }
            else
            {
                this.VisitPage(user, page);
            }

            return DateTime.Now;
        }

        public void VisitPage(User user, PageVisitType page)
        {
            var lastVisit = this.databaseContext.UserPageVisits.FirstOrDefault(v => v.UserId == user.Id && v.Page == page.ToString());
            if (lastVisit != null)
            {
                lastVisit.Date = DateTime.Now;
            }
            else
            {
                var visit = this.databaseContext.UserPageVisits.Create();
                visit.Date = DateTime.Now;
                visit.Page = page.ToString();
                visit.UserId = user.Id;
                this.databaseContext.UserPageVisits.Add(visit);
            }

            this.databaseContext.SaveChanges();
        }
    }
}

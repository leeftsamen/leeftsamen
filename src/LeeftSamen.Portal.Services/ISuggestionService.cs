// <copyright file="ISuggestionService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using LeeftSamen.Portal.Data.Models;

namespace LeeftSamen.Portal.Services
{
    public interface ISuggestionService
    {
        Task SendSuggestion(User user, string subject, string suggestion);
    }
}
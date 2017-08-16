// <copyright file="ISettingService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Services
{
    public interface ISettingService
    {
        Task<List<Setting>> GetSettings();

        Task<List<Setting>> GetSettingsByGroup(string group);

        Task<List<Setting>> GetSettingsByName(string name);

        Task<Setting> GetSettingById(int settingId);

        Task<List<UserSetting>> GetUserSettings();

        Task<List<UserSetting>> GetUserSettingsByUser(string userId);

        Task<UserSetting> GetUserSettingByUserAndSettingId(string userId, int settingId);

        Task<UserSetting> GetUserSettingByUserAndSetting(string userId, string group, string name);

        Task AddUserSetting(int settingId, string userId, string value);

        Task UpdateUserSettingValue(UserSetting setting, string value);
    }
}

// <copyright file="SettingService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeeftSamen.Portal.Data.Models;
using LeeftSamen.Portal.Data;
using System.Data.Entity;

namespace LeeftSamen.Portal.Services
{
    class SettingService : ISettingService
    {
        private readonly IApplicationDbContext databaseContext;

        public SettingService(IApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<Setting> GetSettingById(int settingId)
        {
            return await this.databaseContext.Settings.FirstOrDefaultAsync(s => s.SettingId == settingId).ConfigureAwait(false);
        }

        public async Task<List<Setting>> GetSettings()
        {
            return await this.databaseContext.Settings.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<Setting>> GetSettingsByName(string name)
        {
            return await this.databaseContext.Settings.Where(s => s.Name == name).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<Setting>> GetSettingsByGroup(string group)
        {
            return await this.databaseContext.Settings.Where(s => s.Group == group).ToListAsync().ConfigureAwait(false);
        }

        public async Task<UserSetting> GetUserSettingByUserAndSettingId(string userId, int settingId)
        {
            return await this.databaseContext.UserSettings.Where(s => s.UserId == userId && s.SettingId == settingId).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<UserSetting> GetUserSettingByUserAndSetting(string userId, string group, string name)
        {
            var setting = await this.databaseContext.Settings.FirstOrDefaultAsync(s => s.Group == group && s.Name == name).ConfigureAwait(false);
            if (setting == null)
            {
                return null;
            }

            return await this.databaseContext.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId && s.SettingId == setting.SettingId).ConfigureAwait(false);
        }

        public async Task<List<UserSetting>> GetUserSettings()
        {
            return await this.databaseContext.UserSettings.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<UserSetting>> GetUserSettingsByUser(string userId)
        {
            return await this.databaseContext.UserSettings.Where(s => s.UserId == userId).ToListAsync().ConfigureAwait(false);
        }

        public async Task AddUserSetting(int settingId, string userId, string value)
        {
            var setting = this.databaseContext.UserSettings.Create();
            setting.SettingId = settingId;
            setting.UserId = userId;
            setting.Value = value;
            this.databaseContext.UserSettings.Add(setting);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateUserSettingValue(UserSetting setting, string value)
        {
            setting.Value = value;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

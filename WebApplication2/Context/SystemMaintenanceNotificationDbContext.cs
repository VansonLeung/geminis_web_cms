using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Context
{
    public class SystemMaintenanceNotificationDbContext
    {
        // singleton

        private static SystemMaintenanceNotificationDbContext instance;

        public static SystemMaintenanceNotificationDbContext getInstance()
        {
            if (instance == null)
            {
                instance = new SystemMaintenanceNotificationDbContext();
            }
            return instance;
        }


        // initialization

        private BaseDbContext db = BaseDbContext.getInstance();

        protected DbSet<SystemMaintenanceNotification> getItemDb()
        {
            return db.systemMaintenanceNotificationDb;
        }



        // query

        public List<SystemMaintenanceNotification> findAllNotifications()
        {
            return getItemDb()
                .OrderByDescending(item => item.modified_at)
                .ToList();
        }

        public SystemMaintenanceNotification findNotificationByID(int ID)
        {
            return getItemDb()
                .Where(item => item.NotificationID == ID)
                .FirstOrDefault();
        }


        // edit

        public string createImmediateNotification(SystemMaintenanceNotification item)
        {
            item.isImmediate = true;
            var minutes = item.timePeriodMin;
            if (minutes <= 0)
            {
                return "Period must be at least 1 minute.";
            }
            var startDate = DateTime.Now;
            var endDate = startDate.AddMinutes(minutes);

            item.startDate = startDate;
            item.endDate = endDate;

            getItemDb().Add(item);
            db.SaveChanges();

            return null;
        }

        public string createScheduledNotification(SystemMaintenanceNotification item)
        {
            item.isScheduled = true;
            var minutes = item.timePeriodMin;
            var startDate = item.startDate;
            if (startDate == null)
            {
                return "Start Date must be set for creating scheduled notification.";
            }

            var endDate = startDate.GetValueOrDefault().AddMinutes(minutes);
            item.endDate = endDate;
            getItemDb().Add(item);
            db.SaveChanges();

            return null;
        }

        public string editNotification(SystemMaintenanceNotification item)
        {
            var local = getItemDb()
                            .Local
                            .FirstOrDefault(f => f.NotificationID == item.NotificationID);
            if (local != null)
            {
                db.Entry(local).State = EntityState.Detached;
            }

            db.Entry(item).State = EntityState.Modified;
            item.endDate = item.startDate.GetValueOrDefault().AddMinutes(item.timePeriodMin);
            db.SaveChanges();
            return null;
        }

        public string activateNotification(SystemMaintenanceNotification item)
        {
            db.Entry(item).State = EntityState.Modified;
            item.isActive = true;
            db.SaveChanges();
            return null;
        }

        public string deactivateNotification(SystemMaintenanceNotification item)
        {
            db.Entry(item).State = EntityState.Modified;
            item.isActive = false;
            db.SaveChanges();
            return null;
        }
    }
}
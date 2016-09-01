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
        
        public string createScheduledNotification(SystemMaintenanceNotification item)
        {
            var startDate = item.startDate;
            if (startDate == null)
            {
                return "Start Date must be set for creating scheduled notification.";
            }

            var endDate = item.endDate;
            if (endDate == null)
            {
                return "End Date must be set for creating scheduled notification.";
            }

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

            var startDate = item.startDate;
            if (startDate == null)
            {
                return "Start Date must be set for creating scheduled notification.";
            }

            var endDate = item.endDate;
            if (endDate == null)
            {
                return "End Date must be set for creating scheduled notification.";
            }

            db.Entry(item).State = EntityState.Modified;
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
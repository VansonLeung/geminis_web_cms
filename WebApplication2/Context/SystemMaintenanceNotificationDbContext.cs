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
        


        // query

        public List<SystemMaintenanceNotification> findAllNotifications()
        {
            using (var db = new BaseDbContext())
            {
                return db.systemMaintenanceNotificationDb
                .OrderByDescending(item => item.modified_at)
                .ToList();
            }
        }

        public SystemMaintenanceNotification findNotificationByID(int ID)
        {
            using (var db = new BaseDbContext())
            {
                return db.systemMaintenanceNotificationDb
                .Where(item => item.NotificationID == ID)
                .FirstOrDefault();
            }
        }

        public List<SystemMaintenanceNotification> findAllActivatedNotifications()
        {
            using (var db = new BaseDbContext())
            {
                var now = new DateTime();
                return db.systemMaintenanceNotificationDb
                    .Where(item => 
                    item.isActive == true
                    && item.startDate <= now
                    && item.endDate >= now
                    )
                .OrderBy(item => item.startDate)
                .ToList();
            }
        }

        // edit

        public string createScheduledNotification(SystemMaintenanceNotification item)
        {
            using (var db = new BaseDbContext())
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

                db.systemMaintenanceNotificationDb.Add(item);
                db.SaveChanges();
            }
            AuditLogDbContext.getInstance().createAuditLogSystemMaintenanceNotificationAction(item, AuditLogDbContext.ACTION_CREATE);
            return null;
        }

        public string editNotification(SystemMaintenanceNotification item)
        {
            using (var db = new BaseDbContext())
            {
                var local = db.systemMaintenanceNotificationDb
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
            }
            AuditLogDbContext.getInstance().createAuditLogSystemMaintenanceNotificationAction(item, AuditLogDbContext.ACTION_EDIT);
            return null;
        }

        public string activateNotification(SystemMaintenanceNotification item)
        {
            using (var db = new BaseDbContext())
            {
                db.Entry(item).State = EntityState.Modified;
                item.isActive = true;
                db.SaveChanges();
            }
            AuditLogDbContext.getInstance().createAuditLogSystemMaintenanceNotificationAction(item, AuditLogDbContext.ACTION_ACTIVATE);
            return null;
        }

        public string deactivateNotification(SystemMaintenanceNotification item)
        {
            using (var db = new BaseDbContext())
            {
                db.Entry(item).State = EntityState.Modified;
                item.isActive = false;
                db.SaveChanges();
            }
            AuditLogDbContext.getInstance().createAuditLogSystemMaintenanceNotificationAction(item, AuditLogDbContext.ACTION_DEACTIVATE);
            return null;
        }
    }
}
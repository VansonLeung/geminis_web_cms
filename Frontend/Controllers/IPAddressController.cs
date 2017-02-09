using WebApplication2.Context;
using WebApplication2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frontend.Controllers
{
    public class IPAddressController : BaseController
    {
    	int loginRetries = 5;
        // GET: User

        public IPAddress FindIPAddressRecord()
        {
            var db = IPAddressDbContext.getInstance().getItemDb();
            var ipaddress = GetIPAddress();
            IPAddress addr = null;
            if (ipaddress != null)
            {
                addr = db.AsNoTracking().Where(a => a.Address == ipaddress).FirstOrDefault();
            }
            return addr;
        }

        public IPAddress CreateNewIPAddressRecord()
        {
            var db = IPAddressDbContext.getInstance().getItemDb();
            var ipaddress = GetIPAddress();

            var newAddr = new IPAddress();
            newAddr.Address = ipaddress;
            newAddr.Failcount = 0;
            var id = db.Add(newAddr);
            IPAddressDbContext.getInstance().db.SaveChanges();
            return newAddr;
        }

        public IPAddress UpsertIPAddress()
        {
            IPAddress addr = FindIPAddressRecord();

            if (addr == null)
            {
                return CreateNewIPAddressRecord();
            }
            else
            {
                return addr;
            }
        }

        public IPAddress RegisterIPAddressFailLoginRetries()
        {
            var db = IPAddressDbContext.getInstance().db;

            IPAddress addr = UpsertIPAddress();

            // modify + 1 retry count

            db.Entry(addr).State = EntityState.Modified;
            addr.Failcount += 1;
            db.SaveChanges();

            return addr;
        }

        public int GetIPAddressStatus()
        {
            try
            {
                IPAddress addr = FindIPAddressRecord();

                if (addr != null && addr.Failcount < loginRetries)
                {
                    return 200;
                }

                return 403;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public IPAddress ClearFails()
        {
            try
            {
                IPAddress addr = FindIPAddressRecord();

                if (addr != null)
                {
                    var db = IPAddressDbContext.getInstance().db;

                    IPAddress _addr = UpsertIPAddress();

                    db.Entry(_addr).State = EntityState.Modified;
                    _addr.Failcount = 0;
                    db.SaveChanges();

                    addr = _addr;
                }

                return addr;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        
        public string GetIPAddress()
        {
            string VisitorsIPAddr = string.Empty;
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                VisitorsIPAddr = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (System.Web.HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                VisitorsIPAddr = System.Web.HttpContext.Current.Request.UserHostAddress;
            }
            
            return VisitorsIPAddr;
        }
        
    }
}
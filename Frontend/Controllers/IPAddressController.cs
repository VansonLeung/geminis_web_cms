using System;
using System.Collections.Generic;
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
                addr = db.Find(a => a.Address == ipaddress).FirstOrDefault();
            }
            else
            {
                throw exception no ip addr
            }
            return addr;
        }

        public IPAddress CreateNewIPAddressRecord()
        {
            var db = IPAddressDbContext.getInstance().getItemDb();
            var ipaddress = GetIPAddress();

            var newAddr = new IPAddress();
            newAddr.Address = ipaddress;
            newAddr.retries = 0;
            var id = db.Insert ( newAddr )
            return newAddr;
        }

        public IPAddress UpsertIPAddress()
        {
            try
            {
                IPAddress addr = FindIPAddressRecord();

                if (addr == null)
                {
                    return CreateNewIPAddressRecord();
                }

                return addr;            
            } 
            catch (Exception e)
            {
                // 
            }
        }

        public IPAddress RegisterIPAddressFailLoginRetries()
        {
            try
            {
                IPAddress addr = UpsertIPAddress();

                // modify + 1 retry count

                addr.retries += 1;

                var db = IPAddressDbContext.getInstance().getItemDb();
                db.save(addr);

                return addr;
            }
            catch (Exception e)
            {
                //
            }
        }

        public int GetIPAddressStatus()
        {
            try
            {
                IPAddress addr = FindIPAddressRecord();

                if (addr != null && addr.retries < loginRetries)
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
                    addr.retries = 0;

                    var db = IPAddressDbContext.getInstance().getItemDb();
                    db.save(addr);
                }

                return addr;
            }
            catch (Exception e)
            {
                return 0;
            }
        }


        public string GetIPAddress()
        {
            string VisitorsIPAddr = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
            }
            
            return VisitorsIPAddr;
        }
    }
}
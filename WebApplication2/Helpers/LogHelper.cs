using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Helpers
{
    public class LogHelper
    {
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Debug(object message = null, Exception e = null)
        {
            if (message == null && e != null)
            {
                message = e.Message;
            }
            if (message == null)
            {
                message = "";
            }
            logger.Debug(message, e);
        }

        public static void Info(object message = null, Exception e = null)
        {
            if (message == null && e != null)
            {
                message = e.Message;
            }
            if (message == null)
            {
                message = "";
            }
            logger.Info(message, e);
        }

        public static void Warn(object message = null, Exception e = null)
        {
            if (message == null && e != null)
            {
                message = e.Message;
            }
            if (message == null)
            {
                message = "";
            }
            logger.Warn(message, e);
        }

        public static void Error(object message = null, Exception e = null)
        {
            if (message == null && e != null)
            {
                message = e.Message;
            }
            if (message == null)
            {
                message = "";
            }
            logger.Error(message, e);
        }
    }
}
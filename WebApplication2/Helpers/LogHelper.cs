using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Helpers
{
    public class LogHelper
    {
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Debug(object message, Exception e)
        {
            logger.Debug(message, e);
        }

        public static void Info(object message, Exception e)
        {
            logger.Info(message, e);
        }

        public static void Warn(object message, Exception e)
        {
            logger.Warn(message, e);
        }

        public static void Error(object message, Exception e)
        {
            logger.Error(message, e);
        }
    }
}
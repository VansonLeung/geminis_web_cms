using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Context;
using WebApplication2.Helpers;

namespace Frontend.App_Start
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<SearchIndexingJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(7, 0))
                  )
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }


    public class SearchIndexingJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var data = LuceneSearchDataRepository.GetAll();
            LuceneSearch.AddUpdateLuceneIndex(LuceneSearchDataRepository.GetAll());
            AuditLogDbContext.getInstance().createAuditLog(new WebApplication2.Models.AuditLog
            {
                action = "[Search]",
                remarks = "Indexing data count: " + data.Count,
            });
        }
    }

}
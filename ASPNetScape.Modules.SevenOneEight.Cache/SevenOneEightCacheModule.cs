﻿using ASPNetScape.Abstractions.Interfaces.Cache;
using ASPNetScape.Modules.SevenOneEight.Cache.Cache;
using ASPNetScape.Modules.SevenOneEight.Cache.Cache.RuneTek5;
using Autofac;

namespace ASPNetScape.Modules.SevenOneEight.Cache
{
    public class SevenOneEightCacheModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileStore>().As<IFileStore>().As<IStartable>().SingleInstance();
            builder.RegisterType<RuneTek5Cache>().As<IReferenceTableCache>().SingleInstance();
        }
    }
}

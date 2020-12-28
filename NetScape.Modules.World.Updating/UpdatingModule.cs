﻿using Autofac;
using NetScape.Abstractions.Interfaces.World.Updating;
using NetScape.Abstractions.Model.Game;

namespace NetScape.Modules.World.Updating
{
    public class UpdatingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PlayerUpdater>()
                .As<IEntityUpdater<Player>>();
            base.Load(builder);
        }
    }
}

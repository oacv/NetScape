﻿using Autofac;
using NetScape.Abstractions.Model.Game;
using System;
using System.Collections.Generic;

namespace NetScape.Modules.World
{
    public interface IWorld : IStartable
    {
        List<Player> Players { get; }
        void Add(Player player);
        void Remove(Player player);
    }
}
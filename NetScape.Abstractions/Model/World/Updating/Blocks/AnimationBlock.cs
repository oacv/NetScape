﻿using NetScape.Abstractions.Model.World.Updating;

namespace NetScape.Abstractions.Model.World.Updating.Blocks
{
    public class AnimationBlock : SynchronizationBlock
    {
        public Animation Animation { get; }

        public AnimationBlock(Animation animation)
        {
            this.Animation = animation;
        }
    }
}
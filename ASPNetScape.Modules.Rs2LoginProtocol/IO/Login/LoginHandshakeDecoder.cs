﻿using ASPNetScape.Abstractions.Model.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Serilog;
using System.Collections.Generic;

namespace ASPNetScape.Modules.LoginProtocolThreeOneSeven.IO.Login
{
    public class LoginHandshakeDecoder : ByteToMessageDecoder
    {
        private readonly ILogger _logger;

        public LoginHandshakeDecoder(ILogger logger)
        {
            _logger = logger;
        }

        protected override void Decode(IChannelHandlerContext ctx, IByteBuffer buffer, List<object> output)
        {
            if (!buffer.IsReadable())
            {
                return;
            }

            int id = buffer.ReadByte();

            switch (id)
            {
                case (int)HandshakeType.SERVICE_GAME:
                    ctx.Channel.Pipeline.AddLast(nameof(LoginEncoder), new LoginEncoder());
                    ctx.Channel.Pipeline.AddAfter(nameof(LoginEncoder), nameof(LoginDecoder), new LoginDecoder(_logger));
                    ctx.Channel.Pipeline.AddAfter(nameof(LoginDecoder), nameof(LoginHandler), new LoginHandler());
                    break;

                //case HandshakeType.SERVICE_UPDATE:
                //    ctx.Channel.Pipeline.AddFirst("updateEncoder", new UpdateEncoder());
                //    ctx.Channel.Pipeline.AddBefore("handler", "updateDecoder", new UpdateDecoder());

                //    var buf = ctx.Allocator.Buffer(8).WriteLong(0);
                //    ctx.Channel.WriteAndFlushAsync(buf);
                //    break;

                default:
                    _logger.Information($"Unexpected handshake request received: {id}");
                    return;
            }

            ctx.Channel.Pipeline.Remove(this);
            var handshakeType = (HandshakeType)id;
            output.Add(handshakeType);
        }
    }
}

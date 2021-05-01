﻿using Autofac;
using Google.Protobuf;
using NetScape.Abstractions.Interfaces;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static NetScape.Modules.Messages.Models.ThreeOneSevenDecoderMessages.Types;

namespace NetScape.Modules.Messages
{
    public class MessageAttributeHandler : IDisposable
    {
        private readonly List<IDisposable> _subscriptions;
        private ContainerProvider _containerProvider;
        private readonly IMessageDecoder[] _decoders;
        
        public MessageAttributeHandler(ContainerProvider containerProvider, IMessageDecoder[] decoders)
        {
            _containerProvider = containerProvider;
            _decoders = decoders;
            _subscriptions = new();
        }

        public void Dispose()
        {
            _subscriptions.ForEach(t => t.Dispose());
        }

        public void Start()
        {
            var methods = Assembly.GetExecutingAssembly().GetTypes()
                       .SelectMany(t => t.GetMethods())
                       .Where(t => t.GetCustomAttribute<MessageAttribute>(false) != null)
                       .ToList();
            var classes = methods.Select(t => t.DeclaringType).Distinct().ToList();
            foreach (var clazz in classes)
            {
                var resolvedClazz = _containerProvider.Container.Resolve(clazz.GetTypeInfo().AsType());
                var subMethods = methods.Where(t => t.DeclaringType.FullName == clazz.FullName);
                foreach(var subscriptionMethod in subMethods)
                {
                    var customAttribute = subscriptionMethod.GetCustomAttribute<MessageAttribute>(false);
                    var messageDecoder = _decoders.First(decoder => decoder.TypeName == customAttribute.Type.Name);

                    var parameters = subscriptionMethod.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
                    Expression call = Expression.Call(Expression.Constant(resolvedClazz), subscriptionMethod, parameters);
                    Type actionType = typeof(Action<>).MakeGenericType(parameters[0].Type);
                    Delegate expressionDelegate = Expression.Lambda(actionType, call, parameters).Compile();
                    _subscriptions.Add(messageDecoder.SubscribeDelegate(expressionDelegate));
                    Serilog.Log.Logger.Debug($"Subscribed to {messageDecoder.GetType().Name} - {subscriptionMethod} for message {messageDecoder.TypeName}");
                }
            }
        }
    }


    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MessageHandlerAttribute : Attribute
    {
        public MessageHandlerAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class MessageAttribute : Attribute
    {
        public Type Type { get; set; }
        public MessageAttribute(Type type)
        {
            Type = type;
        }
    }
}
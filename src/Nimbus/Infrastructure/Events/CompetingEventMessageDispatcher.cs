﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Nimbus.Configuration.Settings;
using Nimbus.DependencyResolution;
using Nimbus.Handlers;
using Nimbus.Infrastructure.PropertyInjection;
using Nimbus.Infrastructure.TaskScheduling;
using Nimbus.Interceptors.Inbound;

namespace Nimbus.Infrastructure.Events
{
    internal class CompetingEventMessageDispatcher : EventMessageDispatcher
    {
        private readonly IPropertyInjector _propertyInjector;

        public CompetingEventMessageDispatcher(IBrokeredMessageFactory brokeredMessageFactory,
                                               IClock clock,
                                               IDependencyResolver dependencyResolver,
                                               IInboundInterceptorFactory inboundInterceptorFactory,
                                               IReadOnlyDictionary<Type, Type[]> handlerMap,
                                               DefaultMessageLockDurationSetting defaultMessageLockDuration,
                                               INimbusTaskFactory taskFactory,
                                               IPropertyInjector propertyInjector,
                                               ILogger logger)
            : base(brokeredMessageFactory, clock, dependencyResolver, handlerMap, inboundInterceptorFactory, logger, defaultMessageLockDuration, taskFactory)
        {
            _propertyInjector = propertyInjector;
        }

        protected override object CreateHandlerFromScope<TBusEvent>(IDependencyResolverScope scope, TBusEvent busEvent, Type handlerType, BrokeredMessage brokeredMessage)
        {
            var handler = (IHandleCompetingEvent<TBusEvent>)scope.Resolve(handlerType);
            _propertyInjector.Inject(handler, brokeredMessage);
            return handler;
        }

        protected override Task DispatchToHandleMethod<TBusEvent>(TBusEvent busEvent, object handler)
        {
            var genericHandler = (IHandleCompetingEvent<TBusEvent>) handler;
            var handlerTask = genericHandler.Handle(busEvent);
            return handlerTask;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nimbus.StressTests.ThroughputTests.MessageContracts;
using NUnit.Framework;

namespace Nimbus.StressTests.ThroughputTests
{
    [TestFixture]
    public class WhenSendingManyRequestsOfTheSameType : ThroughputSpecificationForBus
    {
        protected override int ExpectedMessagesPerSecond
        {
            get { return 50; }
        }

        protected override int NumMessagesToSend
        {
            get { return 4000; }
        }

        public override IEnumerable<Task> SendMessages(IBus bus)
        {
            for (var i = 0; i < NumMessagesToSend; i++)
            {
                yield return bus.Request(new FooRequest());
                Console.Write(".");
            }
            Console.WriteLine();
        }
    }
}
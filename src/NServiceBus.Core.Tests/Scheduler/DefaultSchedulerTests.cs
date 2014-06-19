﻿namespace NServiceBus.Scheduling.Tests
{
    using System.Threading;
    using Core.Tests.Fakes;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultSchedulerTests
    {
        FakeBus bus = new FakeBus();
        IScheduledTaskStorage taskStorage = new InMemoryScheduledTaskStorage();
        IScheduler scheduler;

        [SetUp]
        public void SetUp()
        {
            scheduler = new DefaultScheduler(bus, taskStorage);
        }

        [Test]
        public void When_scheduling_a_task_it_should_be_added_to_the_storage()
        {
            var task = new ScheduledTask();
            var taskId = task.Id;
            scheduler.Schedule(task);

            Assert.That(taskStorage.Get(taskId).Id, Is.EqualTo(taskId));
        }

        [Test]
        public void When_scheduling_a_task_defer_should_be_called()
        {
            scheduler.Schedule(new ScheduledTask());
            Assert.That(bus.DeferWasCalled > 0);
        }

        [Test]
        public void When_starting_a_task_defer_should_be_called()
        {
            var task = new ScheduledTask {Task = () => { }};
            var taskId = task.Id;

            scheduler.Schedule(task);

            var deferCount = bus.DeferWasCalled;
            scheduler.Start(taskId);
            
            Assert.That(bus.DeferWasCalled > deferCount);
        }

        [Test]
        public void When_starting_a_task_the_lambda_should_be_executed()
        {
            var i = 1;

            var task = new ScheduledTask { Task = () => { i++; } };
            var taskId = task.Id;

            scheduler.Schedule(task);            
            scheduler.Start(taskId);

            Thread.Sleep(100); // Wait for the task...

            Assert.That(i == 2);
        }
    }
}
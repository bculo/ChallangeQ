using AutoFixture;
using Events;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using QCode.Application.Services;
using System.Collections.Concurrent;

namespace QCode.Application.UnitTests.Services
{
    public class InMemoryExtractManagerTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly ILogger<InMemoryExtractManager> _logger = Substitute.For<ILogger<InMemoryExtractManager>>();

        [Fact]
        public void HandleEvent_ShouldNotThrowException_WhenEventInstanceProvided()
        {
            var _manager = new InMemoryExtractManager(_logger,
                new ConcurrentQueue<CreatePositionsReport>(),
                new ConcurrentQueue<PositionsReportBuildFailed>());
            var @event = _fixture.Create<CreatePositionsReport>();

            Action action = () => _manager.HandleEvent(@event);

            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void HandleEvent_ShouldThrowArgumentNullException_WhenNullProvided()
        {
            var _manager = new InMemoryExtractManager(_logger, 
                new ConcurrentQueue<CreatePositionsReport>(),
                new ConcurrentQueue<PositionsReportBuildFailed>());
            CreatePositionsReport? @event = null;

            Action action = () => _manager.HandleEvent(@event);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetEvent_ShouldBeNull_WhenEventQueueIsEmpty()
        {
            var _manager = new InMemoryExtractManager(_logger,
                new ConcurrentQueue<CreatePositionsReport>(),
                new ConcurrentQueue<PositionsReportBuildFailed>());

            var @event = _manager.GetEvent();

            @event.Should().BeNull();
        }

        [Fact]
        public void GetEvent_ShouldNotBeNull_WhenEventQueueIsNotEmpty()
        {
            var _manager = new InMemoryExtractManager(_logger,
                new ConcurrentQueue<CreatePositionsReport>(_fixture.CreateMany<CreatePositionsReport>(5)),
                new ConcurrentQueue<PositionsReportBuildFailed>());

            var @event = _manager.GetEvent();

            @event.Should().NotBeNull();
        }

        [Fact]
        public void HandleFailedAttemptEvent_ShouldNotThrowException_WhenEventInstanceProvided()
        {
            var _manager = new InMemoryExtractManager(_logger,
                new ConcurrentQueue<CreatePositionsReport>(),
                new ConcurrentQueue<PositionsReportBuildFailed>());
            var @event = _fixture.Create<PositionsReportBuildFailed>();

            Action action = () => _manager.HandleFailedAttemptEvent(@event);

            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void HandleFailedAttemptEvent_ShouldThrowArgumentNullException_WhenNullProvided()
        {
            var _manager = new InMemoryExtractManager(_logger,
                new ConcurrentQueue<CreatePositionsReport>(),
                new ConcurrentQueue<PositionsReportBuildFailed>());
            PositionsReportBuildFailed? @event = null;

            Action action = () => _manager.HandleFailedAttemptEvent(@event);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetFailedAttemptEvent_ShouldBeNull_WhenEventQueueIsEmpty()
        {
            var _manager = new InMemoryExtractManager(_logger,
                new ConcurrentQueue<CreatePositionsReport>(),
                new ConcurrentQueue<PositionsReportBuildFailed>());

            var @event = _manager.GetFailedAttemptEvent();

            @event.Should().BeNull();
        }

        [Fact]
        public void GetFailedAttemptEvent_ShouldNotBeNull_WhenEventQueueIsNotEmpty()
        {
            var _manager = new InMemoryExtractManager(_logger,
                new ConcurrentQueue<CreatePositionsReport>(),
                new ConcurrentQueue<PositionsReportBuildFailed>(_fixture.CreateMany<PositionsReportBuildFailed>(5)));

            var @event = _manager.GetFailedAttemptEvent();

            @event.Should().NotBeNull();
        }
    }
}

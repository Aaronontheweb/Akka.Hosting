using System;
using System.Threading.Tasks;
using Akka.Actor;
using FluentAssertions;
using Xunit;

namespace Akka.Hosting.TestKit.Tests;

public class Bugfix458Spec : TestKit
{
    private class EchoActor : ReceiveActor
    {
        public EchoActor()
        {
            ReceiveAny(msg => Sender.Tell(msg));
        }
    }
    
    protected override void ConfigureAkka(AkkaConfigurationBuilder builder, IServiceProvider provider)
    {
        builder.WithActors((system, registry, resolver) =>
        {
            var echoActor = system.ActorOf<EchoActor>("echo");
            registry.Register<EchoActor>(echoActor);
        });
    }

    [Fact]
    public void Bugfix458_should_resolve_TestActor_in_SynchronousCode()
    {
        var echo = ActorRegistry.Get<EchoActor>();
        echo.Tell("hello");
        ExpectMsg("hello");
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    public async Task Bugfix458_should_resolve_TestActor_in_AsynchronousCode(int _)
    {
        var echo = await ActorRegistry.GetAsync<EchoActor>();
        echo.Tell("hello");
        await ExpectMsgAsync("hello");
    }
}
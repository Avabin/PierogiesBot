using Core;

namespace Infrastructure;

public interface IEventsMediator
{
    IObservable<Delivery> GetObservableForTopic(string topic,  string brokerName               = "pub-sub");
    void                Publish(Delivery               @event, string topic, string brokerName = "pub-sub");
}
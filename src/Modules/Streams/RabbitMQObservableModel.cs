using System.Collections.Concurrent;
using System.Reactive.Disposables;
using Orleans.Streams;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Streams;

public class RabbitMQObservableModel : EventingBasicConsumer, IObservable<BasicDeliverEventArgs>
{
    private readonly ConcurrentQueue<BasicDeliverEventArgs> _queue = new();
    private readonly QueueId                                _queueId;

    public RabbitMQObservableModel(IModel model, QueueId queueId) : base(model)
    {
        _queueId = queueId;
    }

    public IDisposable Subscribe(IObserver<BasicDeliverEventArgs> observer)
    {
        void Handle(object? sender, BasicDeliverEventArgs args)
        {
            _queue.Enqueue(args);
            observer.OnNext(args);
        }

        var queue = _queueId.ToString();
        Received += Handle;
        Model.BasicConsume(queue, false, _queueId.ToStringWithHashCode(), this);
        return new CompositeDisposable(Disposable.Create(this,  (x) => x.Received -= Handle),
                                       Disposable.Create(queue, (q) => Model.BasicCancel(q)));
    }

    public IReadOnlyList<BasicDeliverEventArgs> Get(int max)
    {
        var list = new List<BasicDeliverEventArgs>();
        while (list.Count < max && _queue.TryDequeue(out var item)) list.Add(item);

        return list;
    }
}
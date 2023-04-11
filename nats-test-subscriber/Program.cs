// See https://aka.ms/new-console-template for more information

using System.Text;
using NATS.Client;
using NATS.Client.JetStream;

string subject = "test-subject";
string durable = "required-durable";

Console.WriteLine("NATS test subscriber");
Console.WriteLine("Running in pull mode");

// TODO: Change to pull mode

try
{

    Options options = ConnectionFactory.GetDefaultOptions();

    options.Url = "nats://localhost:4222";
    options.AllowReconnect = true;
    options.MaxReconnect = Options.ReconnectForever;
    options.PingInterval = 30000;
    

    options.ReconnectedEventHandler = (obj, args) =>
    {
        Console.WriteLine($"Reconnected to {args.Conn.ConnectedUrl}. New status: {args.Conn.State.ToString()}");
    };
    options.DisconnectedEventHandler = (obj, args) =>
    {
        Console.WriteLine($"Connection was disconnected. New status: {args.Conn.State.ToString()}");
    };
    options.ClosedEventHandler = (obj, args) =>
    {
        Console.WriteLine($"Connection was closed. New status: {args.Conn.State.ToString()}");
    };
    
    using IConnection c = new ConnectionFactory().CreateConnection(options);
    IJetStreamManagement jetStreamManagement = c.CreateJetStreamManagementContext();


    // create a JetStream context
    IJetStream js = c.CreateJetStreamContext();

    // For this, durable seems to be the queue group
    PullSubscribeOptions pullOptions = PullSubscribeOptions.Builder()
        .WithDurable(durable)
        .Build();

    IJetStreamPullSubscription sub = js.PullSubscribe(subject, pullOptions);
    
    c.Flush(500);

    while (true)
    {
        sub.Pull(1);
        try
        {
            //Msg? msg = sub.NextMessage(500);
            Msg? msg = sub.NextMessage(500);
            if (msg != null)
            {
                Console.WriteLine(Encoding.UTF8.GetString(msg.Data) + " id: " + msg.MetaData.StreamSequence);
                Task.Delay(200);
                msg.Ack();
            }
        }
        catch (NATSTimeoutException)
        {
            // timeout is acceptable, means no messages available.
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
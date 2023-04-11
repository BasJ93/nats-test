// See https://aka.ms/new-console-template for more information

using System.Text;
using NATS.Client;
using NATS.Client.JetStream;

string subject = "test-subject";
string deliverySubject = "test-delivery-subject";
string stream = "test-stream";

Console.WriteLine("NATS test publisher");

try
{
    using IConnection c = new ConnectionFactory().CreateConnection("localhost");
    IJetStreamManagement jetStreamManagement = c.CreateJetStreamManagementContext();

    StreamConfiguration streamConfiguration = StreamConfiguration.Builder()
        .WithName(stream)
        .WithStorageType(StorageType.Memory)
        .WithRetentionPolicy(RetentionPolicy.WorkQueue)
        .WithSubjects(new []{subject})
        .Build();

    StreamInfo streamInfo = jetStreamManagement.AddStream(streamConfiguration);
    Console.WriteLine("Created stream {0} with subject {1}", stream, subject);

    // create a JetStream context
    IJetStream js = c.CreateJetStreamContext();
        
    for (int i = 0; i < 1000; i++)
    {
        byte[] data = Encoding.UTF8.GetBytes("Test message " + i);

        Msg msg = new Msg(subject, data);

        PublishAck pa = js.Publish(msg);
            
        Console.WriteLine("Published message '{0}' on subject '{1}', stream '{2}', seqno '{3}'.",
            Encoding.UTF8.GetString(data), deliverySubject, pa.Stream, pa.Seq);
    }

    //Console.ReadKey();

    //jetStreamManagement.DeleteStream(stream);
}
catch(Exception ex)
{
    Console.WriteLine(ex);
}
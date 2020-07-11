using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;


namespace rabbitmq.test.RabbitMQ
{
    /// <summary>
    /// RabbitMq消息消费者
    /// </summary>
    public class RabbitMqSubscriber
    {
        private string exchangeName;
        private string queueName;
        private readonly IConnection connection;
        private readonly IModel channel;

        /// <summary>
        /// 初始化消费者
        /// </summary>
        /// <param name="uri">消息服务器地址</param>
        /// <param name="queueName">队列名</param>
        /// <param name="userName">用户</param>
        /// <param name="password">密码</param>
        /// <param name="exchangeName">交换机,有值表示广播模式</param>
        public RabbitMqSubscriber(string uri = "amqp://localhost:5672", string userName = "guest", string password = "guest", string exchangeName = "", string queue = "")
        {
            var factory = new ConnectionFactory() { Uri = new Uri(uri) };
            if (!string.IsNullOrWhiteSpace(exchangeName))
                this.exchangeName = exchangeName;
            if (!string.IsNullOrWhiteSpace(userName))
                factory.UserName = userName;
            if (!string.IsNullOrWhiteSpace(password))
                factory.Password = password;
            if (!string.IsNullOrWhiteSpace(queue))
                queueName = queue;
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();
        }

        public void Subscribe<TMessage>(Action<TMessage> callback = null)
        {
            Subscribe(null, null, callback);
        }

        public void Subscribe<TMessage>(string queue, Action<TMessage> callback = null)
        {
            Subscribe(null, queue, callback);
        }
        /// <summary>
        ///  触发消费行为
        /// </summary>
        /// <param name="queue">队列名称</param>
        /// <param name="callback">回调方法</param>
        public void Subscribe<TMessage>(string exchange, string queue, Action<TMessage> callback = null)
        {
            if (!string.IsNullOrWhiteSpace(exchange))
            {
                exchangeName = exchange;
            }
            // 使用自定义的队队
            if (!string.IsNullOrWhiteSpace(queue))
            {
                queueName = queue;
            }
            if (!string.IsNullOrWhiteSpace(queueName))//分发模式
            {
                channel.ExchangeDeclare(exchangeName, "topic");//广播
                channel.QueueDeclare(
                    queue: queueName,
                    durable: false,//持久化
                    exclusive: false, //独占,只能被一个consumer使用
                    autoDelete: false,//自己删除,在最后一个consumer完成后删除它
                    arguments: null);
                channel.QueueBind(queueName, exchangeName, queueName);

            }
            else
            {
                //广播模式
                channel.ExchangeDeclare(this.exchangeName, "fanout");//广播
                QueueDeclareOk queueOk = channel.QueueDeclare();//每当Consumer连接时，我们需要一个新的，空的queue,如果在声明queue时不指定,那么RabbitMQ会随机为我们选择这个名字
                queueName = queueOk.QueueName;//得到RabbitMQ帮我们取了名字
                channel.QueueBind(queueName, exchangeName, string.Empty);//不需要指定routing key，设置了fanout,指了也没有用.
            }
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body;
                var json = Encoding.UTF8.GetString(body.ToArray());
                callback(SerializeMemoryHelper.JsonDeserialize<TMessage>(json));
                channel.BasicAck(e.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
            queueName = null;
            Console.WriteLine(" [*] Waiting for messages." + "To exit press CTRL+C");

        }

    }
}

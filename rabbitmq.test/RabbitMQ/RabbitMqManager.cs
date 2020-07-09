namespace rabbitmq.test.RabbitMQ
{
    /// <summary>
    /// 队伍管理者
    /// </summary>
    public class RabbitMqManager
    {
        private readonly static RabbitMqPublisher rabbitMqPublisher;
        private readonly static RabbitMqSubscriber rabbitMqSubscriber;
        private static string mqHost = System.Configuration.ConfigurationManager.AppSettings["rabbitmqHost"];
        private static string mqUser = System.Configuration.ConfigurationManager.AppSettings["rabbitmqUser"];
        private static string mqPassword = System.Configuration.ConfigurationManager.AppSettings["rabbitmqPassword"];
        private static string rabbitmqExchange = System.Configuration.ConfigurationManager.AppSettings["rabbitmqExchange"];
        private static object lockObj = new object();

        static RabbitMqManager()
        {
            lock (lockObj)
            {
                if (rabbitMqPublisher == null)
                {
                    rabbitMqPublisher = new RabbitMqPublisher(mqHost, mqUser, mqPassword, rabbitmqExchange);
                }
                if (rabbitMqSubscriber == null)
                {
                    rabbitMqSubscriber = new RabbitMqSubscriber(mqHost, mqUser, mqPassword, rabbitmqExchange);
                }
            }

        }

        /// <summary>
        /// 发布者
        /// </summary>
        public static RabbitMqPublisher Publisher
        {
            get
            {
                return rabbitMqPublisher;
            }
        }

        /// <summary>
        /// 订阅者
        /// </summary>
        public static RabbitMqSubscriber Subscriber
        {
            get
            {
                return rabbitMqSubscriber;
            }
        }
    }
}

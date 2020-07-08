using rabbitmq.test.RabbitMQ;
using System;

namespace rabbitmq.test
{
    class Program
    {
        static void Main(string[] args)
        {
            //先订阅消息
            new RabbitMqSubscriber(queue: "lind").Subscribe<UserInfo>((userinfo) =>
            {
                Console.WriteLine("收到用户信息：" + userinfo.ToString());
            });

            //再程序里发布消息
            RabbitMqPublisher rabbitMqPublisher = new RabbitMqPublisher();
            rabbitMqPublisher.Publish<UserInfo>("lind", new UserInfo
            {
                UserID = 1,
                UserName = "zhangzhanling"
            });

        }
    }
}

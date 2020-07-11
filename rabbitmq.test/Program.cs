using rabbitmq.test.RabbitMQ;
using System;

namespace rabbitmq.test
{
    class Program
    {
        static void Main(string[] args)
        {

            fanout();

            Console.ReadKey();

        }

        static void topic()
        {
            //先订阅消息
            RabbitMqManager.Subscriber.Subscribe<UserInfo>("lind", (userinfo) =>
            {
                Console.WriteLine("收到用户信息1：" + userinfo.ToString());
            });


            //先订阅消息
            RabbitMqManager.Subscriber.Subscribe<UserInfo>("lind", (userinfo) =>
            {
                Console.WriteLine("收到用户信息2：" + userinfo.ToString());
            });


            //再程序里发布消息
            RabbitMqManager.Publisher.Publish("lind", new UserInfo
            {
                UserID = 1,
                UserName = "zhangzhanling"
            });
        }

        static void fanout()
        {
            //先订阅消息
            RabbitMqManager.Subscriber.Subscribe<UserInfo>( callback:(userinfo) =>
            {
                Console.WriteLine("收到用户信息1：" + userinfo.ToString());
            });


            //先订阅消息
            RabbitMqManager.Subscriber.Subscribe<UserInfo>(callback: (userinfo) =>
            {
                Console.WriteLine("收到用户信息2：" + userinfo.ToString());
            });


            //再程序里发布消息
            RabbitMqManager.Publisher.PublishFanout(new UserInfo
            {
                UserID = 1,
                UserName = "zhangzhanling"
            });
        }
    }
}

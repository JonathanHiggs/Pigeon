namespace Pigeon.ActiveMQ
{
    public class SubjectMapper
    {
        public string GetTopicName<TTopic>() => typeof(TTopic).FullName;


        public string GetTopicName(object obj) => obj.GetType().FullName;
    }
}

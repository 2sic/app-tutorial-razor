using Custom.DataSource;

namespace AppCode.Extensions.HelloWorld
{
    public class HelloWorldDataSource : DataSource16
    {
        public HelloWorldDataSource(Dependencies services) : base(services)
        {
            ProvideOut(GetData);
        }

        private object GetData()
        {
            return new
            {
                Message = "Hello from my DataSource"
            };
        }
    }
}
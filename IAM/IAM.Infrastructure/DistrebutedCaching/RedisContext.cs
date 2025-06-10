using IAM.Application.Common.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDatabase = StackExchange.Redis.IDatabase;

namespace IAM.Infrastructure.DistrebutedCaching
{
    public class RedisContext : ICachingContext
    {
        static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:2028");
        private readonly IDatabase db = redis.GetDatabase();

        public string Get(string email)
        {
            return db.StringGet(email);
        }

        public void Set(string email, string code)
        {
            TimeSpan timeSpan = TimeSpan.FromMinutes(2);
            bool a = db.StringSet(email, code, timeSpan);
            Console.WriteLine(a);
        }
    }
}

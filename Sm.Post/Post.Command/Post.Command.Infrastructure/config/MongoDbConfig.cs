using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Post.Command.Infrastructure.config;

public class MongoDbConfig
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string Collection { get; set; }

}
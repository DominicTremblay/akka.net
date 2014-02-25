﻿using Pigeon.Actor;
using Pigeon.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Routing
{
     public interface ConsistentHashable {
         object ConsistentHashKey { get; }
    }

     public class ConsistentHashingRoutingLogic : RoutingLogic
     {
         public override Routee Select(object message, Routee[] routees)
         {             
             if (message is ConsistentHashable)
             {
                 var hashable = (ConsistentHashable)message;
                 var hash = hashable.ConsistentHashKey.GetHashCode();
                 return routees[hash % routees.Length];
             }

             throw new NotSupportedException("Only ConsistentHashable messages are supported right now");
         }
     }
     public class ConsistentHashingGroup : Group
     {
         public ConsistentHashingGroup(Config config)
             : base(config.GetStringList("routees.paths"))
         { }
         public ConsistentHashingGroup(params string[] paths)
             : base(paths)
         { }
         public ConsistentHashingGroup(IEnumerable<string> paths)
             : base(paths)
         { }

         public ConsistentHashingGroup(IEnumerable<ActorRef> routees) : base(routees) { }

         public override Router CreateRouter()
         {
             return new Router(new ConsistentHashingRoutingLogic());
         }
     }
}

using AlertReset.Class;
using AlertReset.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertReset
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() > 0)
            {
                IApplication application = new AlertManagement();
                application.Execute(args[0].Split('|'));
            }
        }
    }
}

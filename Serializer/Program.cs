using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class testClass
{
    public int testInt { get; set; }
    public string testString { get; set; }
    public bool testBool { get; set; }
    public double testDouble { get; set; }
}

namespace Serializer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string serialized = ObjectSerializer.SerializeObject(new testClass {
                testInt = 1,
                testString = "test",
                testBool = true, testDouble = 1.0
            });

            Console.WriteLine(serialized);
            Console.ReadKey();
        }
    }
}

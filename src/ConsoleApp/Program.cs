using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    static class Program
    {
        static async Task Main()
        {
            while (true)
            {
                Console.WriteLine("1: Test client credentials");
                Console.WriteLine("2: Test authorization code with browser login");
                Console.WriteLine("Any other key: Exit");

                var option = Console.ReadKey();
                if (option.KeyChar == '1')
                {
                    await ClientCredentials.Test();
                }
                else if (option.KeyChar == '2')
                {
                    await AuthorizationCode.Test();
                }
                else
                {
                    break;
                }
            }
        }
    }
}

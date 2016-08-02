using System;
using System.ServiceModel;
//using Client;


// ReSharper disable once CheckNamespace
namespace Server
{
    partial class Program
    {
        static void Main(string[] args)
        {

            Console.Title = "SERVER";
           
            var service = new ServiceHost(typeof(Service));


                service.Open();

            Console.WriteLine("Service start in " + DateTime.Now);
            using (var db = new ConnectToDb())
            {
              //  TemplateDb();

              // AddNormDb();
                db.SaveChanges();
            }


            #region e
            string exit;
            while (true)
            {
                exit = Console.ReadLine().ToString();
                if (exit == "e")
                {
                   
                    Console.WriteLine("Service close");
                    break;
                }
            }
            #endregion
            service.Close();
        }
    }
}


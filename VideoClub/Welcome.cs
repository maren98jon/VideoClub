using System;
using System.Configuration;
using System.Data.SqlClient;

namespace VideoClub
{
    public class Welcome
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["VideoClub"].ConnectionString;
        static SqlConnection connection = new SqlConnection(connectionString);

        public static void WelcomeScrn()
        {            
            int option;
            bool exit = false;            
            while (!exit)
            {
                try
                {
                    Users userName = new Users();
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine(@"
        ██╗   ██╗██╗██████╗ ███████╗ ██████╗  ██████╗██╗     ██╗   ██╗██████╗     ██████╗ ██████╗ ██╗  ██╗      
        ██║   ██║██║██╔══██╗██╔════╝██╔═══██╗██╔════╝██║     ██║   ██║██╔══██╗    ██╔══██╗██╔══██╗██║ ██╔╝      
        ██║   ██║██║██║  ██║█████╗  ██║   ██║██║     ██║     ██║   ██║██████╔╝    ██████╔╝██████╔╝█████╔╝       
        ╚██╗ ██╔╝██║██║  ██║██╔══╝  ██║   ██║██║     ██║     ██║   ██║██╔══██╗    ██╔══██╗██╔══██╗██╔═██╗       
         ╚████╔╝ ██║██████╔╝███████╗╚██████╔╝╚██████╗███████╗╚██████╔╝██████╔╝    ██████╔╝██████╔╝██║  ██╗      
          ╚═══╝  ╚═╝╚═════╝ ╚══════╝ ╚═════╝  ╚═════╝╚══════╝ ╚═════╝ ╚═════╝     ╚═════╝ ╚═════╝ ╚═╝  ╚═╝      
");

                    Console.ResetColor();
                    Console.WriteLine("\nPulse '1' Si ya tiene cuenta y desea loguearse.\nPulse '2' Si es nuevo y desea registrarse.\nPulse '3' Si desea SALIR.");
                    option = Int32.Parse(Console.ReadLine());
                    switch (option)
                    {
                        case 1:
                            if (userName.LogIn())
                            {
                                Console.Clear();
                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.WriteLine(@"
        ██╗   ██╗██╗██████╗ ███████╗ ██████╗  ██████╗██╗     ██╗   ██╗██████╗     ██████╗ ██████╗ ██╗  ██╗      
        ██║   ██║██║██╔══██╗██╔════╝██╔═══██╗██╔════╝██║     ██║   ██║██╔══██╗    ██╔══██╗██╔══██╗██║ ██╔╝      
        ██║   ██║██║██║  ██║█████╗  ██║   ██║██║     ██║     ██║   ██║██████╔╝    ██████╔╝██████╔╝█████╔╝       
        ╚██╗ ██╔╝██║██║  ██║██╔══╝  ██║   ██║██║     ██║     ██║   ██║██╔══██╗    ██╔══██╗██╔══██╗██╔═██╗       
         ╚████╔╝ ██║██████╔╝███████╗╚██████╔╝╚██████╗███████╗╚██████╔╝██████╔╝    ██████╔╝██████╔╝██║  ██╗      
          ╚═══╝  ╚═╝╚═════╝ ╚══════╝ ╚═════╝  ╚═════╝╚══════╝ ╚═════╝ ╚═════╝     ╚═════╝ ╚═════╝ ╚═╝  ╚═╝      
");
                                Console.ResetColor();
                                Menu.FirstMenu(userName);                                
                            }
                            break;
                        case 2:
                            Users newUser = new Users();
                            newUser.Register();
                            Console.WriteLine("Pulse 1 si desea continuar o pulse otra tecla para salir.");
                            try
                            {
                                int num = Int32.Parse(Console.ReadLine());
                                if (num != 1)   
                                {
                                    exit = true;
                                }                                
                            }
                            catch
                            {
                                exit = true;
                            }
                            break;
                        case 3:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("No ha introducido una opción correcta.");
                            break;
                    }                    
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine(ex.Message);
                    
                }
            }

            Console.WriteLine("Muchas gracias por usar este servicio. Hasta la proxima!");
        }

        
    }
}

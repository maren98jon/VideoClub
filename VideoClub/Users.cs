using System;
using System.Configuration;
using System.Data.SqlClient;

namespace VideoClub
{
    public class Users
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["VideoClub"].ConnectionString;
        static SqlConnection connection = new SqlConnection(connectionString);
        static SqlCommand command;
        static SqlDataReader user;


        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Mail { get; set; }
        public string PassWord { get; set; }

        public Users()
        {
        }

        public bool LogIn() //cuando exit sea false quiero darle la opcion de salir, y en el welcome tengo que ponerle una salida sin ir al menu.
        {  //doble autentificacion. Si fallas 3 veces tienes que poner tu mail tu pass y tu fecha nacimiento
            string mail;
            string passWord;
            bool exit;
            do
            {
                Console.Write("Introduzca su mail: ");//comprobar los dos a la vez
                mail = Console.ReadLine(); //podria poner CheckMail(COnsole.readLINEW????                 
                Console.Write("Introduzca su contraseña: ");
                //passWord = Console.ReadLine();
                passWord = ReadPassword();
                exit = (CheckMail(mail.ToLower()) && CheckPassWord(passWord)); //dentro del  metodo tendria que meter el Mail
                if (!exit)
                {
                    Console.WriteLine("El mail y/o contraseña que ha introducido no son correctos.");
                    Console.WriteLine("Pulse '1' si desea volver a intentarlo o pulse otra tecla si desea salir a la pantalla principal.");
                    try
                    {
                        switch (Int32.Parse(Console.ReadLine()))
                        {
                            case 1:
                                break;
                                
                            default:
                                Console.WriteLine("Saliendo a la pantalla principal.");
                                return false;                                
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Saliendo a la pantalla principal.");
                        return false;
                    }
                }
            } while (!exit);
            return true;                      
        }

        public void Register()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Introduzca su mail: ");//comprobar los dos a la vez                
                string mail = Console.ReadLine();                
                mail = mail.ToLower();                
                if (mail.IndexOf("@") > 0 && (mail.IndexOf(".") > mail.IndexOf("@")))
                {
                    if (!CheckMail(mail)) //comprobamos que no esté en la base de datos
                    {
                        Mail = mail;
                        Console.WriteLine("Introduzca una contraseña: ");//me gustaría que no se viera la pass y que tuviera una mayusc y caracteres
                        
                        PassWord = Console.ReadLine();
                        
                        if (SecurePassWord())
                        {
                            Console.WriteLine("Introduzca su nombre: ");                            
                            string name = Console.ReadLine();
                            name = name.ToLower();
                            name = name.Replace(name.Substring(0,1),name.Substring(0,1).ToUpper());
                            name = name.Replace(name.Substring(1), name.Substring(1).ToLower());
                            Name = name;                            

                            Console.WriteLine("Introduzca su primer apellido: "); //registar la primera en mayusc                            
                            string lastName = Console.ReadLine();
                            lastName = lastName.ToLower();
                            lastName = lastName.Replace(lastName.Substring(0, 1), lastName.Substring(0, 1).ToUpper());
                            lastName = lastName.Replace(lastName.Substring(1),lastName.Substring(1).ToLower());
                            LastName = lastName;
                            
                            bool tryBirthDate = false;
                            while (!tryBirthDate)
                            {
                                try
                                {
                                    Console.WriteLine("Introduzca su fecha de nacimiento formato dd/mm/aaaa: ");                                    
                                    BirthDate = Convert.ToDateTime(Console.ReadLine());                                    
                                    if (BirthDate < DateTime.Today)
                                    {
                                        tryBirthDate = true;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Fecha de nacimiento incorrecta");
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                }
                                catch
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Formato de fecha de nacimento no válido");
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                            }
                            connection.Open();
                            command = new SqlCommand($"INSERT INTO USERS (Name,LastName,BirthDate,Mail,Password) VALUES ('{Name}','{LastName}',CONVERT(DATE,'{BirthDate}',105),'{Mail}','{PassWord}')", connection);
                            command.ExecuteNonQuery();
                            connection.Close();
                            Console.WriteLine($"Bienvenid@ {Name} al videoclub BOOM");
                            exit = true;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Su contraseña debe tener entre 6 y 16 carácteres. Además debe contener mínimo una mayúscula, una minúscula, un número y un caracter no alfa-numérico por lo menos.");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("Pulse '1' si desea volver a intentarlo u otra tecla para salir.");
                            if (Int32.Parse(Console.ReadLine())!=1)
                            {
                                exit = true;
                            }
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Este mail ya tiene una cuenta.");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Pulse '1' si desea volver a intentarlo u otra tecla para salir.");
                        if (Int32.Parse(Console.ReadLine()) != 1)
                        {
                            exit = true;
                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Formato de mail incorrecto.");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Pulse '1' si desea volver a intentarlo u otra tecla para salir.");
                    if (Int32.Parse(Console.ReadLine()) != 1)
                    {
                        exit = true;
                    }
                }
            }

        }

        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password;
        }
    

    private bool SecurePassWord()
        {
            bool hasNum = false;
            bool hasCap = false;
            bool hasLow = false;
            bool hasSpec = false;
            char currentChar;
            if (PassWord.Length < 16 && PassWord.Length >5)
            {
                for (int i = 0; i < PassWord.Length; i++)
                {
                    currentChar = PassWord[i];
                    if (char.IsDigit(currentChar))
                    {
                        hasNum = true;
                    }
                    else if (char.IsUpper(currentChar))
                    {
                        hasCap = true;
                    }
                    else if (char.IsLower(currentChar))
                    {
                        hasLow = true;
                    }
                    else if (!char.IsLetterOrDigit(currentChar))
                    {
                        hasSpec = true;
                    }                    
                }
            }            
            if (hasNum && hasCap && hasCap && hasLow && hasSpec)
            {
                return true;
            }
            return false;
        }

        public bool CheckMail(string mail)
        {
            bool varReturn = false;
            if (mail.IndexOf("@") > 0 && mail.IndexOf(".") > mail.IndexOf("@"))// compruebo que el mail esté introducido bien para no 
            { //tendré que buscar el mail en la base de datos                     tener que llamar a la bbdd innecesariamente
                connection.Open();
                command = new SqlCommand("SELECT * FROM USERS", connection);
                user = command.ExecuteReader();
                while (user.Read())
                {
                    if (Convert.ToString(user["Mail"]) == mail) //compruebo si el mail está en la base de datos
                    {
                        Mail = mail;
                        Name = Convert.ToString(user["Name"]);
                        LastName = Convert.ToString(user["LastName"]);
                        BirthDate = Convert.ToDateTime(user["BirthDate"]);
                        PassWord = Convert.ToString(user["PassWord"]);

                        varReturn = true;
                    }
                }
                connection.Close();                
                return varReturn;
            }
            else
            {
                return varReturn;
            }            
        }

        public bool CheckPassWord(string passWord)
        {
            if (PassWord == passWord) //compruebo si la pass está en la base de datos
            {
                return true;
            }
            else
            {                
                return false;
            }            
        }        
    }
}

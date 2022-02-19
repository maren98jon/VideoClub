using System;
using System.Collections.Generic; //por si vuelvo a usar listas
using System.Configuration;
using System.Data.SqlClient;

namespace VideoClub
{
    public class Menu
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["VideoClub"].ConnectionString;
        static SqlConnection connection = new SqlConnection(connectionString);
        static SqlCommand command;
        static SqlDataReader film;
        static SqlDataReader rent;

        public static void FirstMenu(Users userName)
        {
            int option;
            bool exit = false;
            Console.WriteLine(@"
          __  __ ___ _  _ _   _ 
         |  \/  | __| \| | | | |
         | |\/| | _|| .` | |_| |
         |_|  |_|___|_|\_|\___/  

");
            Console.WriteLine($"\nBienvenid@ {userName.Name}. ¿Qué desea hacer?.");
            while (!exit)
            {
                try
                {
                    Console.WriteLine("\n\t1- Ver peliculas disponibles.\n\t2- Alquilar película." +
                                    "\n\t3- Mis alquileres.\n\t4- Cerrar sesión.");
                    option = Int32.Parse(Console.ReadLine());

                    switch (option)
                    {
                        case 1:
                            ShowAvailableFilms(userName);
                            break;

                        case 2:
                            RentFilm(userName);
                            break;

                        case 3:
                            ShowMyRentals(userName);
                            break;

                        case 4:
                            exit = true;
                            break;

                        default:
                            Console.WriteLine("No ha introducido una opción correcta.");

                            break;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR! Debe introducir un número.");
                    Console.WriteLine(ex.Message);
                }
            }            
        }



        private static void ShowAvailableFilms(Users userName)
        {
            Console.WriteLine("Las peliculas que tienes disponibles en este momento son:"); //me gustaria poner la opcion de ver la sinopsis de la pelicula que quiera
            connection.Open();
            command = new SqlCommand("SELECT * FROM FILMS", connection);
            film = command.ExecuteReader();
            while (film.Read())
            {
                if (Convert.ToInt32(film["Age"]) <= UserAge(userName.BirthDate)) //siempre que cojo datos de las bbdd tengo que convertirlo??
                { //me gustaría mostrar las NO DISPONIBLES en rojo y decirle cuando se deberían librar.
                    if (Convert.ToChar(film["available"]) == 'N')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"\n\tID: {film["Id"]} | Pelicula: {film["Title"]} ");
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write("NO DISPONIBLE");
                        Console.ResetColor();
                        Console.WriteLine();

                    }
                    else
                    {
                        Console.WriteLine($"\n\tID: {film["Id"]} Pelicula: {film["Title"]}");//Tenia puesto mostrar la edad recomend lo pongo en mostar mas datos
                    }

                }
            }
            connection.Close(); //no puedo declarar dos veces el mismo objeto, al ser comando se puede??
            bool exit = false; ;
            while (!exit)
            {
                try
                {
                    Console.WriteLine("\nIntroduzca el Id de la pelicula de la que desee mas información o pulse una tecla no númerica para continuar: ");
                    int option = Int32.Parse(Console.ReadLine());
                    connection.Open();
                    command = new SqlCommand("SELECT * FROM FILMS", connection);
                    film = command.ExecuteReader();
                    while (film.Read())
                    {
                        if (Convert.ToInt32(film["Age"]) <= UserAge(userName.BirthDate) && Convert.ToInt32(film["Id"])==option) 
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine($"\nPelicula para mayores de: {film["Age"]} \nSinopsis: {film["Synopsis"]}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    connection.Close();
                }
                catch
                {
                    exit = true;
                }
            }
        }

        private static void RentFilm(Users userName)
        {
            int filmId = 0;
            bool exit = false;
            while (!exit)
            {
                try
                {
                    Console.WriteLine("Introduzca el numero de identificación de la pelicula que desea alquilar.");
                    filmId = Int32.Parse(Console.ReadLine());
                    exit = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Debe introducir un número.");
                    Console.WriteLine("Pulse 1 si desea continuar o pulse otra tecla si quiere salir.");
                    try
                    {
                        int option = Int32.Parse(Console.ReadLine());
                        if (option != 1)
                        {
                            exit = true;
                        }
                    }
                    catch (Exception ex1)
                    {
                        Console.WriteLine("No es válido");
                        exit = true;
                    }
                }
            }
            if (filmId > 0) //esto lo he hecho por poder hacer una salida pal lobby
            {
                bool available = false;
                connection.Open();
                command = new SqlCommand($"SELECT * FROM FILMS WHERE ID = '{filmId}'", connection);
                film = command.ExecuteReader();
                while (film.Read())
                {
                    if (Convert.ToInt32(film["Age"]) <= UserAge(userName.BirthDate))
                    {
                        if (film["Available"].ToString() == "Y")
                        {
                            available = true;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"La película {film["Title"]} ya está alquilada a otro usuario.");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"No tienes edad para alquilar la película {film["Title"]}");
                        Console.ForegroundColor = ConsoleColor.White;

                    }
                }
                connection.Close();
                if (available)
                {
                    connection.Open();  //ponemos no disponible
                    command = new SqlCommand($"UPDATE FILMS SET AVAILABLE = 'N' WHERE ID = '{filmId}';" +
                                             $"INSERT INTO RENTALS (idFilm,UserMail,RentDate) VALUES ('{filmId}','{userName.Mail}',convert(date, '{DateTime.Today}', 105))",connection);
                    command.ExecuteNonQuery();
                    connection.Close();

                    //connection.Open(); //creamos nuevo alquiler con todos los datos
                    //command = new SqlCommand($"INSERT INTO RENTALS (idFilm,UserMail,RentDate) VALUES ('{filmId}','{userName.Mail}',convert(date, '{DateTime.Today}', 105))",connection);
                    //command.ExecuteNonQuery();
                    //connection.Close();


                    connection.Open();
                    command = new SqlCommand($"SELECT * FROM FILMS WHERE ID = '{filmId}'", connection);
                    film = command.ExecuteReader();
                    while (film.Read())
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Ha alquilado la pelicula {film["Title"]}");
                        Console.WriteLine($"Tiene hasta el dia {(DateTime.Today.AddDays(3)).ToString("dddd, dd 'de' MMMM 'de' yyyy")} para entregar la película");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    connection.Close();                    
                }
            }
        }

        private static void ShowMyRentals(Users userName)
        {
            bool haveFilm = false;
            Rentals myRent = new Rentals();
            List<Rentals> myRentals = new List<Rentals>();
            Console.WriteLine("Estas son las películas que ha alquilado");
            Console.WriteLine();
            connection.Open();
            //command = new SqlCommand($"SELECT * FROM RENTALS WHERE UserMail = '{userName.Mail}'",connection);
            command = new SqlCommand($"SELECT IdFilm,Title,R.Id,R.RentDate,R.ReturnDate " +
                                     $"FROM Films F INNER JOIN Rentals R " +
                                     $"ON F.Id = R.IdFilm WHERE UserMail = '{userName.Mail}'", connection);
            rent = command.ExecuteReader();
            while (rent.Read())//crear lista con los ids de las peliculas que tiene alquiladas
            {
                //if (rent["ReturnDate"] != null) //si tiene fecha de vuelta ponemos que ha sido entregada
                if (rent["ReturnDate"].ToString() != string.Empty)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"Película {rent["IdFilm"]}-{rent["Title"]}");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\tSe devolvió el día {Convert.ToDateTime(rent["ReturnDate"]).ToString("dd/MM/yyyy")}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    haveFilm = true;
                    myRent.FilmId = Convert.ToInt32(rent["IdFilm"]);
                    myRent.Id = Convert.ToInt32(rent["Id"]);
                    myRentals.Add(myRent);

                    if (((DateTime.Today - Convert.ToDateTime(rent["RentDate"])).TotalDays) > 3 ) // FALLA ESTO
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Película {rent["IdFilm"]}-{rent["Title"]} \tSe alquiló el día {Convert.ToDateTime(rent["RentDate"]).ToString("dd/MM/yyyy")}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine($"Película {rent["IdFilm"]}-{rent["Title"]} \tSe alquiló el día {Convert.ToDateTime(rent["RentDate"]).ToString("dd/MM/yyyy")}. Le quedan {Math.Abs((DateTime.Today - Convert.ToDateTime(rent["RentDate"])).TotalDays-3)} días para devolverla.");
                        Console.ResetColor();

                    }
                }
                
            }
            connection.Close();
            bool isOk = false;
            int rentId = 0;
            if (haveFilm)
            {
                Console.WriteLine("¿Desea devolver alguna película?");//Lo dejo sin hacer por ahora
                try
                {
                    Console.WriteLine("\nIntroduce el id de la película que deseas devolver u otra tecla si deseas salir");//da pereza
                    int option = Int32.Parse(Console.ReadLine());
                    foreach (Rentals oneRent in myRentals)
                    {
                        if (oneRent.FilmId == option)
                        {
                            rentId = oneRent.Id;
                            isOk = true;
                        }
                    }
                    if (isOk)
                    {
                        connection.Open();  //ponemos no disponible
                        command = new SqlCommand($"UPDATE FILMS SET AVAILABLE = 'Y' WHERE ID = '{option}';" +
                                                 $"UPDATE RENTALS SET ReturnDate = GETDATE() WHERE ID = '{rentId}'", connection);
                        command.ExecuteNonQuery();
                        connection.Close();
                        Console.WriteLine("Ha devuelto la película.");
                    }
                    else
                    {
                        Console.WriteLine("No tiene alquilada esa película");
                    }

                }
                catch (Exception ex)
                {

                }
                //foreach (int id in filmList)
                //{
                //connection.Open();
                //command = new SqlCommand($"SELECT * FROM FILMS WHERE ID = {id}");
                //film = command.ExecuteReader();
                //while (film.Read())//crear lista con los ids de las peliculas que tiene alquiladas
                //{
                //}
                //connection.Close();
                //}
            }
            else
            {
                Console.WriteLine("No tiene ninguna pelicula para devolver.");
            }
        }

        public static int UserAge(DateTime birthdate)
        {
            int age;
            age = DateTime.Now.Year - birthdate.Year;
            if (DateTime.Now.Month < birthdate.Month || (DateTime.Now.Month == birthdate.Month && DateTime.Now.Day < birthdate.Day))
            {
                age--;
            }
            return age;
        }
    }
    
}

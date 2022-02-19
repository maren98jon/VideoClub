using System;
namespace VideoClub
{
    public class Rentals
    {
        public int Id { get; set; }
        public int FilmId { get; set; }
        public string UserMail { get; set; }
        public DateTime RentDate { get; set; }
        public DateTime ReturnDate { get; set; }

        public Rentals()
        {
        }
    }
}

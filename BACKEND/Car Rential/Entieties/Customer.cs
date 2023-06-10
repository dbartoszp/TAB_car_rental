﻿namespace Car_Rential.Entieties
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Pesel { get; set; }
        public string HassedPassword { get; set; }
        public int ReservationCount { get; set; } = 0;

        public int CustromerAddressId { get; set; }
        public virtual CustomerAddress CustromerAddress { get; set; }

        public virtual List<Reservation> Reservations { get; set; }
    }
}

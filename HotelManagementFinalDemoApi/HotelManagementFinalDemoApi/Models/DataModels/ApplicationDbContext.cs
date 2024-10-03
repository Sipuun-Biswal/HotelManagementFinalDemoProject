using Microsoft.EntityFrameworkCore;
namespace HotelManagementFinalDemoApi.Models.DataModels
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Otp> Otps { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ResertPassword> ResertPasswords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Hotel Relationship
            modelBuilder.Entity<Hotel>()
             .HasOne(h => h.Country) 
            .WithMany() 
            .HasForeignKey(h => h.CountryId) 
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hotel>()
            .HasOne(h => h.State) 
            .WithMany() 
             .HasForeignKey(h => h.StateId) 
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hotel>()
            .HasOne(h => h.City) 
            .WithMany() 
            .HasForeignKey(h => h.CityId) 
            .OnDelete(DeleteBehavior.Restrict); 


            //Room Relationship
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
            //State
            modelBuilder.Entity<State>()
               .HasOne(s => s.Country)
               .WithMany(c => c.States)
               .HasForeignKey(s => s.CountryId)
               .OnDelete(DeleteBehavior.Cascade);
            //City
            modelBuilder.Entity<City>()
                .HasOne(c => c.State)
                .WithMany(s => s.Cities)
                .HasForeignKey(c => c.StateId)
                .OnDelete(DeleteBehavior.Cascade);

            //Bookings



            modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);



            ////Feedback

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Booking)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.BookingId)
                .OnDelete(DeleteBehavior.Cascade);


            //Booking
            modelBuilder.Entity<Booking>()
                    .HasOne(r => r.Room)
                    .WithMany(h => h.Bookings)
                    .HasForeignKey(r => r.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);


          

        }
    }
}

using Microsoft.EntityFrameworkCore;
using SocialApp.API.Models;

namespace SocialApp.API.Data
{
    public class DataContext : DbContext
    {

        //DbContext represents a session with a database
        //if there is a values table in db
        //we will use this to query database
        //this will return the result from db
        //we are inheriting/deriving from this class

        //instance,options,pass options to base constructor
        //constructor chaining
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        //give plural name of entity
        //Value -> values

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Photo> Photos { get; set; }

        public DbSet<Like> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }

        //we need to override OnModelCreating
        //primary key setting (LikerId,LikeeId)
        protected override void OnModelCreating(ModelBuilder builder){
            builder.Entity<Like>().HasKey(k => new {k.LikerId,k.LikeeId});

            builder.Entity<Like>()
            .HasOne(u => u.Likee)
            .WithMany(u => u.Likers)
            .HasForeignKey(u => u.LikeeId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
            .HasOne(u => u.Liker)
            .WithMany(u => u.Likees)
            .HasForeignKey(u => u.LikerId)
            .OnDelete(DeleteBehavior.Restrict);


             builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);//jab tak dono message delete nahi karte message rahega save

            builder.Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany(m => m.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);//jab tak dono message delete nahi karte message rahega save
            
        }




        /*

        define relationships in entity framework
Fluent API
add likes functionality

OneToMany
User -> Photos

Relational Model
Like = Entity

User sends -> Like -> receive -> Users



ManyToMany
User -> Users

-------------------------------
Fluent API
One To Many Relationships
No Way to define many to many relationships

so we handle it this way
User [HasOne] Like(r) [WithMany] Like(es)
User [HasOne] Like(e) [WithMany] Like(rs)


when we create migration.. see the onDelete: ReferentialAction.Restrict

when user likes or removes the like -> this means that user will not get deleted



        */


        /*
        if you update dotnet / core sdk / any sdk and ef tool does not run then globally install dotnet ef
        dotnet tool install --global dotnet-ef --version 3.0.0
You can invoke the tool using the following command: dotnet-ef
Tool 'dotnet-ef' (version '3.0.0') was successfully installed.

no change in command syntax
and we get

command = dotnet ef migrations add AddedLikeEntity [from cmd]

info: Microsoft.EntityFrameworkCore.Infrastructure[10403]
      Entity Framework Core 2.1.11-servicing-32099 initialized 'DataContext' using provider 'Microsoft.EntityFrameworkCore.Sqlite' with options: None
Done. To undo this action, use 'ef migrations remove'


after which run 
dotnet ef database update
and check dbbrowser to see if table was created properly
        */
        






    }
}
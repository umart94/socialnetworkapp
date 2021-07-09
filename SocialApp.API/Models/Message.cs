using System;

namespace SocialApp.API.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }

        public User Sender { get; set; }

        public int RecipientId { get; set; }

        public User Recipient { get; set; }

        public string Content { get; set; }

        public bool isRead { get; set; }

        public DateTime? DateRead { get; set; } //? means optional property because we want it to be null if message hasnt been read yet

        public DateTime MessageSent { get; set; }
        //delete message from 1 side - wont permanently delete
        //it will keep message on the other side because itna important message hai
        //jab dono ne delete mardia - to hum message ko delete mardenge takay kachra na bhara rahay

        public bool SenderDeleted { get; set; }

        public bool RecipientDeleted { get; set; }

        /*
        dotnet ef migrations add MessageEntityAdded
info: Microsoft.EntityFrameworkCore.Infrastructure[10403]
      Entity Framework Core 2.1.11-servicing-32099 initialized 'DataContext' using provider 'Microsoft.EntityFrameworkCore.Sqlite' with options: None
Done. To undo this action, use 'ef migrations remove'

dotnet ef database update
*/






    }
}
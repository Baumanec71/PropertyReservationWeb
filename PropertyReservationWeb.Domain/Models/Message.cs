using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyReservationWeb.Domain.Models
{
    public class Message
    {
        private Message() { }
        public Message(long id,bool deletionStatus,bool readingStatus,string messageText,string authorName, DateTime dateCreate, long idConversationRoom)
        {
            Id = id;
            DeletionStatus = deletionStatus;
            ReadingStatus = readingStatus;
            MessageText = messageText;
            AuthorName = authorName;
            DateCreate = dateCreate;
            IdConversationRoom = idConversationRoom;
        }
        public long Id { get; }


        public bool DeletionStatus { get;  } 
        public bool ReadingStatus { get;  } 
        public string MessageText { get; } = string.Empty;
        public string AuthorName { get; } = string.Empty;
        public DateTime DateCreate { get; }
        public long IdConversationRoom { get; }
        public ConversationRoom ConversationRoom { get; } = null!;
    }
}

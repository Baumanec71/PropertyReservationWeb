using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyReservationWeb.Domain.Models
{
    public class ConversationRoom
    {
        private ConversationRoom() { }
        public ConversationRoom(long id,DateTime dateCreate,bool deletionStatus, long idOneUser, long idTwoUser) 
        { 
            Id= id;
            DateCreate = dateCreate;
            DeletionStatus = deletionStatus;
            IdOneUser= idOneUser;
            IdTwoUser= idTwoUser;
        }
        public long Id { get; set; }


        public DateTime DateCreate { get; set; }

        public bool DeletionStatus { get; set; } //

        public long IdOneUser { get; set; }

        public long IdTwoUser { get; set; }

        public User OneUser { get; set; } = null!;
        public User TwoUser { get; set; } = null!;

        public List<Message> Message { get; set; } = [];
    }
}

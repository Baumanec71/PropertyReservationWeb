using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyReservationWeb.Domain.Models
{
    public class Review
    {
        public Review() { }
        public Review(
            long id,
            int theQualityOfTheTransaction,
            string comment,
            bool statusDel,
            bool isTheLandlord, 
            bool recipientsViewingStatus,
            bool authorsViewingStatus,
            DateTime dateOfCreation,
            long idNeedRentalRequest,
            RentalRequest rentalRequest) 
        { 
            Id = id;
            TheQualityOfTheTransaction = theQualityOfTheTransaction;
            Comment = comment;
            StatusDel = statusDel;
            IsTheLandlord = isTheLandlord;
            RecipientsViewingStatus = recipientsViewingStatus;
            AuthorsViewingStatus = authorsViewingStatus;
            DateOfCreation = dateOfCreation;
            IdNeedRentalRequest = idNeedRentalRequest;
            RentalRequest = rentalRequest;
        }
        public long Id { get; set; }
        public int TheQualityOfTheTransaction { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool StatusDel { get; set; }
        public bool IsTheLandlord { get; set; }
        public bool RecipientsViewingStatus { get; set; }
        public bool AuthorsViewingStatus { get; set; }
        public DateTime DateOfCreation { get; set; }
        public long IdNeedRentalRequest { get; set; }
        public RentalRequest RentalRequest { get; set; }
    }
}

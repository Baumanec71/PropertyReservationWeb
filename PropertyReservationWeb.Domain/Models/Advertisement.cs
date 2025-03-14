using NetTopologySuite.Geometries;
using PropertyReservationWeb.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyReservationWeb.Domain.Models
{
    public class Advertisement
    {
        public Advertisement() { }
        public Advertisement(
            ObjectType objectType,
            Point adressCoordinates,
            string adressName,
            string apartmentNumber,
            bool confirmationStatus,
            bool deletionStatus,
            string description,
            uint totalArea,
            decimal rentalPrice,
            decimal fixedPrepaymentAmount,
            uint numberOfRooms,
            uint numberOfBeds,
            uint numberOfBathrooms,
            DateTime dateCreate,
            double rating,
            uint numberOfPromotionPoints,
            long idAuthor
            )
        {
            ObjectType = objectType;
            AdressCoordinates = adressCoordinates;
            AdressName = adressName;
            ApartmentNumber = apartmentNumber;
            ConfirmationStatus = confirmationStatus;
            DeletionStatus = deletionStatus;
            Description = description;
            TotalArea = totalArea;
            RentalPrice = rentalPrice;
            FixedPrepaymentAmount = fixedPrepaymentAmount;
            NumberOfRooms = numberOfRooms;
            NumberOfBeds = numberOfBeds;
            NumberOfBathrooms = numberOfBathrooms;
            DateCreate = dateCreate;
            Rating = rating;
            NumberOfPromotionPoints = numberOfPromotionPoints;
            IdAuthor = idAuthor;
        }
        public long Id { get; set; }
        public ObjectType ObjectType { get; set; }

        [Column(TypeName = "geography")]
        public Point? AdressCoordinates { get; set; }
        public string AdressName { get; set; } = string.Empty;
        public string ApartmentNumber { get; set; } = string.Empty;
        public bool ConfirmationStatus { get; set; }
        public bool DeletionStatus { get; set; }
        public string Description { get; set; } = string.Empty;
        public uint TotalArea { get; set; }
        public decimal RentalPrice { get; set; }
        public decimal FixedPrepaymentAmount { get; set; }
        public uint NumberOfRooms { get; set; }
        public uint NumberOfBeds { get; set; }
        public uint NumberOfBathrooms { get; set; }
        public DateTime DateCreate { get; set; }
        public double Rating { get; set; }
        public uint NumberOfPromotionPoints { get; set; }
        public long IdAuthor { get; set; }
        public User User { get; set; } = null!;
        public List<Photo> Photos { get; set; } = new();
        public List<AdvertisementAmenity> AdvertisementAmenities { get; set; } = new();
        public List<RentalRequest> RentalRequests { get; set; } = new();
    }
}

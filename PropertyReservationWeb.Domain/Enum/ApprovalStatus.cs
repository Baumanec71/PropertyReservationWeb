using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyReservationWeb.Domain.Enum
{
    public enum ApprovalStatus
    {
        [Display(Name = "На рассмотрении")]
        UnderСonsideration = 0,
        [Display(Name = "Одобрен")]
        Approved = 1,
        [Display(Name = "Отклонен")]
        Rejected = 2,
        [Display(Name = "Завершен успешно")]
        EndApproved = 3,
    }
}

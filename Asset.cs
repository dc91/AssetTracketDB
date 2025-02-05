using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetTracketDB
{
    internal abstract class Asset
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string ModelName { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public DateOnly PurchaseDate { get; set; }

        public int OfficeId { get; set; }   // Foreign key to Office

        // Navigation property - EF Core uses this to handle the relationship
        public Office Office { get; set; }

        [NotMapped]
        public string EndOfLifeStatus
        {
            get
            {
                DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
                DateOnly endOfLifeDate = PurchaseDate.AddMonths(36);

                if (currentDate > endOfLifeDate)
                    return "Past End of Life";
                else if (currentDate > endOfLifeDate.AddMonths(-3))
                    return "3 Months Left";
                else if (currentDate > endOfLifeDate.AddMonths(-6))
                    return "6 Months Left";
                else
                    return "Normal";
            }

        }
        public abstract string Type { get; }

    }
}

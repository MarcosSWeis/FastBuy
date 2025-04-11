using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FastBuy.Auth.Api.Entity
{
    public class ApplicationUser :IdentityUser
    {
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]

        public string LastName { get; set; }
        public DocumentTypeEnum DocumentType { get; set; }
        [MaxLength(100)]

        public string DocumentNumber { get; set; }

    }

    public enum DocumentTypeEnum :short
    {
        DNI = 1,
        LIBRETAENRROLAMIENTO,
        CANET,
        PASAPORTE
    }
}

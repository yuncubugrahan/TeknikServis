using System.ComponentModel.DataAnnotations;

namespace TeknikServis.Models
{
    public class RequestCategory
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public List<ServiceRequest> ServiceRequests { get; set; } = new();
    }
}

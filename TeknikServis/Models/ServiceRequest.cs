using System.ComponentModel.DataAnnotations;

namespace TeknikServis.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Müşteri adı zorunludur.")]
        [StringLength(80, ErrorMessage = "En fazla 80 karakter.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cihaz adı zorunludur.")]
        [StringLength(100, ErrorMessage = "En fazla 100 karakter.")]
        public string DeviceName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Arıza açıklaması zorunludur.")]
        [StringLength(500, ErrorMessage = "En fazla 500 karakter.")]
        public string ProblemDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Talep tarihi zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Durum zorunludur.")]
        public string Status { get; set; } = "Beklemede";

        [Range(1, int.MaxValue, ErrorMessage = "Lütfen kategori seçiniz.")]
        public int RequestCategoryId { get; set; }

        public RequestCategory? RequestCategory { get; set; }
    }
}
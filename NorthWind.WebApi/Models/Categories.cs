#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthWind.WebApi.Models;

public class Categories
{
    // Primary key
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensures that this field will use SQL Server's IDENTITY feature
    public int CategoryID { get; set; }

    // Columns
    [Required]
    [StringLength(15)] // Matches nvarchar(15)
    public string CategoryName { get; set; }

    [Column(TypeName = "ntext")] // Maps to SQL ntext type
    public string Description { get; set; }

    [Column(TypeName = "image")] // Maps to SQL image type
    public byte[] Picture { get; set; }
}

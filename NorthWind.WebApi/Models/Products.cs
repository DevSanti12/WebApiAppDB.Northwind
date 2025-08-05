#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthWind.WebApi.Models;

public class Products
{
    // Primary key
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensures that this field will use SQL Server's IDENTITY feature
    public int ProductID { get; set; }

    // Columns
    [Required]
    [StringLength(40)] // Matches nvarchar(40)
    public string ProductName { get; set; }

    public int? SupplierID { get; set; } // Nullable int
    public int? CategoryID { get; set; } // Nullable int

    [StringLength(20)] // Matches nvarchar(20)
    public string QuantityPerUnit { get; set; }

    [Column(TypeName = "money")] // Maps to SQL money type
    public decimal? UnitPrice { get; set; } // Nullable decimal

    public short? UnitsInStock { get; set; } // Nullable short (smallint)
    public short? UnitsOnOrder { get; set; } // Nullable short (smallint)
    public short? ReorderLevel { get; set; } // Nullable short (smallint)

    [Required] // Not nullable
    public bool Discontinued { get; set; }
}

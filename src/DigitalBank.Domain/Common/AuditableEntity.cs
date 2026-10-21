using System.ComponentModel.DataAnnotations;
using DigitalBank.Domain.Constants;

namespace DigitalBank.Domain.Common;

// Clase base para auditoria de entidadaes
// Se usan Data Annotations para validacion y definicionn de tipos en persistencia
public abstract class AuditableEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(100)]
    public string CreatedBy { get; set; } = BankConstants.DefaultSystemUser;

    [Required]
    [MaxLength(50)]
    public string CreatedIp { get; set; } = BankConstants.DefaultLocalIp;

    public DateTime? UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    [MaxLength(50)]
    public string? UpdatedIp { get; set; }

    public void SetCreationAudit(string createdBy, string createdIp)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = string.IsNullOrWhiteSpace(createdBy) ? BankConstants.DefaultSystemUser : createdBy.Trim();
        CreatedIp = string.IsNullOrWhiteSpace(createdIp) ? BankConstants.DefaultLocalIp : createdIp.Trim();
    }

    public void SetUpdateAudit(string updatedBy, string updatedIp)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = string.IsNullOrWhiteSpace(updatedBy) ? BankConstants.DefaultSystemUser : updatedBy.Trim();
        UpdatedIp = string.IsNullOrWhiteSpace(updatedIp) ? BankConstants.DefaultLocalIp : updatedIp.Trim();
    }
}

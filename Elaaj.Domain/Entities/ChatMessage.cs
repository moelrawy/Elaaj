using System;

namespace Elaaj.Domain.Entities;

public class ChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // مربوط بالروشتة عشان الشات يفتح على الروشتة دي تحديداً
    public Guid PrescriptionId { get; set; }

    public string SenderId { get; set; } = string.Empty; // اللي بعت الرسالة
    public string ReceiverId { get; set; } = string.Empty; // اللي هيستقبل الرسالة

    public string Content { get; set; } = string.Empty; // نص الرسالة

    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties (Optional)
    public virtual Prescription Prescription { get; set; } = null!;
}
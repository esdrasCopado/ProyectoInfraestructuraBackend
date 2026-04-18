using System;
using System.Collections.Generic;

namespace SolicitudServidores.Back.Models
{
    public class ServerRequest
    {
        public long Id { get; set; }
        public string? ProjectName { get; set; }
        public string? RequestedBy { get; set; }
        public string? Description { get; set; }
        public string? Architecture { get; set; } // Monolítica, Microservicios, etc
        public string? RequiredServices { get; set; } // BD, App, Web, etc
        public string? TargetDate { get; set; }
        public string? Status { get; set; } = "Pendiente"; // Pendiente, En Revisión, Aprobada, Rechazada
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public List<ServerEntry>? Servers { get; set; }
    }

    public class ServerEntry
    {
        public long Id { get; set; }
        public long ServerRequestId { get; set; }
        public ServerRequest? ServerRequest { get; set; }
        public string? Hostname { get; set; }
        public string? Ip { get; set; }
        public string? Role { get; set; }
        public string? Os { get; set; }
        public string? Cpu { get; set; }
        public string? Ram { get; set; }
        public string? Disk { get; set; }
        public string? Purpose { get; set; }
    }
}

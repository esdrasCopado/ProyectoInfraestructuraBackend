namespace SolicitudServidores.DTOs
{
    public class AvanzarEtapaRequest
    {
        // en_proceso | completado | rechazado
        public string Status { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public long? CompletadoBy { get; set; }
    }
}

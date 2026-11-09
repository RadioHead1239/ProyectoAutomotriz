using System.Globalization;

namespace SistemaAutomotriz.Helpers
{
    public static class Fechas
    {
        public static readonly CultureInfo Cultura = new("es-PE");

        public static readonly TimeZoneInfo ZonaPeru = ObtenerZona();

        private static TimeZoneInfo ObtenerZona()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("America/Lima"); }
            catch (TimeZoneNotFoundException) { return TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"); }
        }

        public static DateTime AhoraLocal() => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ZonaPeru);

        public static DateTime ALocal(DateTime utc) =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), ZonaPeru);

        public static DateTime InicioDiaUtc(DateTime fechaLocal) =>
            TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(fechaLocal.Date, DateTimeKind.Unspecified), ZonaPeru);

        public static string Capitalizar(string texto) =>
            string.IsNullOrEmpty(texto) ? texto : char.ToUpper(texto[0], Cultura) + texto[1..];
    }
}

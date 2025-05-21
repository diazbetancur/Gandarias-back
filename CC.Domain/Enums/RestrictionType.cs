namespace CC.Domain.Enums;

public enum RestrictionType
{
    NotWorking,             // No trabaja ese día
    FullyAvailable,         // Disponible todo el día
    AvailableFrom,          // Disponible desde una hora
    AvailableUntil,         // Disponible hasta una hora
    AvailableBetween,       // Disponible en uno o dos bloques
    NotAvailableBetween     // No disponible en uno o dos bloques
}
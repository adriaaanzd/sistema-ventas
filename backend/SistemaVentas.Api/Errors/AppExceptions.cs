namespace SistemaVentas.Api.Errors;

// Excepciones de negocio que lanzan los servicios; GlobalExceptionHandler las traduce a HTTP.

/// <summary>El recurso pedido no existe (404).</summary>
public class NotFoundException(string message) : Exception(message);

/// <summary>La operación choca con el estado actual de los datos, p. ej. borrar un cliente con ventas (409).</summary>
public class ConflictException(string message) : Exception(message);

/// <summary>Los datos son válidos en forma pero rompen una regla de negocio, p. ej. stock insuficiente (400).</summary>
public class BusinessRuleException(string message) : Exception(message);

// Acceso a la API REST. Todas las páginas llaman al backend a través de api().

// Perfil "https" de backend/SistemaVentas.Api/Properties/launchSettings.json.
export const API_URL = 'https://localhost:7080/api';

// Error con el código HTTP y, si es una validación (400), los mensajes por campo.
export class ApiError extends Error {
  constructor(status, problema) {
    super(mensajeDe(problema));
    this.name = 'ApiError';
    this.status = status;
    this.errores = problema?.errors ?? {};
  }
}

// ProblemDetails: usa "errors" (validación), luego "detail" (negocio) y por último "title".
function mensajeDe(problema) {
  if (problema?.errors) return [...new Set(Object.values(problema.errors).flat())].join(' ');
  return problema?.detail ?? problema?.title ?? 'Ocurrió un error inesperado.';
}

/**
 * Llama a la API y devuelve el JSON de la respuesta (null si es 204).
 * Lanza ApiError si la respuesta no es 2xx o si no hay conexión.
 * @param {string} ruta Por ejemplo '/clientes?buscar=ana'.
 * @param {{ method?: string, body?: unknown }} [opciones]
 */
export async function api(ruta, { method = 'GET', body } = {}) {
  let respuesta;
  try {
    respuesta = await fetch(API_URL + ruta, {
      method,
      headers: body === undefined ? undefined : { 'Content-Type': 'application/json' },
      body: body === undefined ? undefined : JSON.stringify(body),
    });
  } catch {
    // fetch solo se rechaza por red, CORS o certificado; un 4xx/5xx llega como respuesta normal.
    throw new ApiError(0, { detail: `No se pudo conectar con la API (${API_URL}). Verifique que esté en ejecución.` });
  }

  const datos = respuesta.status === 204 ? null : await respuesta.json().catch(() => null);
  if (!respuesta.ok) throw new ApiError(respuesta.status, datos);
  return datos;
}

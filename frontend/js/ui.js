// Utilidades de interfaz compartidas: barra lateral, toasts, confirmación y formatos.
// Depende de window.bootstrap (bootstrap.bundle.min.js se carga antes que los módulos).

export const LOCALE = 'es-PE';
export const MONEDA = 'PEN';

const PAGINAS = [
  { archivo: 'index.html', texto: 'Dashboard', icono: 'bi-speedometer2' },
  { archivo: 'clientes.html', texto: 'Clientes', icono: 'bi-people' },
  { archivo: 'productos.html', texto: 'Productos', icono: 'bi-box-seam' },
  { archivo: 'ventas.html', texto: 'Ventas', icono: 'bi-cart3' },
];

/** Escapa texto antes de insertarlo con innerHTML, para que un nombre como "<script>" no se ejecute. */
export function escaparHtml(texto) {
  const reemplazos = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' };
  return String(texto ?? '').replace(/[&<>"']/g, (c) => reemplazos[c]);
}

function montarSidebar() {
  const sidebar = document.getElementById('sidebar');
  if (!sidebar) return;

  const actual = location.pathname.split('/').pop() || 'index.html';
  const enlaces = PAGINAS.map((p) => {
    const activa = p.archivo === actual;
    return `
      <a class="nav-link${activa ? ' active' : ''}" href="${p.archivo}"${activa ? ' aria-current="page"' : ''}>
        <i class="bi ${p.icono}" aria-hidden="true"></i>${p.texto}
      </a>`;
  }).join('');

  const marca = `
    <span class="marca-icono"><i class="bi bi-graph-up-arrow" aria-hidden="true"></i></span>
    <span>Sistema de Ventas</span>`;

  sidebar.innerHTML = `
    <div class="offcanvas-header border-bottom">
      <a class="marca" href="index.html">${marca}</a>
      <button type="button" class="btn-close" data-bs-dismiss="offcanvas" data-bs-target="#sidebar" aria-label="Cerrar menú"></button>
    </div>
    <div class="offcanvas-body flex-column p-3">
      <a class="marca d-none d-lg-flex px-2 py-1" href="index.html">${marca}</a>
      <p class="nav-titulo">Menú</p>
      <nav class="nav flex-column gap-1" aria-label="Secciones">${enlaces}</nav>
    </div>`;
}

const ICONOS_TOAST = {
  success: 'bi-check-circle-fill',
  danger: 'bi-x-circle-fill',
  warning: 'bi-exclamation-triangle-fill',
};

/**
 * Muestra un toast de Bootstrap en la esquina superior derecha.
 * @param {string} mensaje
 * @param {'success'|'danger'|'warning'} [tipo]
 */
export function mostrarToast(mensaje, tipo = 'success') {
  let contenedor = document.getElementById('toasts');
  if (!contenedor) {
    contenedor = document.createElement('div');
    contenedor.id = 'toasts';
    contenedor.className = 'toast-container position-fixed top-0 end-0 p-3';
    document.body.append(contenedor);
  }

  const esError = tipo === 'danger';
  const toast = document.createElement('div');
  toast.className = `toast align-items-center text-bg-${tipo} border-0`;
  toast.setAttribute('role', esError ? 'alert' : 'status');
  toast.setAttribute('aria-live', esError ? 'assertive' : 'polite');
  toast.setAttribute('aria-atomic', 'true');
  toast.innerHTML = `
    <div class="d-flex">
      <div class="toast-body"><i class="bi ${ICONOS_TOAST[tipo]} me-2" aria-hidden="true"></i>${escaparHtml(mensaje)}</div>
      <button type="button" class="btn-close${tipo === 'warning' ? '' : ' btn-close-white'} me-2 m-auto" data-bs-dismiss="toast" aria-label="Cerrar"></button>
    </div>`;

  contenedor.append(toast);
  toast.addEventListener('hidden.bs.toast', () => toast.remove());
  bootstrap.Toast.getOrCreateInstance(toast, { delay: esError ? 7000 : 4000 }).show();
}

/** Muestra un toast rojo con el mensaje de un ApiError (o de cualquier Error). */
export function mostrarError(error) {
  mostrarToast(error?.message || 'Ocurrió un error inesperado.', 'danger');
}

/**
 * Pide confirmación con un modal de Bootstrap. Resuelve true solo si se pulsa el botón de confirmar.
 * @returns {Promise<boolean>}
 */
export function confirmar(mensaje, { titulo = 'Confirmar acción', textoBoton = 'Eliminar', claseBoton = 'btn-danger' } = {}) {
  return new Promise((resolve) => {
    const elemento = document.createElement('div');
    elemento.className = 'modal fade';
    elemento.tabIndex = -1;
    elemento.setAttribute('aria-labelledby', 'confirmar-titulo');
    elemento.innerHTML = `
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h2 class="modal-title h5" id="confirmar-titulo">${escaparHtml(titulo)}</h2>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
          </div>
          <div class="modal-body">${escaparHtml(mensaje)}</div>
          <div class="modal-footer">
            <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cancelar</button>
            <button type="button" class="btn ${claseBoton}" data-accion="confirmar">${escaparHtml(textoBoton)}</button>
          </div>
        </div>
      </div>`;

    let aceptado = false;
    const modal = new bootstrap.Modal(elemento);
    elemento.querySelector('[data-accion="confirmar"]').addEventListener('click', () => {
      aceptado = true;
      modal.hide();
    });
    elemento.addEventListener('hidden.bs.modal', () => {
      elemento.remove();
      resolve(aceptado);
    });

    document.body.append(elemento);
    modal.show();
  });
}

// ===== Formularios =====
// Las reglas viven en el HTML (required, pattern, min, maxlength...) y replican las Data Annotations del backend.
// Cada control va junto a su .invalid-feedback dentro del mismo contenedor.

function feedbackDe(control) {
  return control.parentElement?.querySelector('.invalid-feedback');
}

function restaurarMensaje(control) {
  const feedback = feedbackDe(control);
  if (feedback?.dataset.original) feedback.textContent = feedback.dataset.original;
}

/** Al escribir en un campo se borra el error que haya marcado el servidor en ese campo. */
export function prepararFormulario(form) {
  form.addEventListener('input', (e) => {
    e.target.setCustomValidity?.('');
    restaurarMensaje(e.target);
  });
}

/** Vacía el formulario y quita los estilos de validación. */
export function reiniciarFormulario(form) {
  form.reset();
  form.classList.remove('was-validated');
  for (const control of form.elements) {
    control.setCustomValidity?.('');
    restaurarMensaje(control);
  }
}

/** Recorta espacios, valida con las reglas HTML5 y muestra los mensajes de Bootstrap. */
export function validarFormulario(form) {
  for (const control of form.elements) {
    if (control.type === 'text' || control.type === 'email' || control.type === 'tel' || control.tagName === 'TEXTAREA') {
      control.value = control.value.trim();
    }
  }
  form.classList.add('was-validated');
  return form.checkValidity();
}

/**
 * Muestra bajo cada campo los errores 400 de la API ({ Correo: ['...'] }).
 * Devuelve false si ningún error corresponde a un campo del formulario.
 */
export function mostrarErroresDeCampo(form, errores) {
  let alguno = false;
  for (const [campo, mensajes] of Object.entries(errores ?? {})) {
    const control = form.elements.namedItem(campo.charAt(0).toLowerCase() + campo.slice(1));
    if (!control) continue;
    control.setCustomValidity(mensajes[0]);
    const feedback = feedbackDe(control);
    if (feedback) {
      feedback.dataset.original ??= feedback.textContent;
      feedback.textContent = mensajes[0];
    }
    alguno = true;
  }
  form.classList.add('was-validated');
  return alguno;
}

// ===== Formatos =====
const formatoMoneda = new Intl.NumberFormat(LOCALE, { style: 'currency', currency: MONEDA });

/** 1234.5 → "S/ 1,234.50" */
export const formatearMoneda = (valor) => formatoMoneda.format(valor);

/** "2026-10-15T14:50:00" → "15 oct. 2026" */
export const formatearFecha = (iso) =>
  new Date(iso).toLocaleDateString(LOCALE, { day: '2-digit', month: 'short', year: 'numeric' });

/** "2026-10-15T14:50:00" → "15 oct. 2026, 2:50 p. m." */
export const formatearFechaHora = (iso) =>
  new Date(iso).toLocaleString(LOCALE, { dateStyle: 'medium', timeStyle: 'short' });

// Los módulos se ejecutan después de que el HTML está listo, así que el sidebar se monta al importar ui.js.
montarSidebar();

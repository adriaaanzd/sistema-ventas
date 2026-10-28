import { api } from './api.js';
import { confirmar, escaparHtml, formatearFechaHora, formatearMoneda, mostrarError, mostrarToast } from './ui.js';

const form = document.getElementById('form-venta');
const selectCliente = document.getElementById('venta-cliente');
const selectProducto = document.getElementById('venta-producto');
const inputCantidad = document.getElementById('venta-cantidad');
const feedbackCantidad = document.getElementById('feedback-cantidad');
const tablaCarrito = document.getElementById('tabla-carrito');
const resumenUnidades = document.getElementById('resumen-unidades');
const totalVenta = document.getElementById('venta-total');
const botonRegistrar = document.getElementById('btn-registrar');
const tablaHistorial = document.getElementById('tabla-historial');
const contadorHistorial = document.getElementById('contador-historial');
const modalDetalle = new bootstrap.Modal(document.getElementById('modal-detalle'));
const tituloDetalle = document.getElementById('modal-detalle-titulo');
const cuerpoDetalle = document.getElementById('detalle-cuerpo');

/** @type {Map<number, object>} productos por Id, con su stock actual */
let productos = new Map();
/** @type {Map<number, number>} productoId → cantidad en el carrito */
const carrito = new Map();

// Los montos se suman en céntimos (enteros): 0.1 + 0.2 en coma flotante no da 0.3.
const aCentimos = (precio) => Math.round(precio * 100);
const subtotalCentimos = (productoId, cantidad) => aCentimos(productos.get(productoId).precio) * cantidad;

const disponible = (p) => p.stock - (carrito.get(p.id) ?? 0);

// ===== Carga de datos =====

async function cargarClientes() {
  const clientes = await api('/clientes');
  selectCliente.innerHTML = '<option value="">Seleccione un cliente…</option>'
    + clientes.map((c) => `<option value="${c.id}">${escaparHtml(c.nombre)} — ${escaparHtml(c.documento)}</option>`).join('');
}

async function cargarProductos() {
  const lista = await api('/productos');
  productos = new Map(lista.map((p) => [p.id, p]));
  // Si un producto del carrito fue eliminado mientras tanto, se quita del carrito.
  for (const id of carrito.keys()) if (!productos.has(id)) carrito.delete(id);
  pintarSelectorProductos();
  pintarCarrito();
}

async function cargarHistorial() {
  try {
    const ventas = await api('/ventas');
    contadorHistorial.textContent = `${ventas.length} ${ventas.length === 1 ? 'venta' : 'ventas'}`;
    if (ventas.length === 0) {
      tablaHistorial.innerHTML = '<tr><td colspan="4" class="text-secondary">Aún no hay ventas registradas.</td></tr>';
      return;
    }
    tablaHistorial.innerHTML = ventas.map((v) => `
      <tr>
        <td>
          <div class="fw-medium">#${v.id}</div>
          <div class="small text-secondary">${formatearFechaHora(v.fecha)}</div>
        </td>
        <td>
          <div>${escaparHtml(v.clienteNombre)}</div>
          <div class="small text-secondary">${v.cantidadProductos} ${v.cantidadProductos === 1 ? 'unidad' : 'unidades'}</div>
        </td>
        <td class="numero fw-medium">${formatearMoneda(v.total)}</td>
        <td class="text-end">
          <button type="button" class="btn btn-sm btn-outline-primary" data-id="${v.id}" aria-label="Ver detalle de la venta ${v.id}">
            <i class="bi bi-eye" aria-hidden="true"></i>
          </button>
        </td>
      </tr>`).join('');
  } catch (error) {
    mostrarError(error);
    tablaHistorial.innerHTML = '<tr><td colspan="4" class="text-danger">No se pudo cargar el historial.</td></tr>';
  }
}

// ===== Carrito =====

function pintarSelectorProductos() {
  const seleccionado = selectProducto.value;
  selectProducto.innerHTML = '<option value="">Seleccione un producto…</option>'
    + [...productos.values()].map((p) => {
      const libre = disponible(p);
      const estado = p.stock === 0 ? 'agotado' : libre <= 0 ? 'todo en el carrito' : `disponible: ${libre}`;
      return `<option value="${p.id}"${libre <= 0 ? ' disabled' : ''}>${escaparHtml(p.nombre)} — ${formatearMoneda(p.precio)} (${estado})</option>`;
    }).join('');
  selectProducto.value = seleccionado;
  if (selectProducto.selectedOptions[0]?.disabled) selectProducto.value = '';
}

function pintarCarrito() {
  if (carrito.size === 0) {
    tablaCarrito.innerHTML = '<tr><td colspan="5" class="text-secondary text-center py-4">Agregue productos para armar la venta.</td></tr>';
    actualizarTotales();
    return;
  }

  tablaCarrito.innerHTML = [...carrito].map(([id, cantidad]) => {
    const p = productos.get(id);
    const nombre = escaparHtml(p.nombre);
    return `
      <tr data-id="${id}">
        <td>
          <div class="fw-medium">${nombre}</div>
          <div class="small text-secondary">${escaparHtml(p.codigo)}<span class="d-sm-none"> · ${formatearMoneda(p.precio)}</span></div>
          <button type="button" class="btn btn-link btn-sm text-danger p-0 d-sm-none" data-accion="quitar" aria-label="Quitar ${nombre}">Quitar</button>
        </td>
        <td class="numero d-none d-sm-table-cell">${formatearMoneda(p.precio)}</td>
        <td class="numero">
          <input type="number" class="form-control form-control-sm input-cantidad${cantidad > p.stock ? ' is-invalid' : ''}" min="1" max="${p.stock}" step="1"
                 value="${cantidad}" inputmode="numeric" aria-label="Cantidad de ${nombre}">
          <div class="invalid-feedback">Entre 1 y ${p.stock}.</div>
        </td>
        <td class="numero subtotal">${formatearMoneda(subtotalCentimos(id, cantidad) / 100)}</td>
        <td class="text-end d-none d-sm-table-cell">
          <button type="button" class="btn btn-sm btn-outline-danger" data-accion="quitar" aria-label="Quitar ${nombre}">
            <i class="bi bi-x-lg" aria-hidden="true"></i>
          </button>
        </td>
      </tr>`;
  }).join('');
  actualizarTotales();
}

function actualizarTotales() {
  let centimos = 0;
  let unidades = 0;
  for (const [id, cantidad] of carrito) {
    centimos += subtotalCentimos(id, cantidad);
    unidades += cantidad;
  }
  totalVenta.textContent = formatearMoneda(centimos / 100);
  resumenUnidades.textContent = `${unidades} ${unidades === 1 ? 'unidad' : 'unidades'}`;
  botonRegistrar.disabled = carrito.size === 0 || tablaCarrito.querySelector('.is-invalid') !== null;
}

function agregarProducto() {
  const producto = productos.get(Number(selectProducto.value));
  const cantidad = Number(inputCantidad.value);
  const libre = producto ? disponible(producto) : 0;
  const cantidadValida = Number.isInteger(cantidad) && cantidad >= 1 && cantidad <= libre;

  selectProducto.classList.toggle('is-invalid', !producto);
  inputCantidad.classList.toggle('is-invalid', Boolean(producto) && !cantidadValida);
  feedbackCantidad.textContent = `Entre 1 y ${libre}.`;
  if (!producto || !cantidadValida) return;

  carrito.set(producto.id, (carrito.get(producto.id) ?? 0) + cantidad);
  selectProducto.value = '';
  inputCantidad.value = 1;
  pintarSelectorProductos();
  pintarCarrito();
  selectProducto.focus();
}

// Cambiar la cantidad en la tabla recalcula sin volver a dibujarla, para no perder el foco del campo.
function cambiarCantidad(input) {
  const fila = input.closest('tr');
  const id = Number(fila.dataset.id);
  const cantidad = Number(input.value);
  const valida = Number.isInteger(cantidad) && cantidad >= 1 && cantidad <= productos.get(id).stock;

  input.classList.toggle('is-invalid', !valida);
  if (valida) {
    carrito.set(id, cantidad);
    fila.querySelector('.subtotal').textContent = formatearMoneda(subtotalCentimos(id, cantidad) / 100);
    pintarSelectorProductos();
  }
  actualizarTotales();
}

function quitarProducto(id) {
  carrito.delete(id);
  pintarSelectorProductos();
  pintarCarrito();
}

async function vaciarCarrito() {
  if (carrito.size === 0) return;
  const ok = await confirmar('¿Quitar todos los productos de la venta?', { titulo: 'Vaciar venta', textoBoton: 'Vaciar' });
  if (!ok) return;
  carrito.clear();
  pintarSelectorProductos();
  pintarCarrito();
}

async function registrarVenta(evento) {
  evento.preventDefault();
  selectCliente.classList.toggle('is-invalid', !selectCliente.value);
  if (!selectCliente.value || carrito.size === 0 || tablaCarrito.querySelector('.is-invalid')) return;

  // Solo se envían cliente, productos y cantidades: precios y total los calcula el backend.
  const venta = {
    clienteId: Number(selectCliente.value),
    lineas: [...carrito].map(([productoId, cantidad]) => ({ productoId, cantidad })),
  };

  botonRegistrar.disabled = true;
  try {
    const creada = await api('/ventas', { method: 'POST', body: venta });
    mostrarToast(`Venta #${creada.id} registrada por ${formatearMoneda(creada.total)}.`);
    carrito.clear();
    selectCliente.value = '';
    await Promise.all([cargarProductos(), cargarHistorial()]);
  } catch (error) {
    mostrarError(error);
    // El stock pudo cambiar (otra venta); se recarga para mostrar lo disponible ahora.
    await cargarProductos().catch(() => {});
  } finally {
    actualizarTotales();
  }
}

// ===== Detalle =====

async function verDetalle(id) {
  tituloDetalle.textContent = `Venta #${id}`;
  cuerpoDetalle.innerHTML = '<p class="text-secondary mb-0">Cargando…</p>';
  modalDetalle.show();
  try {
    const v = await api(`/ventas/${id}`);
    cuerpoDetalle.innerHTML = `
      <dl class="row mb-3">
        <dt class="col-sm-3">Fecha</dt><dd class="col-sm-9">${formatearFechaHora(v.fecha)}</dd>
        <dt class="col-sm-3">Cliente</dt><dd class="col-sm-9">${escaparHtml(v.clienteNombre)}</dd>
        <dt class="col-sm-3">Documento</dt><dd class="col-sm-9">${escaparHtml(v.clienteDocumento)}</dd>
      </dl>
      <div class="table-responsive border rounded">
        <table class="table tabla mb-0">
          <thead>
            <tr>
              <th scope="col">Producto</th>
              <th scope="col" class="numero d-none d-sm-table-cell">Precio unit.</th>
              <th scope="col" class="numero">Cant.</th>
              <th scope="col" class="numero">Subtotal</th>
            </tr>
          </thead>
          <tbody>
            ${v.lineas.map((l) => `
              <tr>
                <td>
                  <div class="fw-medium">${escaparHtml(l.productoNombre)}</div>
                  <div class="small text-secondary">${escaparHtml(l.productoCodigo)}<span class="d-sm-none"> · ${formatearMoneda(l.precioUnitario)}</span></div>
                </td>
                <td class="numero d-none d-sm-table-cell">${formatearMoneda(l.precioUnitario)}</td>
                <td class="numero">${l.cantidad}</td>
                <td class="numero">${formatearMoneda(l.subtotal)}</td>
              </tr>`).join('')}
          </tbody>
          <tfoot>
            <tr>
              <th scope="row" colspan="2" class="d-none d-sm-table-cell text-end">Total</th>
              <th scope="row" class="d-sm-none text-end">Total</th>
              <td class="numero fw-bold">${v.lineas.reduce((n, l) => n + l.cantidad, 0)}</td>
              <td class="numero fw-bold">${formatearMoneda(v.total)}</td>
            </tr>
          </tfoot>
        </table>
      </div>
      <p class="small text-secondary mt-2 mb-0">Los precios son los vigentes al momento de la venta.</p>`;
  } catch (error) {
    cuerpoDetalle.innerHTML = '<p class="text-danger mb-0">No se pudo cargar el detalle.</p>';
    mostrarError(error);
  }
}

// ===== Eventos =====

document.getElementById('btn-agregar').addEventListener('click', agregarProducto);
document.getElementById('btn-vaciar').addEventListener('click', vaciarCarrito);
inputCantidad.addEventListener('keydown', (e) => {
  // Enter en la cantidad agrega el producto en vez de enviar el formulario.
  if (e.key === 'Enter') {
    e.preventDefault();
    agregarProducto();
  }
});
selectProducto.addEventListener('change', () => selectProducto.classList.remove('is-invalid'));
selectCliente.addEventListener('change', () => selectCliente.classList.remove('is-invalid'));
tablaCarrito.addEventListener('input', (e) => {
  if (e.target.matches('.input-cantidad')) cambiarCantidad(e.target);
});
tablaCarrito.addEventListener('click', (e) => {
  const boton = e.target.closest('button[data-accion="quitar"]');
  if (boton) quitarProducto(Number(boton.closest('tr').dataset.id));
});
tablaHistorial.addEventListener('click', (e) => {
  const boton = e.target.closest('button[data-id]');
  if (boton) verDetalle(Number(boton.dataset.id));
});
form.addEventListener('submit', registrarVenta);

pintarCarrito();
try {
  await Promise.all([cargarClientes(), cargarProductos()]);
} catch (error) {
  mostrarError(error);
  selectCliente.innerHTML = '<option value="">No se pudieron cargar los clientes</option>';
  selectProducto.innerHTML = '<option value="">No se pudieron cargar los productos</option>';
}
cargarHistorial();

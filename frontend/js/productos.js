import { api, ApiError } from './api.js';
import {
  confirmar, escaparHtml, formatearMoneda, mostrarError, mostrarErroresDeCampo, mostrarToast,
  prepararFormulario, reiniciarFormulario, validarFormulario,
} from './ui.js';

const tabla = document.getElementById('tabla-productos');
const contador = document.getElementById('contador');
const buscador = document.getElementById('buscar');
const form = document.getElementById('form-producto');
const modalElemento = document.getElementById('modal-producto');
const modal = new bootstrap.Modal(modalElemento);
const tituloModal = document.getElementById('modal-producto-titulo');
const botonGuardar = document.getElementById('btn-guardar');

let productos = [];
let idEditando = null;
let ultimaConsulta = 0;

const filaMensaje = (texto, clase = 'text-secondary') =>
  `<tr><td colspan="6" class="${clase}">${escaparHtml(texto)}</td></tr>`;

async function cargar() {
  // Si el usuario sigue escribiendo, solo se pinta la respuesta de la última búsqueda.
  const consulta = ++ultimaConsulta;
  const texto = buscador.value.trim();
  try {
    const datos = await api('/productos' + (texto ? `?buscar=${encodeURIComponent(texto)}` : ''));
    if (consulta !== ultimaConsulta) return;
    productos = datos;
    pintar(texto);
  } catch (error) {
    if (consulta !== ultimaConsulta) return;
    mostrarError(error);
    tabla.innerHTML = filaMensaje('No se pudieron cargar los productos.', 'text-danger');
  }
}

// En móvil la insignia va debajo del número; desde 576 px, a su lado.
const posicionInsignia = 'd-block d-sm-inline-block mt-1 mt-sm-0 ms-sm-1';

function insigniaStock(p) {
  if (p.stock === 0) return `<span class="badge badge-agotado ${posicionInsignia}">Agotado</span>`;
  if (p.stockBajo) return `<span class="badge badge-stock-bajo ${posicionInsignia}">Bajo</span>`;
  return '';
}

function pintar(texto) {
  const bajos = productos.filter((p) => p.stockBajo).length;
  contador.textContent = `${productos.length} ${productos.length === 1 ? 'producto' : 'productos'}`
    + (bajos ? ` · ${bajos} con stock bajo` : '');

  if (productos.length === 0) {
    tabla.innerHTML = filaMensaje(texto ? `Ningún producto coincide con "${texto}".` : 'Aún no hay productos registrados.');
    return;
  }

  tabla.innerHTML = productos.map((p) => {
    const nombre = escaparHtml(p.nombre);
    return `
      <tr>
        <td class="d-none d-md-table-cell text-secondary">${escaparHtml(p.codigo)}</td>
        <td>
          <div class="fw-medium">${nombre}</div>
          <div class="small text-secondary d-md-none">${escaparHtml(p.codigo)}</div>
        </td>
        <td class="numero">${formatearMoneda(p.precio)}</td>
        <td class="numero">${p.stock}${insigniaStock(p)}</td>
        <td class="numero d-none d-lg-table-cell">${p.stockMinimo}</td>
        <td class="text-end">
          <div class="acciones">
            <button type="button" class="btn btn-sm btn-outline-primary" data-accion="editar" data-id="${p.id}" aria-label="Editar ${nombre}">
              <i class="bi bi-pencil" aria-hidden="true"></i>
            </button>
            <button type="button" class="btn btn-sm btn-outline-danger" data-accion="eliminar" data-id="${p.id}" aria-label="Eliminar ${nombre}">
              <i class="bi bi-trash" aria-hidden="true"></i>
            </button>
          </div>
        </td>
      </tr>`;
  }).join('');
}

function abrirModal(producto = null) {
  reiniciarFormulario(form);
  idEditando = producto?.id ?? null;
  tituloModal.textContent = producto ? 'Editar producto' : 'Nuevo producto';
  if (producto) {
    form.elements.codigo.value = producto.codigo;
    form.elements.nombre.value = producto.nombre;
    form.elements.descripcion.value = producto.descripcion ?? '';
    form.elements.precio.value = producto.precio;
    form.elements.stock.value = producto.stock;
    form.elements.stockMinimo.value = producto.stockMinimo;
  }
  modal.show();
}

async function guardar(evento) {
  evento.preventDefault();
  if (!validarFormulario(form)) return;

  const campos = form.elements;
  const datos = {
    codigo: campos.codigo.value,
    nombre: campos.nombre.value,
    descripcion: campos.descripcion.value || null,
    precio: Number(campos.precio.value),
    stock: Number(campos.stock.value),
    stockMinimo: Number(campos.stockMinimo.value),
  };

  // Deshabilitar el botón evita registrar dos veces con un doble clic.
  botonGuardar.disabled = true;
  try {
    if (idEditando) await api(`/productos/${idEditando}`, { method: 'PUT', body: datos });
    else await api('/productos', { method: 'POST', body: datos });
    modal.hide();
    mostrarToast(idEditando ? 'Producto actualizado.' : 'Producto registrado.');
    await cargar();
  } catch (error) {
    if (!(error instanceof ApiError && mostrarErroresDeCampo(form, error.errores))) mostrarError(error);
  } finally {
    botonGuardar.disabled = false;
  }
}

async function eliminar(producto) {
  const ok = await confirmar(`¿Eliminar ${producto.nombre}? Esta acción no se puede deshacer.`, { titulo: 'Eliminar producto' });
  if (!ok) return;
  try {
    await api(`/productos/${producto.id}`, { method: 'DELETE' });
    mostrarToast('Producto eliminado.');
    await cargar();
  } catch (error) {
    mostrarError(error);
  }
}

let temporizador;
buscador.addEventListener('input', () => {
  clearTimeout(temporizador);
  temporizador = setTimeout(cargar, 300);
});

document.getElementById('btn-nuevo').addEventListener('click', () => abrirModal());

tabla.addEventListener('click', (e) => {
  const boton = e.target.closest('button[data-accion]');
  if (!boton) return;
  const producto = productos.find((p) => p.id === Number(boton.dataset.id));
  if (boton.dataset.accion === 'editar') abrirModal(producto);
  else eliminar(producto);
});

modalElemento.addEventListener('shown.bs.modal', () => form.elements.codigo.focus());
prepararFormulario(form);
form.addEventListener('submit', guardar);

cargar();

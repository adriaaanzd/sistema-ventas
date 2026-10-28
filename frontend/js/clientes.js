import { api, ApiError } from './api.js';
import {
  confirmar, escaparHtml, mostrarError, mostrarErroresDeCampo, mostrarToast,
  prepararFormulario, reiniciarFormulario, validarFormulario,
} from './ui.js';

const tabla = document.getElementById('tabla-clientes');
const contador = document.getElementById('contador');
const buscador = document.getElementById('buscar');
const form = document.getElementById('form-cliente');
const modalElemento = document.getElementById('modal-cliente');
const modal = new bootstrap.Modal(modalElemento);
const tituloModal = document.getElementById('modal-cliente-titulo');
const botonGuardar = document.getElementById('btn-guardar');

let clientes = [];
let idEditando = null;
let ultimaConsulta = 0;

const filaMensaje = (texto, clase = 'text-secondary') =>
  `<tr><td colspan="5" class="${clase}">${escaparHtml(texto)}</td></tr>`;

async function cargar() {
  // Si el usuario sigue escribiendo, solo se pinta la respuesta de la última búsqueda.
  const consulta = ++ultimaConsulta;
  const texto = buscador.value.trim();
  try {
    const datos = await api('/clientes' + (texto ? `?buscar=${encodeURIComponent(texto)}` : ''));
    if (consulta !== ultimaConsulta) return;
    clientes = datos;
    pintar(texto);
  } catch (error) {
    if (consulta !== ultimaConsulta) return;
    mostrarError(error);
    tabla.innerHTML = filaMensaje('No se pudieron cargar los clientes.', 'text-danger');
  }
}

function pintar(texto) {
  contador.textContent = `${clientes.length} ${clientes.length === 1 ? 'cliente' : 'clientes'}`;
  if (clientes.length === 0) {
    tabla.innerHTML = filaMensaje(texto ? `Ningún cliente coincide con "${texto}".` : 'Aún no hay clientes registrados.');
    return;
  }

  tabla.innerHTML = clientes.map((c) => {
    const nombre = escaparHtml(c.nombre);
    return `
      <tr>
        <td>
          <div class="fw-medium">${nombre}</div>
          <div class="small text-secondary text-break d-md-none">${escaparHtml(c.correo)}</div>
        </td>
        <td>${escaparHtml(c.documento)}</td>
        <td class="d-none d-md-table-cell">${escaparHtml(c.correo)}</td>
        <td class="d-none d-lg-table-cell">${escaparHtml(c.telefono ?? '—')}</td>
        <td class="text-end">
          <div class="acciones">
            <button type="button" class="btn btn-sm btn-outline-primary" data-accion="editar" data-id="${c.id}" aria-label="Editar a ${nombre}">
              <i class="bi bi-pencil" aria-hidden="true"></i>
            </button>
            <button type="button" class="btn btn-sm btn-outline-danger" data-accion="eliminar" data-id="${c.id}" aria-label="Eliminar a ${nombre}">
              <i class="bi bi-trash" aria-hidden="true"></i>
            </button>
          </div>
        </td>
      </tr>`;
  }).join('');
}

function abrirModal(cliente = null) {
  reiniciarFormulario(form);
  idEditando = cliente?.id ?? null;
  tituloModal.textContent = cliente ? 'Editar cliente' : 'Nuevo cliente';
  if (cliente) {
    form.elements.nombre.value = cliente.nombre;
    form.elements.documento.value = cliente.documento;
    form.elements.correo.value = cliente.correo;
    form.elements.telefono.value = cliente.telefono ?? '';
    form.elements.direccion.value = cliente.direccion ?? '';
  }
  modal.show();
}

async function guardar(evento) {
  evento.preventDefault();
  if (!validarFormulario(form)) return;

  const campos = form.elements;
  const datos = {
    nombre: campos.nombre.value,
    documento: campos.documento.value,
    correo: campos.correo.value,
    telefono: campos.telefono.value || null,
    direccion: campos.direccion.value || null,
  };

  // Deshabilitar el botón evita registrar dos veces con un doble clic.
  botonGuardar.disabled = true;
  try {
    if (idEditando) await api(`/clientes/${idEditando}`, { method: 'PUT', body: datos });
    else await api('/clientes', { method: 'POST', body: datos });
    modal.hide();
    mostrarToast(idEditando ? 'Cliente actualizado.' : 'Cliente registrado.');
    await cargar();
  } catch (error) {
    if (!(error instanceof ApiError && mostrarErroresDeCampo(form, error.errores))) mostrarError(error);
  } finally {
    botonGuardar.disabled = false;
  }
}

async function eliminar(cliente) {
  const ok = await confirmar(`¿Eliminar a ${cliente.nombre}? Esta acción no se puede deshacer.`, { titulo: 'Eliminar cliente' });
  if (!ok) return;
  try {
    await api(`/clientes/${cliente.id}`, { method: 'DELETE' });
    mostrarToast('Cliente eliminado.');
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
  const cliente = clientes.find((c) => c.id === Number(boton.dataset.id));
  if (boton.dataset.accion === 'editar') abrirModal(cliente);
  else eliminar(cliente);
});

modalElemento.addEventListener('shown.bs.modal', () => form.elements.nombre.focus());
prepararFormulario(form);
form.addEventListener('submit', guardar);

cargar();

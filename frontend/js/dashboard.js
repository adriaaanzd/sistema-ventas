import { api } from './api.js';
import { LOCALE, MONEDA, escaparHtml, formatearFecha, formatearMoneda, mostrarError } from './ui.js';

const monedaSinDecimales = new Intl.NumberFormat(LOCALE, { style: 'currency', currency: MONEDA, maximumFractionDigits: 0 });

/** "2026-05" → "may 2026" */
function etiquetaMes(mes) {
  const [anio, numero] = mes.split('-').map(Number);
  return new Date(anio, numero - 1).toLocaleDateString(LOCALE, { month: 'short', year: 'numeric' });
}

function pintarIndicadores(d) {
  document.getElementById('kpi-clientes').textContent = d.totalClientes;
  document.getElementById('kpi-productos').textContent = d.totalProductos;
  document.getElementById('kpi-ventas').textContent = d.totalVentas;
  document.getElementById('kpi-ingresos').textContent = formatearMoneda(d.ingresos);
}

function pintarGrafico(ventasPorMes) {
  const canvas = document.getElementById('grafico-ventas');
  const etiquetas = ventasPorMes.map((m) => etiquetaMes(m.mes));
  const colorPrimario = getComputedStyle(document.documentElement).getPropertyValue('--color-primario').trim();

  // El canvas no es legible para lectores de pantalla; el resumen va en su aria-label.
  const resumen = ventasPorMes.map((m, i) => `${etiquetas[i]}: ${formatearMoneda(m.total)}`).join('; ');
  canvas.setAttribute('aria-label', `Ingresos por mes. ${resumen}`);

  new Chart(canvas, {
    type: 'bar',
    data: {
      labels: etiquetas,
      datasets: [{
        label: 'Ingresos',
        data: ventasPorMes.map((m) => m.total),
        backgroundColor: colorPrimario,
        borderRadius: 6,
        maxBarThickness: 40,
      }],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { display: false },
        tooltip: {
          callbacks: {
            label: (ctx) => {
              const cantidad = ventasPorMes[ctx.dataIndex].cantidad;
              return `${formatearMoneda(ctx.parsed.y)} · ${cantidad} ${cantidad === 1 ? 'venta' : 'ventas'}`;
            },
          },
        },
      },
      scales: {
        x: { grid: { display: false } },
        y: { beginAtZero: true, ticks: { callback: (valor) => monedaSinDecimales.format(valor) } },
      },
    },
  });
}

function pintarStockBajo(productos) {
  const lista = document.getElementById('lista-stock-bajo');
  if (productos.length === 0) {
    lista.innerHTML = '<li class="list-group-item text-secondary">Todos los productos tienen stock suficiente.</li>';
    return;
  }

  lista.innerHTML = productos.map((p) => {
    const insignia = p.stock === 0
      ? '<span class="badge badge-agotado">Agotado</span>'
      : `<span class="badge badge-stock-bajo">${p.stock} / mín. ${p.stockMinimo}</span>`;
    return `
      <li class="list-group-item d-flex justify-content-between align-items-center gap-2">
        <div class="text-truncate">
          <div class="fw-medium text-truncate">${escaparHtml(p.nombre)}</div>
          <div class="small text-secondary">${escaparHtml(p.codigo)}</div>
        </div>
        ${insignia}
      </li>`;
  }).join('');
}

function pintarUltimasVentas(ventas) {
  const cuerpo = document.getElementById('tabla-ultimas');
  if (ventas.length === 0) {
    cuerpo.innerHTML = '<tr><td colspan="5" class="text-secondary">Aún no hay ventas registradas.</td></tr>';
    return;
  }

  cuerpo.innerHTML = ventas.map((v) => `
    <tr>
      <td class="fw-medium">#${v.id}</td>
      <td class="text-nowrap">${formatearFecha(v.fecha)}</td>
      <td>${escaparHtml(v.clienteNombre)}</td>
      <td class="numero d-none d-md-table-cell">${v.cantidadProductos}</td>
      <td class="numero fw-medium">${formatearMoneda(v.total)}</td>
    </tr>`).join('');
}

function pintarError() {
  document.getElementById('lista-stock-bajo').innerHTML =
    '<li class="list-group-item text-danger">No se pudieron cargar los datos.</li>';
  document.getElementById('tabla-ultimas').innerHTML =
    '<tr><td colspan="5" class="text-danger">No se pudieron cargar los datos.</td></tr>';
}

try {
  const datos = await api('/dashboard');
  pintarIndicadores(datos);
  pintarGrafico(datos.ventasPorMes);
  pintarStockBajo(datos.productosStockBajo);
  pintarUltimasVentas(datos.ultimasVentas);
} catch (error) {
  mostrarError(error);
  pintarError();
}

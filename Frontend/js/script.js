let currentTab = 'dashboard';
let searchTimeout;
let autocompleteTimeout;

function initializeApp() {
    setDefaultDates();
    loadListas();
    loadDashboard();
    showTab('dashboard');
}

function setDefaultDates() {
    const today = new Date();
    const monthAgo = new Date();
    monthAgo.setMonth(monthAgo.getMonth() - 1);

    document.getElementById("fechaMov").valueAsDate = today;
    document.getElementById("fechaDesde").valueAsDate = monthAgo;
    document.getElementById("fechaHasta").valueAsDate = today;
}

function showTab(tabName) {
    document.querySelectorAll('.tab-content').forEach(tab => {
        tab.classList.remove('active');
    });
      
    document.querySelectorAll('.nav-link').forEach(link => {
        link.classList.remove('active');
    });
      
    document.getElementById(tabName).classList.add('active');
    event.target.classList.add('active');
      
    currentTab = tabName;
      
    switch(tabName) {
        case 'dashboard':
          loadDashboard();
          break;
        case 'inventario':
          mostrarStock();
          break;
    }
}

function loadDashboard() {
    google.script.run.withSuccessHandler(data => {
        const statsGrid = document.getElementById('statsGrid');
        statsGrid.innerHTML = `
          <div class="stat-card">
            <div class="stat-value">${data.totalProductos}</div>
            <div class="stat-label">Total Productos</div>
          </div>

          <div class="stat-card">
            <div class="stat-value">${data.totalMovimientos}</div>
            <div class="stat-label">Total Movimientos</div>
          </div>

          <div class="stat-card">
            <div class="stat-value">${data.sinStock}</div>
            <div class="stat-label">Sin Stock</div>
          </div>

          <div class="stat-card">
            <div class="stat-value">${data.stockBajo}</div>
            <div class="stat-label">Stock Bajo</div>
          </div>
        `;
    }).withFailureHandler(error => {
        showMessage('statsGrid', 'Error al cargar dashboard: ' + error, 'error');
    }).obtenerResumen();
}

function loadListas() {
      
    google.script.run.withSuccessHandler(data => {
        const unidadSelect = document.getElementById("unidadProd");
        const grupoSelect = document.getElementById("grupoProd");

        unidadSelect.innerHTML = "";
        grupoSelect.innerHTML = "";

        data.unidades.forEach(u => {
          unidadSelect.innerHTML += `
            <option value="${u}">${u}</option>
          `;
        });
        data.grupos.forEach(g => {
          grupoSelect.innerHTML += `
            <option value="${g}">${g}</option>
          `;
        });
      
    }).withFailureHandler(error => {
        console.error('Error loading lists:', error);  
    }).obtenerListas();
}

function buscarProductoAutocompletado() {
    clearTimeout(autocompleteTimeout);
    const input = document.getElementById("codigoMov");
    const dropdown = document.getElementById("autocompleteDropdown");
    const codigo = input.value.trim().toUpperCase();
      
    if (codigo.length === 0) {
        dropdown.style.display = "none";
        return;
    }
      
    autocompleteTimeout = setTimeout(() => {
        google.script.run.withSuccessHandler(productos => {
          mostrarAutocompletado(productos);
        }).withFailureHandler(error => {
          console.error('Error en autocompletado:', error);
        }).buscarProductoPorCodigo(codigo);
    }, 200);
}

function mostrarAutocompletado(productos = []) {
    const dropdown = document.getElementById("autocompleteDropdown");
      
    if (productos.length === 0) {
        dropdown.style.display = "none";
        return;
    }
      
    let html = "";
    productos.forEach(producto => {
        html += `
          <div class="autocomplete-item" onmousedown="seleccionarProducto('${producto.codigo}', '${producto.nombre}')">
            <div class="autocomplete-code">${producto.codigo}</div>
            <div class="autocomplete-name">${producto.nombre} - ${producto.grupo}</div>
          </div>
        `;
    });
      
    dropdown.innerHTML = html;
    dropdown.style.display = "block";
}

function seleccionarProducto(codigo, nombre) {
    document.getElementById("codigoMov").value = codigo;
    document.getElementById("autocompleteDropdown").style.display = "none";
}

function ocultarAutocompletado() {
    setTimeout(() => {
        document.getElementById("autocompleteDropdown").style.display = "none";
    }, 150);
}

function registrarProducto(event) {
    event.preventDefault();
      
    const producto = {
        codigo: document.getElementById("codigoProd").value.trim().toUpperCase(),
        nombre: document.getElementById("nombreProd").value.trim(),
        unidad: document.getElementById("unidadProd").value,
        grupo: document.getElementById("grupoProd").value,
        stockMin: parseInt(document.getElementById("stockMinProd").value) || 0
    };

    if (!producto.codigo || !producto.nombre) {
        showMessage('msgProd', 'Código y nombre son campos obligatorios', 'error');
        return;
    }

    google.script.run.withSuccessHandler(mensaje => {
        showMessage('msgProd', mensaje, mensaje.includes('correctamente') ? 'success' : 'error');
        if (mensaje.includes('correctamente')) {
          document.getElementById('formProducto').reset();
          document.getElementById("stockMinProd").value = "0";
        }
    }).withFailureHandler(error => {
        showMessage('msgProd', 'Error: ' + error, 'error');
    }).registrarProducto(producto);
}

function registrarMovimiento(event) {
    event.preventDefault();
      
    const movimiento = {
        codigo: document.getElementById("codigoMov").value.trim().toUpperCase(),
        fecha: document.getElementById("fechaMov").value,
        tipo: document.getElementById("tipoMov").value,
        cantidad: parseFloat(document.getElementById("cantMov").value) || 0,
        observaciones: document.getElementById("obsMov").value.trim()
    };

    if (!movimiento.codigo || !movimiento.fecha || movimiento.cantidad <= 0) {
        showMessage('msgMov', 'Todos los campos son obligatorios y la cantidad debe ser mayor a 0', 'error');
        return;
    }

    google.script.run.withSuccessHandler(mensaje => {
        showMessage('msgMov', mensaje, mensaje.includes('correctamente') ? 'success' : 'error');
        if (mensaje.includes('correctamente')) {
          document.getElementById('formMovimiento').reset();
          document.getElementById("fechaMov").valueAsDate = new Date();
        }
    }).withFailureHandler(error => {
        showMessage('msgMov', 'Error: ' + error, 'error');
    }).registrarMovimiento(movimiento);
}

function handleTipoChange() {
    const tipo = document.getElementById("tipoMov").value;
    const cantField = document.getElementById("cantMov");
      
    switch(tipo) {
        case 'INGRESO':
          cantField.placeholder = 'Cantidad a ingresar';
          break;
        case 'SALIDA':
          cantField.placeholder = 'Cantidad a retirar';
          break;
        case 'AJUSTE_POSITIVO':
          cantField.placeholder = 'Cantidad a aumentar';
          break;
        case 'AJUSTE_NEGATIVO':
          cantField.placeholder = 'Cantidad a disminuir';
          break;
    }
}

function buscarProducto() {
    const texto = document.getElementById("buscarTexto").value.trim();
    if (!texto) {
        showMessage('resultadosBusqueda', 'Ingrese un texto para buscar', 'warning');
        return;
    }

    google.script.run.withSuccessHandler(data => {
        displaySearchResults(data);
    }).withFailureHandler(error => {
        showMessage('resultadosBusqueda', 'Error en la búsqueda: ' + error, 'error');
    }).buscarProducto(texto);
}

function buscarEnTiempoReal() {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
        const texto = document.getElementById("buscarTexto").value.trim();
        if (texto.length >= 2) {
          buscarProducto();
        } else if (texto.length === 0) {
          document.getElementById('resultadosBusqueda').innerHTML = '';
        }
    }, 300);
}

function displaySearchResults(data) {
    const container = document.getElementById('resultadosBusqueda');
      
    if (data.length === 0) {
        container.innerHTML = '<div class="message warning">No se encontraron productos</div>';
        return;
    }

    let html = `
        <table>
          <thead>
            <tr>
              <th>Código</th>
              <th>Nombre</th>
              <th>Unidad</th>
              <th>Grupo</th>
              <th>Stock Mín.</th>
              <th>Stock Actual</th>
              <th>Estado</th>
            </tr>
          </thead>
          <tbody>
      `;

      data.forEach(producto => {
        const [codigo, nombre, unidad, grupo, stockMin, stockActual] = producto;
        let statusClass = 'status-normal';
        let estado = 'Normal';
        
        if (stockActual <= 0) {
          statusClass = 'status-zero';
          estado = 'Sin Stock';
        } else if (stockActual <= stockMin && stockMin > 0) {
          statusClass = 'status-low';
          estado = 'Stock Bajo';
        }

        html += `
          <tr class="${statusClass}">
            <td>${codigo}</td>
            <td>${nombre}</td>
            <td>${unidad}</td>
            <td>${grupo}</td>
            <td>${stockMin}</td>
            <td>${stockActual}</td>
            <td>${estado}</td>
          </tr>
        `;
    });

    html += '</tbody></table>';
    container.innerHTML = html;
}

function mostrarStock() {
    const loading = document.getElementById("loading");
    const container = document.getElementById("stockTable");
      
    loading.style.display = "block";
      
    google.script.run.withSuccessHandler(data => {
        loading.style.display = "none";
        displayStockTable(data, container);
    }).withFailureHandler(error => {
        loading.style.display = "none";
        showMessage('stockTable', 'Error al cargar stock: ' + error, 'error');
    }).obtenerStock();
}

function displayStockTable(data, container) {
    if (data.length === 0) {
        container.innerHTML = '<div class="message warning">No hay productos registrados</div>';
        return;
    }

      let html = `
        <table>
          <thead>
            <tr>
              <th>Código</th>
              <th>Nombre</th>
              <th>Unidad</th>
              <th>Grupo</th>
              <th>Stock Mín.</th>
              <th>Stock Actual</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
      `;

      data.forEach(producto => {
        let statusClass = 'status-normal';
        let estado = 'Normal';
        
        if (producto.cantidad <= 0) {
          statusClass = 'status-zero';
          estado = 'Sin Stock';
        } else if (producto.cantidad <= producto.stockMin && producto.stockMin > 0) {
          statusClass = 'status-low';
          estado = 'Stock Bajo';
        }

        html += `
          <tr class="${statusClass}">
            <td>${producto.codigo}</td>
            <td>${producto.nombre}</td>
            <td>${producto.unidad}</td>
            <td>${producto.grupo}</td>
            <td>${producto.stockMin}</td>
            <td>${producto.cantidad}</td>
            <td>${estado}</td>
            <td>
              <button class="btn btn-info" onclick="verDetalleProducto('${producto.codigo}')" title="Ver detalle">
                Ver
              </button>
            </td>
          </tr>
        `;
    });

    html += '</tbody></table>';
    container.innerHTML = html;
}

function mostrarAlertas() {
    const loading = document.getElementById("loading");
    const container = document.getElementById("stockTable");
      
    loading.style.display = "block";
      
    google.script.run.withSuccessHandler(data => {
        loading.style.display = "none";
        const alertProducts = data.filter(p => p.cantidad <= 0 || (p.cantidad <= p.stockMin && p.stockMin > 0));
        
        if (alertProducts.length === 0) {
          container.innerHTML = '<div class="message success">No hay productos con alertas de stock</div>';
          return;
        }
        
        displayStockTable(alertProducts, container);
    }).withFailureHandler(error => {
        loading.style.display = "none";
        showMessage('stockTable', 'Error: ' + error, 'error');
    }).obtenerStock();
}

function showStockAlerts() {
    google.script.run.withSuccessHandler(data => {
        const alertProducts = data.filter(p => p.cantidad <= 0 || (p.cantidad <= p.stockMin && p.stockMin > 0));
        const container = document.getElementById('alertsContainer');
        
        if (alertProducts.length === 0) {
          container.innerHTML = '<div class="message success">No hay productos con alertas de stock</div>';
          return;
        }

        let html = `
          <div class="message warning">
            <strong>${alertProducts.length} producto(s) requieren atención</strong>
          </div>
          <table>
            <thead>
              <tr><th>Código</th><th>Nombre</th><th>Stock Actual</th><th>Stock Mín.</th><th>Estado</th></tr>
            </thead>
            <tbody>
        `;

        alertProducts.forEach(p => {
          const estado = p.cantidad <= 0 ? 'Sin Stock' : 'Stock Bajo';
          const statusClass = p.cantidad <= 0 ? 'status-zero' : 'status-low';
          
          html += `
            <tr class="${statusClass}">
              <td>${p.codigo}</td>
              <td>${p.nombre}</td>
              <td>${p.cantidad}</td>
              <td>${p.stockMin}</td>
              <td>${estado}</td>
            </tr>
          `;
        });

        html += '</tbody></table>';
        container.innerHTML = html;
    }).obtenerStock();
}

function mostrarHistorial() {
    const filtros = {
        fechaDesde: document.getElementById("fechaDesde").value,
        fechaHasta: document.getElementById("fechaHasta").value,
        tipo: document.getElementById("filtroTipo").value
    };

    if (!filtros.fechaDesde || !filtros.fechaHasta) {
        showMessage('historialTable', 'Seleccione las fechas de consulta', 'warning');
        return;
    }

    google.script.run.withSuccessHandler(data => {
        displayHistorialTable(data);
    }).withFailureHandler(error => {
        showMessage('historialTable', 'Error: ' + error, 'error');
    }).obtenerHistorial(filtros);
}

function displayHistorialTable(data) {
    const container = document.getElementById('historialTable');
      
    if (data.length === 0) {
        container.innerHTML = '<div class="message warning">No hay movimientos en el período seleccionado</div>';
        return;
    }

      let html = `
        <div class="message success">Se encontraron ${data.length} movimientos</div>
        <table>
          <thead>
            <tr>
              <th>Fecha</th>
              <th>Código</th>
              <th>Producto</th>
              <th>Tipo</th>
              <th>Cantidad</th>
              <th>Observaciones</th>
            </tr>
          </thead>
          <tbody>
      `;

      data.forEach(mov => {
        let tipoClass = 'text-success';
        let tipoText = mov.tipo;
        
        switch(mov.tipo) {
          case 'INGRESO':
            tipoClass = 'text-success';
            tipoText = 'Ingreso';
            break;
          case 'SALIDA':
            tipoClass = 'text-danger';
            tipoText = 'Salida';
            break;
          case 'AJUSTE_POSITIVO':
            tipoClass = 'text-success';
            tipoText = 'Ajuste +';
            break;
          case 'AJUSTE_NEGATIVO':
            tipoClass = 'text-danger';
            tipoText = 'Ajuste -';
            break;
          case 'AJUSTE':
            tipoClass = 'text-warning';
            tipoText = 'Ajuste';
            break;
        }

        html += `
          <tr>
            <td>${mov.fecha}</td>
            <td>${mov.codigo}</td>
            <td>${mov.producto}</td>
            <td class="${tipoClass}">${tipoText}</td>
            <td>${mov.cantidad}</td>
            <td>${mov.observaciones}</td>
          </tr>
        `;
    });

    html += '</tbody></table>';
    container.innerHTML = html;
}

function validarIntegridad() {
    google.script.run.withSuccessHandler(data => {
        let html = '<h4>Validación de Integridad del Sistema</h4>';
        
        if (data.errores.length === 0) {
          html += '<div class="message success">Todos los datos están correctos. El sistema está íntegro.</div>';
        } else {
          html += '<div class="message error"><strong>Se encontraron los siguientes errores:</strong></div>';
          html += '<ul>';
          data.errores.forEach(error => {
            html += `<li class="text-danger">${error}</li>`;
          });
          html += '</ul>';
        }
        
        document.getElementById("configResults").innerHTML = html;
    }).withFailureHandler(error => {
        showMessage('configResults', 'Error en validación: ' + error, 'error');
    }).validarIntegridad();
}

function inicializarSistema() {
    if (confirm('¿Está seguro de que desea inicializar el sistema? Esto creará las hojas necesarias si no existen.')) {
        google.script.run.withSuccessHandler(mensaje => {
          showMessage('configResults', mensaje, mensaje.includes('correctamente') ? 'success' : 'error');
          if (mensaje.includes('correctamente')) {
            loadListas();
            loadDashboard();
          }
        }).withFailureHandler(error => {
          showMessage('configResults', 'Error: ' + error, 'error');
        }).inicializarHojas();
    }
}

function exportarStock() {
    google.script.run.withSuccessHandler(url => {
        if (url) {
          window.open(url, '_blank');
          showMessage('stockTable', 'Stock exportado exitosamente', 'success');
        } else {
          showMessage('stockTable', 'Error al exportar stock', 'error');
        }
    }).withFailureHandler(error => {
        showMessage('stockTable', 'Error: ' + error, 'error');
    }).exportarStockCSV();
}

function limpiarFormProducto() {
    document.getElementById('formProducto').reset();
    document.getElementById("stockMinProd").value = "0";
    document.getElementById('msgProd').innerHTML = '';
}

function limpiarFormMovimiento() {
    document.getElementById('formMovimiento').reset();
    document.getElementById("fechaMov").valueAsDate = new Date();
    document.getElementById('msgMov').innerHTML = '';
    document.getElementById("autocompleteDropdown").style.display = "none";
}

function limpiarBusqueda() {
    document.getElementById("buscarTexto").value = "";
    document.getElementById("resultadosBusqueda").innerHTML = "";
}

function limpiarTodosFormularios() {
    limpiarFormProducto();
    limpiarFormMovimiento();
    limpiarBusqueda();
    document.getElementById('historialTable').innerHTML = '';
    document.getElementById('configResults').innerHTML = '';
}

function showMessage(containerId, message, type) {
    const container = document.getElementById(containerId);
    let className = 'message';
      
    switch(type) {
        case 'success':
          className += ' success';
          break;
        case 'error':
          className += ' error';
          break;
        case 'warning':
          className += ' warning';
          break;
        case 'info':
          className += ' info';
          break;
        default:
          className += ' success';
    }
      
    container.innerHTML = `<div class="${className}">${message}</div>`;
      
    if (type === 'success') {
        setTimeout(() => {
          container.innerHTML = '';
        }, 5000);
    }
}

function verDetalleProducto(codigo) {
    alert(`Funcionalidad de detalle para producto: ${codigo}\nEsta función se implementará próximamente.`);
}

function confirmarReset() {
    if (confirm('ADVERTENCIA: Esta acción eliminará TODOS los datos del sistema.\n\n¿Está completamente seguro de que desea continuar?')) {
        if (confirm('Esta es su última oportunidad para cancelar.\n\n¿Proceder con el reset completo?')) {
          alert('Funcionalidad de reset no implementada por seguridad.\nContacte al administrador del sistema.');
        }
    }
}

function exportarReporte() {
    const filtros = {
        fechaDesde: document.getElementById("fechaDesde").value,
        fechaHasta: document.getElementById("fechaHasta").value,
        tipo: document.getElementById("filtroTipo").value
    };

    if (!filtros.fechaDesde || !filtros.fechaHasta) {
        showMessage('historialTable', 'Seleccione las fechas para exportar', 'warning');
        return;
    }

    google.script.run.withSuccessHandler(data => {
        if (data.length === 0) {
          showMessage('historialTable', 'No hay datos para exportar en el período seleccionado', 'warning');
          return;
        }

        let csv = 'Fecha,Código,Producto,Tipo,Cantidad,Observaciones\n';
        data.forEach(mov => {
          csv += `"${mov.fecha}","${mov.codigo}","${mov.producto}","${mov.tipo}","${mov.cantidad}","${mov.observaciones}"\n`;
        });

        const blob = new Blob([csv], { type: 'text/csv' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.style.display = 'none';
        a.href = url;
        a.download = `Reporte_Movimientos_${filtros.fechaDesde}_${filtros.fechaHasta}.csv`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        
        showMessage('historialTable', 'Reporte exportado exitosamente', 'success');
    }).obtenerHistorial(filtros);
}

document.addEventListener('keydown', function(event) {
    if ((event.ctrlKey || event.metaKey) && event.key === 's') {
        event.preventDefault();
        
        switch(currentTab) {
          case 'productos':
            document.getElementById('formProducto').dispatchEvent(new Event('submit'));
            break;
          case 'movimientos':
            document.getElementById('formMovimiento').dispatchEvent(new Event('submit'));
            break;
        }
    }
      
    if ((event.ctrlKey || event.metaKey) && event.key === 'r') {
        event.preventDefault();
        
        switch(currentTab) {
          case 'dashboard':
            loadDashboard();
            break;
          case 'inventario':
            mostrarStock();
            break;
        }
    }
});

document.addEventListener('click', function(event) {
    if (!event.target.closest('.autocomplete-container')) {
        document.getElementById("autocompleteDropdown").style.display = "none";
    }
});
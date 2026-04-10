const API_BASE_KEY = 'smartsales.apiBaseUrl';

const paths = {
  clientes: '/api/Cliente',
  productos: '/api/Producto',
  usuarios: '/api/Usuario',
  ventas: '/api/Venta',
  configuracion: '/api/Configuracion'
};

const TOKEN_KEY = 'smartsales_token';
const USER_KEY = 'smartsales_user';

function getToken() {
  return localStorage.getItem(TOKEN_KEY);
}

function getCurrentUser() {
  const raw = localStorage.getItem(USER_KEY);
  return raw ? JSON.parse(raw) : null;
}

function setSession(token, user) {
  localStorage.setItem(TOKEN_KEY, token);
  localStorage.setItem(USER_KEY, JSON.stringify(user));
}

function clearSession() {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}

function showLogin() {
  const loginView = document.getElementById('loginView');
  const appView = document.getElementById('appView');
  if (loginView) loginView.style.setProperty('display', 'flex', 'important');
  if (appView) appView.style.setProperty('display', 'none', 'important');
}

function showApp() {
  const loginView = document.getElementById('loginView');
  const appView = document.getElementById('appView');
  if (loginView) loginView.style.setProperty('display', 'none', 'important');
  if (appView) appView.style.setProperty('display', 'block', 'important');
}

function applyRolePermissions() {
  const user = getCurrentUser();
  const role = user?.rol;

  const usersNavItem = document.getElementById('usersNavItem');
  const usuariosTab = document.getElementById('usuariosTab');
  const configBtn = document.getElementById('configBtn');

  const isAdmin = role === 'Administrador';
  if (usersNavItem) usersNavItem.style.display = isAdmin ? '' : 'none';
  if (usuariosTab) usuariosTab.style.display = isAdmin ? '' : 'none';
  if (configBtn) configBtn.style.display = isAdmin ? '' : 'none';

  const authInfo = document.getElementById('authInfo');
  if (authInfo) {
    authInfo.textContent = user ? `${user.nombre} (${user.rol})` : 'No autenticado';
  }
}

function getApiBaseUrl() {
  return (localStorage.getItem(API_BASE_KEY) || '').trim();
}

function apiUrl(path) {
  const base = getApiBaseUrl();
  if (!base) return path;
  return `${base.replace(/\/$/, '')}${path}`;
}

function showAlert(message, type = 'success') {
  const host = document.getElementById('alertContainer');
  if (!host || host.offsetParent === null) {
    window.alert(message);
    return;
  }
  host.innerHTML = `<div class="alert alert-${type} alert-dismissible fade show" role="alert">${message}<button type="button" class="btn-close" data-bs-dismiss="alert"></button></div>`;
}

async function request(url, options = {}) {
  const headers = {
    'Content-Type': 'application/json',
    ...(options.headers || {})
  };

  const token = getToken();
  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const response = await fetch(url, { ...options, headers });

  if (response.status === 401) {
    clearSession();
    applyRolePermissions();
    showLogin();
    throw new Error('Sesión expirada o no autorizada.');
  }

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `Error ${response.status}`);
  }

  if (response.status === 204) return null;
  return response.json();
}

function rowsFrom(containerId, rowsHtml) {
  document.getElementById(containerId).innerHTML = rowsHtml;
}

function asArray(payload) {
  if (Array.isArray(payload)) return payload;
  if (payload && Array.isArray(payload.data)) return payload.data;
  if (payload && Array.isArray(payload.detalles)) return payload.detalles;
  return [];
}

async function loadConfigForm() {
  document.getElementById('configApiBaseUrl').value = getApiBaseUrl();

  try {
    const config = await request(apiUrl(paths.configuracion));
    document.getElementById('configConnectionString').value = config?.connectionString ?? '';
  } catch {
    document.getElementById('configConnectionString').value = '';
  }
}

async function loadClientes(url = apiUrl(paths.clientes)) {
  const payload = await request(url);
  const list = asArray(payload);
  rowsFrom('clientesBody', list.map(c => `
    <tr>
      <td>${c.idCliente ?? ''}</td>
      <td>${c.nombre ?? ''}</td>
      <td>${c.email ?? ''}</td>
      <td>${c.telefono ?? ''}</td>
      <td><button class="btn btn-sm btn-outline-danger" data-id="${c.idCliente}" data-action="del-cliente">Eliminar</button></td>
    </tr>`).join(''));
  document.getElementById('countClientes').textContent = list.length;
}

async function loadProductos(url = apiUrl(paths.productos)) {
  const payload = await request(url);
  const list = asArray(payload);
  rowsFrom('productosBody', list.map(p => `
    <tr>
      <td>${p.id ?? ''}</td>
      <td>${p.nombre ?? ''}</td>
      <td>${p.precio ?? ''}</td>
      <td>${p.stock ?? ''}</td>
      <td><button class="btn btn-sm btn-outline-danger" data-id="${p.id}" data-action="del-producto">Eliminar</button></td>
    </tr>`).join(''));
  document.getElementById('countProductos').textContent = list.length;
}

async function loadUsuarios(url = apiUrl(paths.usuarios)) {
  const payload = await request(url);
  const list = asArray(payload);
  rowsFrom('usuariosBody', list.map(u => `
    <tr>
      <td>${u.idUsuario ?? ''}</td>
      <td>${u.nombre ?? ''}</td>
      <td>${u.email ?? ''}</td>
      <td>${u.rol ?? ''}</td>
      <td>${u.estado ? 'Activo' : 'Inactivo'}</td>
      <td><button class="btn btn-sm btn-outline-secondary" data-id="${u.idUsuario}" data-state="${!u.estado}" data-action="toggle-usuario">Cambiar</button></td>
    </tr>`).join(''));
  document.getElementById('countUsuarios').textContent = list.length;
}

async function loadVentas() {
  try {
    const payload = await request(apiUrl(paths.ventas));
    const list = asArray(payload);
    rowsFrom('ventasBody', list.map(v => `
      <tr>
        <td>${v.idVenta ?? ''}</td>
        <td>${v.nombreCliente ?? v.idCliente ?? ''}</td>
        <td>${v.nombreVendedor ?? v.idUsuario ?? ''}</td>
        <td>${v.fecha ? new Date(v.fecha).toLocaleString() : ''}</td>
        <td>${v.total ?? ''}</td>
      </tr>`).join(''));
    document.getElementById('countVentas').textContent = list.length;
  } catch {
    rowsFrom('ventasBody', '');
    document.getElementById('countVentas').textContent = '0';
  }
}

document.getElementById('formCliente')?.addEventListener('submit', async (e) => {
  e.preventDefault();
  const f = new FormData(e.target);
  try {
    await request(apiUrl(paths.clientes), {
      method: 'POST',
      body: JSON.stringify({
        nombre: f.get('nombre'),
        email: f.get('email'),
        telefono: f.get('telefono')
      })
    });
    e.target.reset();
    await loadClientes();
    showAlert('Cliente creado.');
  } catch (err) { showAlert(err.message, 'danger'); }
});

document.getElementById('formProducto')?.addEventListener('submit', async (e) => {
  e.preventDefault();
  const f = new FormData(e.target);
  try {
    await request(apiUrl(paths.productos), {
      method: 'POST',
      body: JSON.stringify({
        nombre: f.get('nombre'),
        precio: Number(f.get('precio')),
        stock: Number(f.get('stock'))
      })
    });
    e.target.reset();
    await loadProductos();
    showAlert('Producto creado.');
  } catch (err) { showAlert(err.message, 'danger'); }
});

document.getElementById('formUsuario')?.addEventListener('submit', async (e) => {
  e.preventDefault();
  const f = new FormData(e.target);
  try {
    await request(apiUrl(paths.usuarios), {
      method: 'POST',
      body: JSON.stringify({
        nombre: f.get('nombre'),
        email: f.get('email'),
        contrasenaHash: f.get('contrasenaHash'),
        rol: f.get('rol')
      })
    });
    e.target.reset();
    await loadUsuarios();
    showAlert('Usuario creado.');
  } catch (err) { showAlert(err.message, 'danger'); }
});

document.getElementById('formVenta')?.addEventListener('submit', async (e) => {
  e.preventDefault();
  const f = new FormData(e.target);
  const cantidad = Number(f.get('cantidad'));
  const precioUnitario = Number(f.get('precioUnitario'));

  try {
    await request(apiUrl(paths.ventas), {
      method: 'POST',
      body: JSON.stringify({
        idCliente: Number(f.get('idCliente')),
        idUsuario: Number(f.get('idUsuario')),
        detalles: [
          {
            idProducto: Number(f.get('idProducto')),
            cantidad,
            precioUnitario,
            subtotal: cantidad * precioUnitario
          }
        ]
      })
    });
    e.target.reset();
    await loadVentas();
    showAlert('Venta registrada.');
  } catch (err) { showAlert(err.message, 'danger'); }
});

document.getElementById('btnBuscarCliente')?.addEventListener('click', async () => {
  const v = document.getElementById('buscarCliente').value.trim();
  if (!v) return loadClientes();
  try { await loadClientes(apiUrl(`${paths.clientes}/Buscar?nombre=${encodeURIComponent(v)}`)); }
  catch (err) { showAlert(err.message, 'danger'); }
});

document.getElementById('btnBuscarProducto')?.addEventListener('click', async () => {
  const v = document.getElementById('buscarProducto').value.trim();
  if (!v) return loadProductos();
  try { await loadProductos(apiUrl(`${paths.productos}/Buscar?nombre=${encodeURIComponent(v)}`)); }
  catch (err) { showAlert(err.message, 'danger'); }
});

document.getElementById('btnBuscarUsuario')?.addEventListener('click', async () => {
  const v = document.getElementById('buscarUsuario').value.trim();
  if (!v) return loadUsuarios();
  try { await loadUsuarios(apiUrl(`${paths.usuarios}/Buscar?nombre=${encodeURIComponent(v)}`)); }
  catch (err) { showAlert(err.message, 'danger'); }
});

document.getElementById('formConfiguracion')?.addEventListener('submit', async (e) => {
  e.preventDefault();

  const currentApiBaseUrl = getApiBaseUrl();
  const apiBaseUrl = document.getElementById('configApiBaseUrl').value.trim();
  const connectionString = document.getElementById('configConnectionString').value.trim();

  try {
    const configUrl = currentApiBaseUrl
      ? `${currentApiBaseUrl.replace(/\/$/, '')}${paths.configuracion}`
      : paths.configuracion;

    await request(configUrl, {
      method: 'PUT',
      body: JSON.stringify({ connectionString })
    });

    if (apiBaseUrl) {
      localStorage.setItem(API_BASE_KEY, apiBaseUrl);
    } else {
      localStorage.removeItem(API_BASE_KEY);
    }

    showAlert('Configuración guardada.');
    await Promise.all([loadClientes(), loadProductos(), loadUsuarios(), loadVentas()]);
  } catch (err) {
    showAlert(err.message, 'danger');
  }
});

document.getElementById('btnRecargarClientes')?.addEventListener('click', () => loadClientes().catch(err => showAlert(err.message, 'danger')));
document.getElementById('btnRecargarProductos')?.addEventListener('click', () => loadProductos().catch(err => showAlert(err.message, 'danger')));
document.getElementById('btnRecargarUsuarios')?.addEventListener('click', () => loadUsuarios().catch(err => showAlert(err.message, 'danger')));
document.getElementById('btnRecargarVentas')?.addEventListener('click', () => loadVentas().catch(err => showAlert(err.message, 'danger')));

document.addEventListener('click', async (e) => {
  const action = e.target.dataset.action;
  if (!action) return;

  try {
    if (action === 'del-cliente') {
      await request(apiUrl(`${paths.clientes}/${e.target.dataset.id}`), { method: 'DELETE' });
      await loadClientes();
      showAlert('Cliente eliminado.');
    }

    if (action === 'del-producto') {
      await request(apiUrl(`${paths.productos}/${e.target.dataset.id}`), { method: 'DELETE' });
      await loadProductos();
      showAlert('Producto eliminado.');
    }

    if (action === 'toggle-usuario') {
      await request(apiUrl(`${paths.usuarios}/CambiarEstado/${e.target.dataset.id}`), {
        method: 'PUT',
        body: JSON.stringify(e.target.dataset.state === 'true')
      });
      await loadUsuarios();
      showAlert('Estado de usuario actualizado.');
    }
  } catch (err) {
    showAlert(err.message, 'danger');
  }
});

document.getElementById('formLogin')?.addEventListener('submit', async (e) => {
  e.preventDefault();
  const f = new FormData(e.target);

  const nombre = String(f.get('nombre') ?? '').trim();
  const password = String(f.get('password') ?? '').trim();

  try {
    const auth = await request(apiUrl('/api/Auth/login'), {
      method: 'POST',
      body: JSON.stringify({ nombre, password })
    });

    const token = auth?.token ?? auth?.Token;
    const idUsuario = auth?.idUsuario ?? auth?.IdUsuario;
    const nombreUsuario = auth?.nombre ?? auth?.Nombre;
    const rolUsuario = auth?.rol ?? auth?.Rol;

    if (!token) {
      throw new Error('Respuesta de login inválida: no se recibió token.');
    }

    setSession(token, {
      idUsuario,
      nombre: nombreUsuario,
      rol: rolUsuario
    });

    showApp();
    applyRolePermissions();

    const isAdmin = rolUsuario === 'Administrador';
    const tasks = [loadClientes(), loadProductos(), loadVentas()];
    if (isAdmin) {
      tasks.push(loadUsuarios());
      await loadConfigForm();
    }
    await Promise.all(tasks);

    return;
  } catch (err) {
    showAlert(err.message, 'danger');
  }
});

document.getElementById('btnLogout')?.addEventListener('click', () => {
  clearSession();
  applyRolePermissions();
  showLogin();
});

(async function init() {
  const token = getToken();
  if (!token) {
    showLogin();
    return;
  }

  showApp();
  applyRolePermissions();

  try {
    const user = getCurrentUser();
    const isAdmin = user?.rol === 'Administrador';
    const tasks = [loadClientes(), loadProductos(), loadVentas()];
    if (isAdmin) {
      tasks.push(loadUsuarios());
      await loadConfigForm();
    }
    await Promise.all(tasks);
  } catch (err) {
    showApp();
    applyRolePermissions();
    showAlert(err.message ?? 'No se pudo cargar el menú principal.', 'danger');
  }
})();

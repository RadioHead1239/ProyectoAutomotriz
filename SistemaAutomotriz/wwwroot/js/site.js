document.addEventListener('change', function (e) {
    if (e.target.matches('select[data-autoenviar]')) e.target.form?.submit();
});

document.addEventListener('click', function (e) {
    const boton = e.target.closest('[data-ver-clave]');
    if (!boton) return;
    const input = boton.closest('.input-clave')?.querySelector('input');
    if (!input) return;
    const mostrar = input.type === 'password';
    input.type = mostrar ? 'text' : 'password';
    boton.setAttribute('aria-label', mostrar ? 'Ocultar contraseña' : 'Mostrar contraseña');
    boton.setAttribute('aria-pressed', String(mostrar));
});

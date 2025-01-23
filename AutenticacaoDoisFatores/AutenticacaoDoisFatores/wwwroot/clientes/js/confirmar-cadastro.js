document.getElementById('form-confirmar-cadastro').addEventListener('submit', function (event) {
    event.preventDefault();

    const params = new URLSearchParams(document.location.search);
    const token = params.get("token");

    fetch('https://localhost:7053/api/cliente/confirmar-cadastro', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    })
        .then(result => {
            if (result.status === 401) {
                alert('O link expirou');
                return;
            }

            console.log('Sucesso: ', result);
        })
        .catch(error => console.error('Erro: ', error));
});
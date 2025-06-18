document.getElementById('form-principal').addEventListener('submit', function (event) {
    showWaitingMessage();

    event.preventDefault();

    const params = new URLSearchParams(document.location.search);
    const token = params.get("token");

    const baseUrl = `${window.location.protocol}//${window.location.hostname}/api`;

    fetch(`${baseUrl}/client/generate-new-key`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    })
        .then(async response => {
            if (response.status === 401) {
                showWarningMessage('O link expirou');
                return;
            }

            if (!response.success && response.status === 422) {
                const content = await response.json();

                if (content.length > 0) {
                    const msg = content[0].message;
                    showWarningMessage(msg);
                }

                return;
            }

            showSuccessMessage();
        })
        .catch(erro => showErrorMessage(`Erro: ${erro}`));
});

function showSuccessMessage() {
    showMessage('A nova chave será enviada em seu e-mail!', 'green');
}

function showWarningMessage(msg) {
    showMessage(msg, 'orange');
}

function showWaitingMessage() {
    showMessage('Processando... Por favor, aguarde!', 'black');
}

function showErrorMessage(msg) {
    console.log(msg);
    showMessage('Não foi possível realizar o processo.', 'red');
}

function showMessage(msg, color) {
    document.getElementById('message').textContent = msg;
    document.getElementById('message').style.color = color;
}
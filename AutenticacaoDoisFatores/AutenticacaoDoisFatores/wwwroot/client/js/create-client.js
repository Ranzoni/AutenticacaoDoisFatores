document.getElementById('principal-form').addEventListener('submit', function (event) {
    showWaitingMessage();

    event.preventDefault();

    const params = new URLSearchParams(document.location.search);
    const token = params.get("token");

    const port = window.location.port ? `:${window.location.port}` : '';
    const baseUrl = `${window.location.protocol}//${window.location.hostname}${port}/api`;

    const clientName = document.getElementById('client-name').value;
    const domainName = removeSpecialCharacters(clientName);
    const email = document.getElementById('client-email').value;
    const adminPassword = document.getElementById('client-password').value;

    const body = {
        name: clientName,
        domainName: domainName,
        email: email,
        adminPassword: adminPassword
    };

    fetch(`${baseUrl}/client`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(body)
    })
        .then(async response => {
            if (response.status === 401) {
                showWarningMessage('O link expirou');
                return;
            }

            if (!response.ok && response.status === 422) {
                const content = await response.json();

                if (content.length > 0) {
                    const msg = content[0].message;
                    showWarningMessage(msg);
                }

                return;
            }

            if (!response.ok) {
                showErrorMessage('Não foi possível concluir a operação.');
                console.log(response);
                return;
            }

            showSuccessMessage();
        })
        .catch(erro => showErrorMessage(`Erro: ${erro}`));
});

function removeSpecialCharacters(str) {
    return str.replace(/[^a-z0-9]/g, '');
}

function showSuccessMessage() {
    showMessage('Cadastro confirmado com sucesso!', 'green');
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
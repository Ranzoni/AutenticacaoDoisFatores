document.getElementById('form-principal').addEventListener('submit', function (event) {
    apresentarMsgAguardando();

    event.preventDefault();

    const parametros = new URLSearchParams(document.location.search);
    const token = parametros.get("token");

    const siteBase = `${window.location.protocol}//${window.location.hostname}/api`;

    fetch(`${siteBase}/cliente/gerar-nova-chave`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    })
        .then(async resposta => {
            if (resposta.status === 401) {
                apresentarMsgAlerta('O link expirou');
                return;
            }

            if (!resposta.success && resposta.status === 422) {
                const conteudo = await resposta.json();

                if (conteudo.length > 0) {
                    const msg = conteudo[0].mensagem;
                    apresentarMsgAlerta(msg);
                }

                return;
            }

            apresentarMsgSucesso();
        })
        .catch(erro => apresentarMsgErro(`Erro: ${erro}`));
});

function apresentarMsgSucesso() {
    apresentarMsg('A nova chave será enviada em seu e-mail!', 'green');
}

function apresentarMsgAlerta(msg) {
    apresentarMsg(msg, 'orange');
}

function apresentarMsgAguardando() {
    apresentarMsg('Processando... Por favor, aguarde!', 'black');
}

function apresentarMsgErro(msg) {
    console.log(msg);
    apresentarMsg('Não foi possível realizar o processo.', 'red');
}

function apresentarMsg(msg, color) {
    document.getElementById('mensagem').textContent = msg;
    document.getElementById('mensagem').style.color = color;
}
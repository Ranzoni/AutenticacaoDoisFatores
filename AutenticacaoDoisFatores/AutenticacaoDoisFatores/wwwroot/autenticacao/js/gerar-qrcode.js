var qrcode = new QRCode(document.getElementById("qrcode"), {
    width: 100,
    height: 100,
    useSVG: true
});

function makeCode() {
    const parametros = new URLSearchParams(document.location.search);
    const qrCodeEmstring = parametros.get("qrCode");

    qrcode.makeCode(qrCodeEmstring);
}

makeCode();
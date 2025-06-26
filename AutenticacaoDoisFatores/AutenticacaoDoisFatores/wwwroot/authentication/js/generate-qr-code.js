const qrcode = new QRCode(document.getElementById("qrcode"), {
    width: 100,
    height: 100,
    useSVG: true
});

function makeCode() {
    const params = new URLSearchParams(document.location.search);
    const qrCodeValue = params.get("qrCode");

    qrcode.makeCode(qrCodeValue);
}

makeCode();
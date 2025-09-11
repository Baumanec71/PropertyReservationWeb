const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const forge = require('node-forge');

const certsDir = path.join(__dirname, 'certs');
if (!fs.existsSync(certsDir)) {
  fs.mkdirSync(certsDir);
}

const keys = forge.pki.rsa.generateKeyPair(2048);
const cert = forge.pki.createCertificate();

cert.publicKey = keys.publicKey;
cert.serialNumber = crypto.randomBytes(16).toString('hex');
cert.validity.notBefore = new Date();
cert.validity.notAfter = new Date();
cert.validity.notAfter.setFullYear(cert.validity.notBefore.getFullYear() + 1);

const attrs = [
  { name: 'commonName', value: 'localhost' },
];
cert.setSubject(attrs);
cert.setIssuer(attrs);

cert.setExtensions([
  { name: 'basicConstraints', cA: false },
  { name: 'keyUsage', digitalSignature: true, keyEncipherment: true },
  { name: 'extKeyUsage', serverAuth: true },
  { name: 'subjectAltName', altNames: [{ type: 2, value: 'localhost' }] },
]);

cert.sign(keys.privateKey);

const privateKeyPem = forge.pki.privateKeyToPem(keys.privateKey);
const certPem = forge.pki.certificateToPem(cert);

fs.writeFileSync(path.join(certsDir, 'localhost-key.pem'), privateKeyPem);
fs.writeFileSync(path.join(certsDir, 'localhost.pem'), certPem);

console.log('✅ Сертификаты localhost.pem и localhost-key.pem успешно созданы в папке certs/');
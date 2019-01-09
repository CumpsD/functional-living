# Generate a Certificate

## Generate CSR + Private Key

```bash
rm example.{csr,key}
openssl req -config example.conf -new -out example.csr
openssl req -text -noout -verify -in example.csr
openssl rsa -in example.key -check
```

## Self-sign the development certificate

```bash
openssl x509 -req -days 365 -in example.csr -signkey example.key -out example.crt
```

## Convert a DER file (.crt .cer .der) to PEM

```bash
openssl x509 -inform der -in example.der -out example.crt
```

## Inspect certificate

```bash
openssl x509 -in example.crt -text -noout
```

## Convert a PEM certificate file and a private key to PKCS#12 (.pfx .p12)

```bash
openssl pkcs12 -export -out example.pfx -inkey example.key -in example.crt
```

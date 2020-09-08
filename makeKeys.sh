#!/bin/bash

KEYNAME="dev"

RSAKEYLENGTH=2048

PRIVATE_KEY=${KEYNAME}.privateKey.rsa.pem
PUBLIC_KEY=${KEYNAME}.publicKey.rsa.pem

echo "Generating RSA keys..."
openssl genrsa -out ${PRIVATE_KEY} ${RSAKEYLENGTH}
openssl rsa -in ${PRIVATE_KEY} -pubout -RSAPublicKey_out -out ${PUBLIC_KEY}

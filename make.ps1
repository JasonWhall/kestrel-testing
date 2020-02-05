# Generate Self Signed Cert
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365 -nodes -subj "/C=GB/ST=Example/L=A-Place/O=Test/CN=Root-CA"

# Build and run
docker build -t kestrel-test .
docker run -d -p5001:5001 kestrel-test
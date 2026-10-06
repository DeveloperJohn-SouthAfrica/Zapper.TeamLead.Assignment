Place the Cloudflare origin certificate, key, and CA bundle here so the nginx reverse proxy can terminate TLS inside the compose stack.

Required files (replace these placeholders with the real PEM files):

- `cf-origin-cert.pem`
- `cf-origin-key.pem`
- `cf-origin-ca.pem`

These files are ignored by git so you can keep secrets local. The compose stack mounts them at `/etc/nginx/certs` and the `nginx` service reads the paths from the environment.

# Varnish
WPF File Encryptor and encrypted image viewer using PRISM.

## Considerations
1. The password security is poor.  The plain text password is assigned to a variable and passed to a service.
2. PRISM is not really used extensively enough to warrent its inclusion.

## To Do
- [ ] Add unit tests.
- [ ] Rename "photo viewer" to image viewer.
- [ ] Address project namespaces.
- [ ] Consider creating file extension .varn
- [ ] Consider storing file information at the start of an encrypted file.
- [ ] Make async.
- [ ] fix view models and services to use either a secure string or the encrypted hash.  

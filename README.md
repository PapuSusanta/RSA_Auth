# RSA Authentication

This project implements RSA-based authentication for secure communication.

## Features

- RSA key generation
- Public key encryption
- Private key decryption
- Digital signature creation and verification
- HttpOnly cookie storage for tokens
- Secure token validation on the server
- Protection against XSS attacks

## How It Works

1. **Login Process:**

   - User submits credentials
   - Server validates credentials and generates a JWT
   - JWT is set as an HttpOnly cookie in the response

2. **Token Storage:**

   - The JWT is stored in an HttpOnly cookie
   - This prevents client-side JavaScript from accessing the token

3. **Request Authentication:**

   - The HttpOnly cookie is automatically sent with each request to the server
   - Server extracts the JWT from the cookie
   - Server validates the JWT for each protected route

4. **Logout Process:**
   - Server clears the HttpOnly cookie containing the JWT

## Security Benefits

- **XSS Protection:** Since the token is in an HttpOnly cookie, it cannot be accessed by malicious scripts injected into the page.
- **CSRF Protection:** Implemented using the SameSite cookie attribute.
- **Secure Flag:** Ensures the cookie is only transmitted over HTTPS (in production).

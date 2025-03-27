// src/utils/validation.ts

export const validateUsername = (username: string): string | null => {
    if (!/^[A-Za-z_]/.test(username)) {
      return "A felhasználónév nem kezdődhet számmal vagy speciális karakterrel.";
    }
    if (!/^[A-Za-z0-9_]+$/.test(username)) {
      return "A felhasználónév csak betűket, számokat és aláhúzásjeleket tartalmazhat.";
    }
    if (username.length < 3) {
      return "A felhasználónév legalább 3 karakter hosszú kell legyen.";
    }
    if (username.length > 20) {
      return "A felhasználónév legfeljebb 20 karakter hosszú lehet.";
    }
    return null;
  };
  
  export const validatePassword = (
    password: string,
    username?: string,
    email?: string
  ): string | null => {
    if (password.length < 8) {
      return "A jelszónak legalább 8 karakter hosszúnak kell lennie.";
    }
    if (!/[A-Z]/.test(password)) {
      return "A jelszónak tartalmaznia kell legalább egy nagybetűt.";
    }
    if (!/[a-z]/.test(password)) {
      return "A jelszónak tartalmaznia kell legalább egy kisbetűt.";
    }
    if (!/[0-9]/.test(password)) {
      return "A jelszónak tartalmaznia kell legalább egy számot.";
    }
    if (!/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
      return "A jelszónak tartalmaznia kell legalább egy speciális karaktert.";
    }
    if (/\s/.test(password)) {
      return "A jelszó nem tartalmazhat szóközöket.";
    }
    if (username && password.toLowerCase().includes(username.toLowerCase())) {
      return "A jelszó nem tartalmazhatja a felhasználónevet.";
    }
    if (email && password.toLowerCase().includes(email.toLowerCase())) {
      return "A jelszó nem tartalmazhatja az e-mail címet.";
    }
    return null;
  };
  
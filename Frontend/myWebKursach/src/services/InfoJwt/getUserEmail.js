import { jwtDecode } from "jwt-decode"; // Используем именованный экспорт

export const getUserEmail = () => {
    const token = localStorage.getItem("authToken");

    if (token) {
        try {
            const decodedToken = jwtDecode(token); // Декодируем токен

            // Правильный путь к роли в токене
            const email = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];


            return email || null; // Возвращаем роль или null, если её нет
        } catch (error) {
            console.error("Ошибка при декодировании токена:", error);
            return null;
        }
    }

    return null;
};
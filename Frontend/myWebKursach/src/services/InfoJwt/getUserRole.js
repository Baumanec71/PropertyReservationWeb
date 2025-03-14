import { jwtDecode } from "jwt-decode"; // Используем именованный экспорт

export const getUserRole = () => {
    const token = localStorage.getItem("authToken");

    if (token) {
        try {
            const decodedToken = jwtDecode(token); // Декодируем токен
            // Правильный путь к роли в токене
            const role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

            return role || null; // Возвращаем роль или null, если её нет
        } catch (error) {
            console.error("Ошибка при декодировании токена:", error);
            return null;
        }
    }

    return null;
};
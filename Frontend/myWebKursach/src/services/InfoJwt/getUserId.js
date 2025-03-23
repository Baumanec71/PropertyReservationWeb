import { jwtDecode } from "jwt-decode"; // Используем именованный экспорт

export const getUserId = () => {
    const token = localStorage.getItem("authToken");

    if (token) {
        try {
            const decodedToken = jwtDecode(token); // Декодируем токен

            // Правильный путь к ID в токене
            const id = decodedToken["sub"]; // sub - стандартный claim для ID пользователя

            return id || null; // Возвращаем ID или null, если его нет
        } catch (error) {
            console.error("Ошибка при декодировании токена:", error);
            return null;
        }
    }

    return null;
};
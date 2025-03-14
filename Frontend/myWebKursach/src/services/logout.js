import axios from "axios";

export const logout = async () => {
    try {
        const token = localStorage.getItem("authToken"); // Получаем токен из localStorage
        
        if (!token) {
            return { success: false, errors: ["Вы не авторизованы"] };
        }

        const response = await axios.post(
            "https://localhost:7069/api/Auth/logout",
            {},
            {
                headers: {
                    Authorization: `Bearer ${token}`, // Передаём токен
                },
                withCredentials: true
            }
        );

        if (response.status === 200) {
            localStorage.removeItem("authToken"); // Удаляем токен после успешного выхода
            console.log("Выход выполнен");
            return { success: true, message: response.data.Message };
        }

        return { success: false, errors: ["Ошибка при выходе. Попробуйте снова."] };
    } catch (error) {
        return {
            success: false,
            errors: [error.response?.data || "Ошибка сервера. Попробуйте позже."]
        };
    }
};
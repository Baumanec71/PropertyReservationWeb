import axios from "axios";

export const getConflicts = async (page = 1) => {
    try {
        const token = localStorage.getItem("authToken");
        if (!token) {
            return { success: false, error: "Токен отсутствует, авторизуйтесь снова." };
        }

        const config = {
            headers: {
                Authorization: `Bearer ${token}`,
                Accept: "application/json",
                "Content-Type": "application/json",
            },
            withCredentials: true, // Для работы с cookies, если необходимо
        };

        // Запрос на получение данных о конфликтах
        const response = await axios.get(
            `${API_BASE_URL}/api/Conflict/GetConflicts?page=${page}`,
            config
        );

        if (response.status === 200) {
            return { success: true, data: response.data };
        } else {
            return { success: false, error: response.data.Description || "Неизвестная ошибка" };
        }
    } catch (error) {
        console.error("Ошибка при получении конфликтов:", error);
        return { success: false, error: error.message || "Что-то пошло не так" };
    }
};
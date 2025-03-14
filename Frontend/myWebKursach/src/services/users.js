import axios from "axios";

export const getUsers = async (page = 1) => {
    try {
        const token = localStorage.getItem("authToken"); // Достаем токен
        if (!token) {
            return { success: false, errors: ["Токен отсутствует, авторизуйтесь снова."] };
        }
        console.log(token);
        const response = await axios.get("https://localhost:7069/api/User/GetUsers", {
            params: { page },
            headers: {
                Authorization: `Bearer ${token}`, 
                'accept': '*/*', // Заголовок accept, как в cURL
            },
            withCredentials: true,
        });

        // Убедитесь, что структура данных соответствует ожиданиям
        return response.data; // Ожидается, что response.data будет содержать ViewModels и TotalPages
    } catch (error) {
        console.error("Ошибка при получении пользователей:", error);
        return { viewModels: [], totalPages: 1 }; // Возвращаем дефолтные значения в случае ошибки
    }
};



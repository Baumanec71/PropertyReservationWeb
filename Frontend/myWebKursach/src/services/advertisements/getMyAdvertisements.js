import axios from "axios";

export const getMyAdvertisements = async (page = 1) => {
    try {
        const token = localStorage.getItem("authToken"); // Достаем токен
        if (!token) {
            return { success: false, errors: ["Токен отсутствует, авторизуйтесь снова."] };
        }
        console.log(token);
        const response = await axios.get(`${API_BASE_URL}/api/Advertisement/GetMyAdvertisements`, {
            params: { page },
            headers: {
                Authorization: `Bearer ${token}`, 
                'accept': '*/*',
            },
            withCredentials: true,
        });

        return response.data;
    } catch (error) {
        console.error("Ошибка при получении пользователей:", error);
        return { viewModels: [], totalPages: 1 };
    }
};
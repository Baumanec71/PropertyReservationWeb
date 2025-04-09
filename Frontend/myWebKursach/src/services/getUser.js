import axios from "axios";

export const getUser = async (id) => {
    try {

        const response = await axios.get(`${API_BASE_URL}/api/User/GetUserId?id=${id}`, {
            headers: {
                'accept': '*/*', // Заголовок accept, как в cURL
            },
            withCredentials: true,
        });
        return { success: true, data: response.data };
    } catch (error) {
        console.error("Ошибка при получении пользователя:", error);
        return { success: false, error: error.response?.data?.error || "Неизвестная ошибка" };
    }
};
import axios from "axios";

export const getAdvertisement = async (id) => {
    try {

        const response = await axios.get(`${API_BASE_URL}/api/Advertisement/GetAdvertisement?id=${id}`, {
            headers: {
                'accept': '*/*', // Заголовок accept, как в cURL
            },
            withCredentials: true,
        });

        return { success: true, data: response.data };
    } catch (error) {
        console.error("Ошибка при получении рекламы:", error);
        return { success: false, error: error.response?.data?.error || "Неизвестная ошибка" };
    }
};
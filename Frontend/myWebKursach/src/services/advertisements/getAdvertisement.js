import axios from "axios";

export const getAdvertisement = async (id) => {
    try {

        const response = await axios.get(`https://localhost:7069/api/Advertisement/GetAdvertisement?id=${id}`, {
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
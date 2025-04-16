import axios from "axios";

const getAuthHeaders = () => {
    const token = localStorage.getItem("authToken");
    if (!token) {
        return null;
    }
    return {
        Authorization: `Bearer ${token}`,
        Accept: "*/*",
        "Content-Type": "application/json",
    };
};

export const getReviewsForAdvertisement = async (idAdvertisement, page = 1) => {
    try {
        const headers = getAuthHeaders();
        if (!headers) {
            return { success: false, error: "Токен отсутствует, авторизуйтесь снова." };
        }

        const config = { headers };

        const response = await axios.get(
            `${API_BASE_URL}/api/Review/GetReviewsForAdvertisement?idAdvertisement=${idAdvertisement}&page=${page}`,
            config
        );

        return response.data;
    } catch (error) {
        console.error("Ошибка при получении отзывов для объявления:", error);
        return { viewModels: [], totalPages: 1 };
    }
};
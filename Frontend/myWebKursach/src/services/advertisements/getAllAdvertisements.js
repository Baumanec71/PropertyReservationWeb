import axios from "axios";

export const getAllAdvertisements = async (page = 1, filterModel = null) => {
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
            withCredentials: true,
        };

        let response;

        if (!filterModel) {
            response = await axios.get(
                `${API_BASE_URL}/api/Advertisement/GetAllAdvertisements?page=${page}`,
                config
            );
        } else {
            response = await axios.post(
                `${API_BASE_URL}/api/Advertisement/GetAllAdvertisements?page=${page}`,
                filterModel,
                config
            );
        }

        return response.data;
    } catch (error) {
        console.error("Ошибка при получении объявлений:", error);
        return { viewModels: [], totalPages: 1 };
    }
};

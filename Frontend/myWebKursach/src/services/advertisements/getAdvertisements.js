import axios from "axios";

export const getAdvertisements = async (page = 1, filterModel = null) => {
    try {

        const config = {
            headers: {
                Accept: "application/json",
                "Content-Type": "application/json",
            },
            withCredentials: true,
        };

        let response;

        if (!filterModel) {
            response = await axios.get(
                `${API_BASE_URL}/api/Advertisement/GetAdvertisements?page=${page}`,
                config
            );
        } else {
            response = await axios.post(
                `${API_BASE_URL}/api/Advertisement/GetAdvertisements?page=${page}`,
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

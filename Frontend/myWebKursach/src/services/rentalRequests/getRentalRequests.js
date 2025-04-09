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

export const getRentalRequests = async (page = 1, idAdvertisement = null, filterModel = null) => {
    try {
        const headers = getAuthHeaders();
        if (!headers) {
            return { success: false, error: "Токен отсутствует, авторизуйтесь снова." };
        }

        const config = { headers };

        let response;

        if (!filterModel) {
            response = await axios.get(
                `${API_BASE_URL}/api/RentalRequest/GetRentalRequests?idAdvertisement=${idAdvertisement}&page=${page}`,
                config
            );
        } else {
            filterModel.selectedIdNeedAdvertisement = idAdvertisement;
            response = await axios.post(
                `${API_BASE_URL}/api/RentalRequest/GetRentalRequestsFiltered?page=${page}`,
                filterModel,
                config
            );
        }

        return response.data;
    } catch (error) {
        console.error("Ошибка при получении заявок на аренду:", error);
        return { viewModels: [], totalPages: 1 };
    }
};
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

export const createReview = async (reviewData) => {
    try {
        const headers = getAuthHeaders();
        if (!headers) {
            return { success: false, error: "Токен отсутствует, авторизуйтесь снова." };
        }

        const response = await axios.post(`${API_BASE_URL}/api/Review/CreateReview`, reviewData, {
            headers,
            withCredentials: true,
        });

        return { success: true, data: response.data };
    } catch (error) {
        if (error.response) {
            if (error.response.status === 400 && error.response.data.error) {
                return { success: false, errors: [error.response.data.error] };
            }
            if (error.response.data && error.response.data.errors) {
                return { success: false, errors: error.response.data.errors };
            }
        }
        return { success: false, errors: [error.response.data]};
    }
};
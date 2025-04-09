import axios from "axios";

export const updateAdvertisement = async (ad, id) => {
    try {
        const token = localStorage.getItem("authToken");
        if (!token) {
            return { success: false, error: "Токен отсутствует, авторизуйтесь снова." };
        }

        const response = await axios.put(
            `${API_BASE_URL}/api/Advertisement/Update?id=${id}`,
            ad,
            {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'accept': '*/*',
                    'Content-Type': 'application/json'
                },
                withCredentials: true,
            }
        );

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
        console.log(error);
        return { success: false, errors: [error.response.data]};
    }
};
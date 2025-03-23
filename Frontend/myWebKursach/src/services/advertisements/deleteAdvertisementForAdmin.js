import axios from "axios";

export const deleteAdvertisementForAdmin = async (id) => {
    try {
        const token = localStorage.getItem("authToken");
        console.log("Токен в профиле получен");

        if (!token) {
            return { success: false, error: "Токен отсутствует, авторизуйтесь снова." };
        }

        const response = await axios.put(
            `https://localhost:7069/api/Advertisement/DeleteAdvertisementForAdmin?id=${id}`,
            {},
            {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Accept': '*/*',
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
        return { success: false, errors: [error.response?.data] };
    }
};
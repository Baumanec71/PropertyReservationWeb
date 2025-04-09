import axios from "axios";

export const profile = async () => {
    try {
        const token = localStorage.getItem("authToken");
        console.log("Токен в профиле получен")
        if (!token) {
            return { success: false, error: "Токен отсутствует, авторизуйтесь снова." };
        }

        const response = await axios.get(`${API_BASE_URL}/api/User/me`, {
    
            headers: {
                Authorization: `Bearer ${token}`, 
                'accept': '*/*',
            },
            withCredentials: true,
        });

        return { success: true, data: response.data };
    } catch (error) {
        console.error("Ошибка при получении профиля:", error);
        return { success: false, error: error.response?.data?.error || "Неизвестная ошибка" };
    }
};
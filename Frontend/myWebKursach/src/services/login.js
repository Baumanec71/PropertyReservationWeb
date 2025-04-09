import axios from "axios";

export const login = async (email, password) => {
    try {
        const response = await axios.post(`${API_BASE_URL}/api/Auth/login`, {
            email,
            password
        }, { withCredentials: true });

        if (response.status === 200 && response.data.authenticated) {
            const token = response.data.token;
            localStorage.setItem("authToken", token);
            return { success: true, data: response.data };
        }

        return { success: false, errors: ["Ошибка входа. Попробуйте снова."] };
    } catch (error) {
        if (error.response) {
            if (error.response.status === 401) {
                return { success: false, errors: ["Неверный email или пароль"] };
            }
            if (error.response.data && error.response.data.error) {
                return { success: false, errors: [error.response.data.error] };
            }
        }
        return { success: false, errors: ["Ошибка сервера. Попробуйте позже."] };
    }
};